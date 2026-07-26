using Amazon.S3;
using Amazon.S3.Model;

namespace backend.Services.Storage;

public class CloudflareR2StorageService : IStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public CloudflareR2StorageService(IConfiguration configuration)
    {
        var accountId = configuration["CloudflareR2:AccountId"];
        var accessKeyId = configuration["CloudflareR2:AccessKeyId"];
        var secretAccessKey = configuration["CloudflareR2:SecretAccessKey"];
        _bucketName = configuration["CloudflareR2:BucketName"]!;

        var endpoint = $"https://{accountId}.r2.cloudflarestorage.com";

        var config = new AmazonS3Config
        {
            ServiceURL = endpoint,
            ForcePathStyle = true
        };

        _s3Client = new AmazonS3Client(
            accessKeyId,
            secretAccessKey,
            config);
    }

    public async Task<string> UploadAsync(
        Stream fileStream,
        string fileName,
        string contentType)
    {
        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = fileName,
            InputStream = fileStream,
            ContentType = contentType
        };

        await _s3Client.PutObjectAsync(request);

        return fileName;
    }

    public async Task DeleteAsync(string fileName)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = fileName
        };

        await _s3Client.DeleteObjectAsync(request);
    }
}