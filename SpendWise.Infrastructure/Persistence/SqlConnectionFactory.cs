using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SpendWise.Infrastructure.Persistence.Connections;

// Creates database connections using the configured connection string.
public sealed class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;
    public SqlConnectionFactory(IConfiguration configuration)
    {
        // Read the SpendWise database connection string.
        _connectionString = configuration.GetConnectionString("SpendWise")
                            ?? throw new InvalidOperationException("Connection string 'SpendWise' was not found.");
    }

    public SqlConnection CreateConnection()
    {
        // Return a new SQL Server connection.
        return new SqlConnection(_connectionString);
    }
}