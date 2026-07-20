public class Image
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Alt { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime TakenAt { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public long FileSize { get; set; }
    public  string MimeType { get; set; } = string.Empty;
}