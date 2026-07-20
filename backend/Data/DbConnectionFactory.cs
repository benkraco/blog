using System.Data;
using Npgsql;

namespace backend.Data;

public class DbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not found."
            );
    }

    public IDbConnection Create()
    {
        return new NpgsqlConnection(_connectionString);
    }
}