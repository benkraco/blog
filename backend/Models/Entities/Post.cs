public class Post
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}