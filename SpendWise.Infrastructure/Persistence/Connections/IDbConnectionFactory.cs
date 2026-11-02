using Microsoft.Data.SqlClient;

namespace SpendWise.Infrastructure.Persistence.Connections;

// Defines how the application creates SQL Server connections.
public interface IDbConnectionFactory
{
    // Create a new database connection.
    SqlConnection CreateConnection();
}