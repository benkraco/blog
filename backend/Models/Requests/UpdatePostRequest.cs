namespace backend.Models.Requests;

public class UpdatePostRequest
{
    public string Title { get; set; } = string.Empty;

    public IFormFile? MarkdownFile { get; set; }

    public List<IFormFile> Images { get; set; } = new();

    public List<Guid> DeletedImageIds { get; set; } = new();

    public List<string> ImageOrder { get; set; } = new();
}