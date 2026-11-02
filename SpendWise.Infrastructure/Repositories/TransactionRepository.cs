using Dapper;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Domain.Entities;
using SpendWise.Infrastructure.Persistence.Connections;

namespace SpendWise.Infrastructure.Repositories;

public sealed class TransactionRepository(IDbConnectionFactory factory) : ITransactionRepository
{
    private const string Cols="id Id,household_id HouseholdId,user_id UserId,paid_by_user_id PaidByUserId,category_id CategoryId,suggested_category_id SuggestedCategoryId,type Type,amount Amount,shop_name ShopName,description Description,transaction_at TransactionAt,payment_method PaymentMethod,notes Notes,beneficiary_type BeneficiaryType,beneficiary_user_id BeneficiaryUserId,created_at CreatedAt,created_by CreatedBy,updated_at UpdatedAt,updated_by UpdatedBy,deleted_at DeletedAt,deleted_by DeletedBy";

    public async Task<Transaction?> GetByIdAsync(Guid id,Guid householdId,Guid userId,CancellationToken ct)
    {
        var sql=$"SELECT {Cols} FROM transactions WHERE id=@id AND household_id=@householdId AND deleted_at IS NULL;";
        await using var c=factory.CreateConnection(); return await c.QuerySingleOrDefaultAsync<Transaction>(new CommandDefinition(sql,new {id,householdId},cancellationToken:ct));
    }
    public async Task<(IReadOnlyList<Transaction> Items,int TotalCount)> GetPagedAsync(TransactionFilter f,CancellationToken ct)
    {
        const string where=" FROM transactions WHERE household_id=@HouseholdId AND deleted_at IS NULL AND (@From IS NULL OR transaction_at>=@From) AND (@To IS NULL OR transaction_at<=@To) AND (@CategoryId IS NULL OR category_id=@CategoryId) AND (@Type IS NULL OR type=CASE WHEN LOWER(@Type)='income' THEN 1 ELSE 2 END)";
        var args=new {f.HouseholdId,f.From,f.To,f.CategoryId,f.Type,Offset=(f.Page-1)*f.PageSize,f.PageSize};
        await using var c=factory.CreateConnection();
        var count=await c.ExecuteScalarAsync<int>(new CommandDefinition("SELECT COUNT(1)"+where,args,cancellationToken:ct));
        var rows=await c.QueryAsync<Transaction>(new CommandDefinition($"SELECT {Cols}"+where+" ORDER BY transaction_at DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;",args,cancellationToken:ct));
        return (rows.AsList(),count);
    }
    public async Task<Transaction> CreateAsync(Transaction x,CancellationToken ct)
    {
        const string sql="INSERT INTO transactions(id,household_id,user_id,paid_by_user_id,category_id,suggested_category_id,type,amount,shop_name,description,transaction_at,payment_method,notes,beneficiary_type,beneficiary_user_id,created_at,created_by,updated_at,updated_by,deleted_at,deleted_by) VALUES(@Id,@HouseholdId,@UserId,@PaidByUserId,@CategoryId,@SuggestedCategoryId,@Type,@Amount,@ShopName,@Description,@TransactionAt,@PaymentMethod,@Notes,@BeneficiaryType,@BeneficiaryUserId,@CreatedAt,@CreatedBy,@UpdatedAt,@UpdatedBy,@DeletedAt,@DeletedBy);";
        await using var c=factory.CreateConnection();
        await c.ExecuteAsync(new CommandDefinition(sql,new {x.Id,x.HouseholdId,x.UserId,x.PaidByUserId,x.CategoryId,x.SuggestedCategoryId,Type=(int)x.Type,x.Amount,x.ShopName,x.Description,x.TransactionAt,x.PaymentMethod,x.Notes,BeneficiaryType=(int)x.BeneficiaryType,x.BeneficiaryUserId,x.CreatedAt,x.CreatedBy,x.UpdatedAt,x.UpdatedBy,x.DeletedAt,x.DeletedBy},cancellationToken:ct)); return x;
    }
    public async Task<bool> UpdateAsync(Transaction x,CancellationToken ct)
    {
        const string sql="UPDATE transactions SET paid_by_user_id=@PaidByUserId,category_id=@CategoryId,type=@Type,amount=@Amount,shop_name=@ShopName,description=@Description,transaction_at=@TransactionAt,payment_method=@PaymentMethod,notes=@Notes,beneficiary_type=@BeneficiaryType,beneficiary_user_id=@BeneficiaryUserId,updated_at=@UpdatedAt,updated_by=@UpdatedBy WHERE id=@Id AND household_id=@HouseholdId AND deleted_at IS NULL;";
        await using var c=factory.CreateConnection(); return await c.ExecuteAsync(new CommandDefinition(sql,new {x.Id,x.HouseholdId,x.PaidByUserId,x.CategoryId,Type=(int)x.Type,x.Amount,x.ShopName,x.Description,x.TransactionAt,x.PaymentMethod,x.Notes,BeneficiaryType=(int)x.BeneficiaryType,x.BeneficiaryUserId,x.UpdatedAt,x.UpdatedBy},cancellationToken:ct))>0;
    }
    public async Task<bool> SoftDeleteAsync(Guid id,Guid householdId,Guid userId,CancellationToken ct)
    {
        const string sql="UPDATE transactions SET deleted_at=@now,deleted_by=@userId WHERE id=@id AND household_id=@householdId AND user_id=@userId AND deleted_at IS NULL;";
        await using var c=factory.CreateConnection(); return await c.ExecuteAsync(new CommandDefinition(sql,new {id,householdId,userId,now=DateTimeOffset.UtcNow},cancellationToken:ct))>0;
    }
    public Task<bool> CategoryBelongsToScopeAsync(Guid categoryId,Guid userId,Guid householdId,CancellationToken ct) =>
        ExistsAsync("SELECT CAST(IIF(EXISTS(SELECT 1 FROM categories WHERE id=@categoryId AND deleted_at IS NULL AND is_active=1 AND (user_id=@userId OR user_id IS NULL) AND (household_id=@householdId OR household_id IS NULL)),1,0) AS bit);",new {categoryId,userId,householdId},ct);
    public Task<bool> IsHouseholdMemberAsync(Guid householdId,Guid userId,CancellationToken ct) =>
        ExistsAsync("SELECT CAST(IIF(EXISTS(SELECT 1 FROM household_members WHERE household_id=@householdId AND user_id=@userId AND is_active=1 AND deleted_at IS NULL),1,0) AS bit);",new {householdId,userId},ct);
    public async Task<TransactionAnalytics> GetAnalyticsAsync(Guid householdId, DateTimeOffset from, DateTimeOffset to, string groupBy, CancellationToken ct)
    {
        var bucket = groupBy.ToLowerInvariant() switch
        {
            "week" => "CONVERT(char(10), DATEADD(day, 1-DATEPART(weekday, transaction_at), CAST(transaction_at AS date)), 23)",
            "month" => "CONVERT(char(7), transaction_at, 126)",
            "year" => "CONVERT(char(4), transaction_at, 112)",
            _ => throw new ArgumentException("groupBy must be week, month, or year.")
        };
        var sql = $"""
            SELECT {bucket} AS Period,
              SUM(CASE WHEN type=1 THEN amount ELSE 0 END) AS Income,
              SUM(CASE WHEN type=2 THEN amount ELSE 0 END) AS Expense
            FROM transactions
            WHERE household_id=@householdId AND deleted_at IS NULL
              AND transaction_at>=@from AND transaction_at<@to
            GROUP BY {bucket} ORDER BY Period;
            """;
        await using var c = factory.CreateConnection();
        var rows = (await c.QueryAsync<AnalyticsPoint>(new CommandDefinition(sql, new { householdId, from, to }, cancellationToken: ct))).AsList();
        var income = rows.Sum(x => x.Income);
        var expense = rows.Sum(x => x.Expense);
        return new TransactionAnalytics(income, expense, income - expense, rows);
    }

    private async Task<bool> ExistsAsync(string sql,object args,CancellationToken ct) { await using var c=factory.CreateConnection(); return await c.ExecuteScalarAsync<bool>(new CommandDefinition(sql,args,cancellationToken:ct)); }
}
