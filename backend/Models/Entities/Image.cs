public class Image
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string Url { get; set; }
    public string Alt { get; set; }
    public int DisplayOrder { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime TakenAt { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public long FileSize { get; set; }
    public  string MimeType { get; set; }
}