using Dapper;
using backend.Data;

namespace backend.Repositories;

public class PostTagRepository
{
    private readonly DbConnectionFactory _factory;

    public PostTagRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<Tag>> GetTagsByPostIdAsync(Guid postId)
    {
        using var connection = _factory.Create();

        const string sql = """
        SELECT "Tags".*
        FROM "Tags"
        INNER JOIN "PostTags" ON "Tags"."id" = "PostTags"."tagId"
        WHERE "PostTags"."postId" = @PostId
        ORDER BY "Tags"."name"
        """;

        return await connection.QueryAsync<Tag>(sql, new { PostId = postId });
    }

    public async Task<IEnumerable<Post>> GetPostsByTagIdAsync(Guid tagId)
    {
        using var connection = _factory.Create();

        const string sql = """
        SELECT "Posts".*
        FROM "Posts"
        INNER JOIN "PostTags" ON "Posts"."id" = "PostTags"."postId"
        WHERE "PostTags"."tagId" = @TagId
        ORDER BY "Posts"."createdAt" DESC
        """;

        return await connection.QueryAsync<Post>(sql, new { TagId = tagId });
    }

    public async Task<bool> ExistsAsync(Guid postId, Guid tagId)
    {
        using var connection = _factory.Create();

        const string sql = """
            SELECT EXISTS (
                SELECT 1
                FROM "PostTags"
                WHERE "postId" = @PostId
                AND "tagId" = @TagId
            );
            """;

        return await connection.QuerySingleAsync<bool>(sql, new { PostId = postId, TagId = tagId });
    }

    public async Task<bool> AddAsync(Guid postId, Guid tagId)
    {
        using var connection = _factory.Create();

        const string sql = """
            INSERT INTO "PostTags" (
                "postId",
                "tagId"
            )
            VALUES (
                @PostId,
                @TagId
            );
            """;

        var rowsAffected = await connection.ExecuteAsync(sql, new { PostId = postId, TagId = tagId });

        return rowsAffected > 0;
    }

    public async Task<bool> RemoveAsync(Guid postId, Guid tagId)
    {
        using var connection = _factory.Create();

        const string sql = """
            DELETE FROM "PostTags"
            WHERE "postId" = @PostId
            AND "tagId" = @TagId
            """;

        var rowsAffected = await connection.ExecuteAsync(sql, new { PostId = postId, TagId = tagId });

        return rowsAffected > 0;
    }
}