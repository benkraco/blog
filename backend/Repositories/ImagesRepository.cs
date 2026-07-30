using Dapper;
using backend.Data;

namespace backend.Repositories;

public class ImageRepository
{
    private readonly DbConnectionFactory _factory;

    public ImageRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<Image>> GetByPostIdAsync(Guid postId)
    {
        using var connection = _factory.Create();

        const string sql = """
            SELECT *
            FROM "Images"
            WHERE "postId" = @PostId
            ORDER BY "displayOrder"
            """;

        return await connection.QueryAsync<Image>(
            sql,
            new { PostId = postId });
    }

    public async Task<Image?> GetByIdAsync(Guid id)
    {
        using var connection = _factory.Create();

        const string sql = """
            SELECT *
            FROM "Images"
            WHERE id = @Id
            """;

        return await connection.QuerySingleOrDefaultAsync<Image>(
            sql,
            new { Id = id });
    }

    public async Task<Image?> CreateAsync(Image image)
    {
        using var connection = _factory.Create();

        const string sql = """
            INSERT INTO "Images" (
                id,
                "postId",
                url,
                alt,
                "displayOrder",
                name,
                description,
                "takenAt",
                width,
                height,
                "fileSize",
                "mimeType"
            )
            VALUES (
                @Id,
                @PostId,
                @Url,
                @Alt,
                @DisplayOrder,
                @Name,
                @Description,
                @TakenAt,
                @Width,
                @Height,
                @FileSize,
                @MimeType
            )
            RETURNING *
            """;

        return await connection.QuerySingleOrDefaultAsync<Image>(
            sql,
            image);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using var connection = _factory.Create();

        const string sql = """
            DELETE FROM "Images"
            WHERE id = @Id
            """;

        var rowsAffected = await connection.ExecuteAsync(
            sql,
            new { Id = id });

        return rowsAffected > 0;
    }

    public async Task<bool> UpdateDisplayOrderAsync(
    Guid id,
    int displayOrder)
    {
        using var connection = _factory.Create();

        const string sql = """
        UPDATE "Images"
        SET "displayOrder" = @DisplayOrder
        WHERE id = @Id
        """;

        var rowsAffected = await connection.ExecuteAsync(
            sql,
            new
            {
                Id = id,
                DisplayOrder = displayOrder
            });

        return rowsAffected > 0;
    }
}