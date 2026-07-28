using backend.Repositories;
using backend.Services.Storage;
using ImageMagick;

namespace backend.Services;

public class ImageService
{
    private readonly ImageRepository _imageRepository;
    private readonly IStorageService _storageService;

    public ImageService(
        ImageRepository imageRepository,
        IStorageService storageService)
    {
        _imageRepository = imageRepository;
        _storageService = storageService;
    }

    public async Task<Image> UploadAsync(
        Guid postId,
        IFormFile file,
        string alt,
        string description,
        int displayOrder)
    {
        if (file is null || file.Length == 0)
        {
            throw new ArgumentException(
                "ERROR - El archivo de imagen es inválido."
            );
        }

        if (!file.ContentType.StartsWith("image/"))
        {
            throw new ArgumentException(
                "ERROR - El archivo debe ser una imagen."
            );
        }

        var imageId = Guid.NewGuid();

        await using var inputStream = file.OpenReadStream();

        using var image = new MagickImage(inputStream);

        int width = (int)image.Width;
        int height = (int)image.Height;

        // Convertir a WebP
        image.Format = MagickFormat.WebP;

        // Calidad de compresión
        image.Quality = 80;

        await using var outputStream = new MemoryStream();

        image.Write(outputStream);

        outputStream.Position = 0;

        string fileName =
            $"posts/{postId}/{imageId}.webp";

        // Subir imagen a R2 y obtener URL pública
        var publicUrl = await _storageService.UploadAsync(
            outputStream,
            fileName,
            "image/webp"
        );

        var imageEntity = new Image
        {
            Id = imageId,
            PostId = postId,
            Url = publicUrl,
            Alt = alt,
            DisplayOrder = displayOrder,
            Name = Path.GetFileNameWithoutExtension(file.FileName) + ".webp",
            Description = description,
            TakenAt = DateTime.UtcNow,
            Width = width,
            Height = height,
            FileSize = outputStream.Length,
            MimeType = "image/webp"
        };

        var createdImage =
            await _imageRepository.CreateAsync(imageEntity);

        if (createdImage is null)
        {
            // Si falla PostgreSQL después de subir a R2,
            // eliminamos la imagen de R2 para no dejar basura.
            await _storageService.DeleteAsync(fileName);

            throw new InvalidOperationException(
                "ERROR - No se pudo guardar la imagen en la base de datos."
            );
        }

        return createdImage;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var image = await _imageRepository.GetByIdAsync(id);

        if (image is null)
        {
            return false;
        }

        await _storageService.DeleteAsync(image.Url);

        await _imageRepository.DeleteAsync(id);

        return true;
    }
}