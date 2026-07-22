using Dapper;
using backend.Data;

namespace backend.Repositories;

public class TagRepository
{
    private readonly DbConnectionFactory _factory;

    public TagRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<Tag>> GetAllAsync()
    {
        using var connection = _factory.Create();

        const string sql = """
            SELECT *
            FROM "Tags"
            """;

        return await connection.QueryAsync<Tag>(sql);
    }

    public async Task<Tag?> GetByIdAsync(Guid  id)
    {
        using var connection = _factory.Create();

        const string sql = """
            SELECT *
            FROM "Tags"
            WHERE id = @Id
            """;

        return await connection.QuerySingleOrDefaultAsync<Tag>(sql, new { Id = id });
    }

    public async Task<Tag?> GetByNameAsync(string name)
    {
        using var connection = _factory.Create();

        const string sql = """
            SELECT *
            FROM "Tags"
            WHERE name = @Name
            """;

        return await connection.QuerySingleOrDefaultAsync<Tag>(sql, new { Name = name });
    }

    public async Task<Tag?> CreateAsync(Tag tag)
    {
        using var connection = _factory.Create();

        const string sql = """
            INSERT INTO "Tags" (
                id,
                name
            )
            VALUES (
                @Id,
                @Name
            )
            RETURNING *
            """;

        return await connection.QuerySingleOrDefaultAsync<Tag>(sql, tag);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using var connection = _factory.Create();

        const string sql = """
            DELETE FROM "Tags"
            WHERE id = @Id
            """;

        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });

        return rowsAffected > 0;
    }
}