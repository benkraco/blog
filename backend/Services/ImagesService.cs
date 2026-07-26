using backend.Repositories;

namespace backend.Services;

public class ImageService
{
    private readonly ImageRepository _imageRepository;

    public ImageService(ImageRepository imageRepository)
    {
        _imageRepository = imageRepository;
    }

    public async Task<IEnumerable<Image>> GetImagesByPostIdAsync(Guid postId)
    {
        return await _imageRepository.GetByPostIdAsync(postId);
    }

    public async Task<Image?> GetImageByIdAsync(Guid id)
    {
        return await _imageRepository.GetByIdAsync(id);
    }

    public async Task<Image?> CreateAsync(Image image)
    {
        if (image.PostId == Guid.Empty)
        {
            throw new ArgumentException("El post de la imagen es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(image.Url))
        {
            throw new ArgumentException("La URL de la imagen es obligatoria.");
        }

        if (string.IsNullOrWhiteSpace(image.Name))
        {
            throw new ArgumentException("El nombre de la imagen es obligatorio.");
        }

        if (image.DisplayOrder < 0)
        {
            throw new ArgumentException("El orden de la imagen no puede ser negativo.");
        }

        if (image.Width <= 0)
        {
            throw new ArgumentException("El ancho de la imagen debe ser mayor a 0.");
        }

        if (image.Height <= 0)
        {
            throw new ArgumentException("El alto de la imagen debe ser mayor a 0.");
        }

        if (image.FileSize <= 0)
        {
            throw new ArgumentException("El tamaño de la imagen debe ser mayor a 0.");
        }

        if (string.IsNullOrWhiteSpace(image.MimeType))
        {
            throw new ArgumentException("El tipo MIME de la imagen es obligatorio.");
        }

        return await _imageRepository.CreateAsync(image);
    }

    public async Task<bool> DeleteImageAsync(Guid id)
    {
        return await _imageRepository.DeleteAsync(id);
    }
}