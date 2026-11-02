using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpendWise.Application.Abstractions.Authentication;
using SpendWise.Application.Abstractions.Persistence;
using SpendWise.Infrastructure.Authentication;
using SpendWise.Infrastructure.Persistence.Connections;
using SpendWise.Infrastructure.Repositories;

namespace SpendWise.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IHouseholdRepository, HouseholdRepository>();
        services.AddScoped<IHouseholdMemberRepository, HouseholdMemberRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IUserPreferenceRepository, UserPreferencesRepository>();
        services.AddScoped<IUserFriendRepository, UserFriendRepository>();
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<PasswordHasherService>();
        services.AddScoped<IPasswordHasherService, PasswordHasherService>();
        return services;
    }
}
