namespace backend.Services.Storage;

public interface IStorageService
{
    Task<string> UploadAsync(
        Stream fileStream,
        string fileName,
        string contentType);

    Task DeleteAsync(string fileName);
}