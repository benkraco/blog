namespace backend.Models.Requests;

public class CreatePostRequest
{
    public string Title { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public IFormFile? MarkdownFile { get; set; }

    public List<IFormFile> Images { get; set; } = new();
}