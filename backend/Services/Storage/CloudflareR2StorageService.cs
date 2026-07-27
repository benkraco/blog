using System.Security.Cryptography;
using System.Text;

namespace backend.Services.Storage;

public class CloudflareR2StorageService : IStorageService
{
    private readonly string _bucketName;
    private readonly string _accountId;
    private readonly string _accessKeyId;
    private readonly string _secretAccessKey;

    public CloudflareR2StorageService(IConfiguration configuration)
    {
        _accountId = configuration["CloudflareR2:AccountId"]!;
        _accessKeyId = configuration["CloudflareR2:AccessKeyId"]!;
        _secretAccessKey = configuration["CloudflareR2:SecretAccessKey"]!;
        _bucketName = configuration["CloudflareR2:BucketName"]!;
    }

    public async Task<string> UploadAsync(
        Stream fileStream,
        string fileName,
        string contentType)
    {
        var url = $"https://{_accountId}.r2.cloudflarestorage.com/{_bucketName}/{fileName}";
        
        // Leer el stream completamente
        var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream);
        byte[] fileBytes = memoryStream.ToArray();
        
        using var client = new HttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Put, url)
        {
            Content = new ByteArrayContent(fileBytes)
        };

        request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
        request.Headers.Add("Host", $"{_accountId}.r2.cloudflarestorage.com");

        // Firmar la request con AWS SigV4
        SignRequest(request, fileBytes);

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return fileName;
    }

    private void SignRequest(HttpRequestMessage request, byte[] body)
    {
        var now = DateTime.UtcNow;
        var amzDate = now.ToString("yyyyMMddTHHmmssZ");
        var dateStamp = now.ToString("yyyyMMdd");

        // Canonical request components
        var method = "PUT";
        var canonicalUri = request.RequestUri!.AbsolutePath;
        var canonicalQuerystring = "";
        var canonicalHeaders = $"host:{_accountId}.r2.cloudflarestorage.com\nx-amz-content-sha256:UNSIGNED-PAYLOAD\nx-amz-date:{amzDate}\n";
        var signedHeaders = "host;x-amz-content-sha256;x-amz-date";

        var payloadHash = "UNSIGNED-PAYLOAD";

        var canonicalRequest = $"{method}\n{canonicalUri}\n{canonicalQuerystring}\n{canonicalHeaders}\n{signedHeaders}\n{payloadHash}";

        // String to sign
        var algorithm = "AWS4-HMAC-SHA256";
        var credentialScope = $"{dateStamp}/auto/s3/aws4_request";
        var canonicalRequestHash = Sha256Hash(canonicalRequest);
        var stringToSign = $"{algorithm}\n{amzDate}\n{credentialScope}\n{canonicalRequestHash}";

        // Signature
        var signature = CalculateSignature(stringToSign, dateStamp);

        // Authorization header
        var authorizationHeader = $"{algorithm} Credential={_accessKeyId}/{credentialScope}, SignedHeaders={signedHeaders}, Signature={signature}";

        request.Headers.Add("x-amz-date", amzDate);
        request.Headers.Add("x-amz-content-sha256", "UNSIGNED-PAYLOAD");
        request.Headers.TryAddWithoutValidation("Authorization", authorizationHeader);
    }

    private string CalculateSignature(string stringToSign, string dateStamp)
    {
        var kDate = HmacSha256($"AWS4{_secretAccessKey}", dateStamp);
        var kRegion = HmacSha256(kDate, "auto");
        var kService = HmacSha256(kRegion, "s3");
        var kSigning = HmacSha256(kService, "aws4_request");
        var signature = HmacSha256(kSigning, stringToSign);
        return BytesToHex(signature);
    }

    private byte[] HmacSha256(string key, string data)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        return hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
    }

    private byte[] HmacSha256(byte[] key, string data)
    {
        using var hmac = new HMACSHA256(key);
        return hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
    }

    private string Sha256Hash(string input)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BytesToHex(hashedBytes);
    }

    private string BytesToHex(byte[] bytes)
    {
        return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
    }

    public async Task DeleteAsync(string fileName)
    {
        var url = $"https://{_accountId}.r2.cloudflarestorage.com/{_bucketName}/{fileName}";

        using var client = new HttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Delete, url);

        request.Headers.Add("Host", $"{_accountId}.r2.cloudflarestorage.com");

        // Firmar la request
        SignRequestForDelete(request);

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    private void SignRequestForDelete(HttpRequestMessage request)
    {
        var now = DateTime.UtcNow;
        var amzDate = now.ToString("yyyyMMddTHHmmssZ");
        var dateStamp = now.ToString("yyyyMMdd");

        var method = "DELETE";
        var canonicalUri = request.RequestUri!.AbsolutePath;
        var canonicalQuerystring = "";
        var canonicalHeaders = $"host:{_accountId}.r2.cloudflarestorage.com\nx-amz-content-sha256:UNSIGNED-PAYLOAD\nx-amz-date:{amzDate}\n";
        var signedHeaders = "host;x-amz-content-sha256;x-amz-date";
        var payloadHash = "UNSIGNED-PAYLOAD";

        var canonicalRequest = $"{method}\n{canonicalUri}\n{canonicalQuerystring}\n{canonicalHeaders}\n{signedHeaders}\n{payloadHash}";

        var algorithm = "AWS4-HMAC-SHA256";
        var credentialScope = $"{dateStamp}/auto/s3/aws4_request";
        var canonicalRequestHash = Sha256Hash(canonicalRequest);
        var stringToSign = $"{algorithm}\n{amzDate}\n{credentialScope}\n{canonicalRequestHash}";

        var signature = CalculateSignature(stringToSign, dateStamp);
        var authorizationHeader = $"{algorithm} Credential={_accessKeyId}/{credentialScope}, SignedHeaders={signedHeaders}, Signature={signature}";

        request.Headers.Add("x-amz-date", amzDate);
        request.Headers.Add("x-amz-content-sha256", "UNSIGNED-PAYLOAD");
        request.Headers.Add("Authorization", authorizationHeader);
    }
}