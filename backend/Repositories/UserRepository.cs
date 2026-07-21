using Dapper;
using backend.Data;

namespace backend.Repositories;

public class UserRepository
{
    private readonly DbConnectionFactory _factory;

    public UserRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var connection = _factory.Create();

        const string sql = """
            SELECT *
            FROM "Users"
            WHERE "username" = @Username;
            """;

        return await connection.QueryFirstOrDefaultAsync<User>(
            sql,
            new { Username = username }
        );
    }
}