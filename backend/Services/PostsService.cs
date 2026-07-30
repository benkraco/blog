using backend.Helpers;
using backend.Models.Requests;
using backend.Repositories;

namespace backend.Services;

public class PostService
{
    private readonly PostRepository _postRepository;
    private readonly ImageService _imageService;
    private readonly ImageRepository _imageRepository;

    public PostService(
        PostRepository postRepository,
        ImageService imageService,
        ImageRepository imageRepository)
    {
        _postRepository = postRepository;
        _imageService = imageService;
        _imageRepository = imageRepository;
    }
    public async Task<IEnumerable<Post>> GetAllPostsAsync()
    {
        return await _postRepository.GetAllAsync();
    }

    public async Task<Post?> GetPostByIdAsync(Guid id)
    {
        return await _postRepository.GetByIdAsync(id);
    }

    public async Task<Post?> GetPostBySlugAsync(string slug)
    {
        var post = await _postRepository.GetBySlugAsync(slug);

        if (post is null)
        {
            return null;
        }

        var images = await _imageRepository.GetByPostIdAsync(post.Id);

        post.Images = images.ToList();

        return post;
    }

    public async Task<Post> CreateAsync(CreatePostRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException(
                "ERROR - El titulo del post no puede ser vacio"
            );
        }

        if (request.MarkdownFile is null)
        {
            throw new ArgumentException(
                "ERROR - El archivo Markdown es obligatorio"
            );
        }

        if (!Path.GetExtension(request.MarkdownFile.FileName)
            .Equals(".md", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "ERROR - El archivo debe ser un Markdown (.md)"
            );
        }

        string title = request.Title.Trim();

        string slug = SlugHelper.Generate(title);

        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException(
                "ERROR - El titulo no puede generar un slug valido"
            );
        }

        bool slugExists =
            await _postRepository.ExistsBySlugAsync(slug);

        if (slugExists)
        {
            throw new InvalidOperationException(
                "ERROR - Un post con ese slug ya existe"
            );
        }

        string content;

        using (var reader = new StreamReader(
            request.MarkdownFile.OpenReadStream()))
        {
            content = await reader.ReadToEndAsync();
        }

        DateTime now = DateHelper.Now();

        Post post = new()
        {
            Id = Guid.NewGuid(),
            Title = title,
            Slug = slug,
            Content = content,
            CreatedAt = request.CreatedAt,
            PublishedAt = now,
            UpdatedAt = now
        };

        var createdPost = await _postRepository.CreateAsync(post);

        for (int i = 0; i < request.Images.Count; i++)
        {
            await _imageService.UploadAsync(
                createdPost.Id,
                request.Images[i],
                request.Images[i].FileName,
                string.Empty,
                i
            );
        }

        return createdPost;
    }

    public async Task<Post?> UpdatePostAsync(
        Guid id,
        UpdatePostRequest request)
    {
        var post = await _postRepository.GetByIdAsync(id);

        if (post is null)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException(
                "ERROR - El titulo del post no puede ser vacio"
            );
        }

        string title = request.Title.Trim();

        string slug = SlugHelper.Generate(title);

        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException(
                "ERROR - El titulo no puede generar un slug valido"
            );
        }

        bool slugExists =
            await _postRepository.ExistsBySlugExceptIdAsync(
                slug,
                id
            );

        if (slugExists)
        {
            throw new InvalidOperationException(
                "ERROR - Un post con ese slug ya existe"
            );
        }

        string content = post.Content;

        if (request.MarkdownFile is not null)
        {
            if (!Path.GetExtension(request.MarkdownFile.FileName)
                .Equals(".md", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "ERROR - El archivo debe ser un Markdown (.md)"
                );
            }

            using var reader = new StreamReader(
                request.MarkdownFile.OpenReadStream()
            );

            content = await reader.ReadToEndAsync();
        }

        DateTime now = DateHelper.Now();

        post.Title = title;
        post.Slug = slug;
        post.Content = content;
        post.UpdatedAt = now;

        var updatedPost =
            await _postRepository.UpdateAsync(post);

        if (updatedPost is null)
        {
            return null;
        }

        foreach (var imageId in request.DeletedImageIds)
        {
            await _imageService.DeleteAsync(imageId);
        }

        var existingImages =
            await _imageRepository.GetByPostIdAsync(id);

        var existingImagesDictionary =
            existingImages.ToDictionary(
                image => image.Id
            );

        var uploadedImages = new List<Image>();

        foreach (var image in request.Images)
        {
            var uploadedImage =
                await _imageService.UploadAsync(
                    id,
                    image,
                    image.FileName,
                    string.Empty,
                    0
                );

            uploadedImages.Add(uploadedImage);
        }

        for (int i = 0; i < request.ImageOrder.Count; i++)
        {
            string imageReference =
                request.ImageOrder[i];

            if (imageReference.StartsWith(
                "existing:",
                StringComparison.OrdinalIgnoreCase))
            {
                string idString =
                    imageReference["existing:".Length..];

                if (!Guid.TryParse(
                    idString,
                    out Guid imageId))
                {
                    throw new ArgumentException(
                        "ERROR - El orden contiene un ID de imagen existente inválido."
                    );
                }

                if (!existingImagesDictionary.ContainsKey(
                    imageId))
                {
                    continue;
                }

                await _imageRepository.UpdateDisplayOrderAsync(
                    imageId,
                    i
                );
            }
            else if (imageReference.StartsWith(
                "new:",
                StringComparison.OrdinalIgnoreCase))
            {
                string indexString =
                    imageReference["new:".Length..];

                if (!int.TryParse(
                    indexString,
                    out int newImageIndex))
                {
                    throw new ArgumentException(
                        "ERROR - El orden contiene una referencia de imagen nueva inválida."
                    );
                }

                if (newImageIndex < 0 ||
                    newImageIndex >= uploadedImages.Count)
                {
                    throw new ArgumentException(
                        "ERROR - El orden contiene una imagen nueva inexistente."
                    );
                }

                var uploadedImage =
                    uploadedImages[newImageIndex];

                await _imageRepository.UpdateDisplayOrderAsync(
                    uploadedImage.Id,
                    i
                );
            }
        }

        var finalImages =
            await _imageRepository.GetByPostIdAsync(id);

        updatedPost.Images =
            finalImages.ToList();

        return updatedPost;
    }

    public async Task<bool> DeletePostAsync(Guid id)
    {
        var post = await _postRepository.GetByIdAsync(id);

        if (post is null)
        {
            return false;
        }

        var images = await _imageRepository.GetByPostIdAsync(id);

        foreach (var image in images)
        {
            await _imageService.DeleteAsync(image.Id);
        }

        return await _postRepository.DeleteAsync(id);
    }
}