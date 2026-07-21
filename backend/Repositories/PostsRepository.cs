using Dapper;
using backend.Data;

namespace backend.Repositories;

public class PostRepository
{
    private readonly DbConnectionFactory _factory;

    public PostRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<Post>> GetAllAsync()
    {
        using var connection = _factory.Create();

        const string sql = """
            SELECT *
            FROM "Posts"
            ORDER BY "createdAt" DESC
            """;

        return await connection.QueryAsync<Post>(sql);
    }

    public async Task<Post?> GetByIdAsync(Guid id)
    {
        using var connection = _factory.Create();

        const string sql = """
            SELECT *
            FROM "Posts"
            WHERE id = @Id
            """;

        return await connection.QuerySingleOrDefaultAsync<Post>(sql, new { Id = id });
    }

    public async Task<bool> ExistsBySlugAsync(string slug)
    {
        using var connection = _factory.Create();

        const string sql = """
        SELECT EXISTS (
            SELECT 1
            FROM "Posts"
            WHERE slug = @Slug
        );
        """;

        return await connection.QuerySingleAsync<bool>(sql, new { Slug = slug });
    }

    public async Task<Post?> CreateAsync(Post post)
    {
        using var connection = _factory.Create();

        const string sql = """
            INSERT INTO "Posts" (
                id,
                title,
                slug,
                content,
                publishedAt,
                updatedAt,
                createdAt,
            )
            VALUES (
                @Id,
                @Title,
                @Slug,
                @Content,
                @PublishedAt,
                @UpdatedAt,
                @CreatedAt,
            )
            RETURNING *
            """;

        return await connection.QuerySingleOrDefaultAsync<Post>(sql, post);
    }

    public async Task<Post?> UpdateAsync(Post post)
    {
        using var connection = _factory.Create();

        const string sql = """
            UPDATE "Posts"
            SET
                title = @Title,
                content = @Content,
                updatedAt = @UpdatedAt
            WHERE id = @Id
            RETURNING *
            """;

        return await connection.QuerySingleOrDefaultAsync<Post>(sql, post);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using var connection = _factory.Create();

        const string sql = """
            DELETE FROM "Posts"
            WHERE id = @Id
            """;

        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });

        return rowsAffected > 0;
    }
}