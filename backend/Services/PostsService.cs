using backend.Helpers;
using backend.Models.Requests;
using backend.Repositories;

namespace backend.Services;

public class PostService
{
    private readonly PostRepository _postRepository;

    public PostService(PostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<IEnumerable<Post>> GetAllPostsAsync()
    {
        return await _postRepository.GetAllAsync();
    }

    public async Task<Post?> GetPostByIdAsync(Guid id)
    {
        return await _postRepository.GetByIdAsync(id);
    }

    public async Task<Post> CreateAsync(CreatePostRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException("ERROR - El titulo del post no puede ser vacio");
        }

        string title = request.Title.Trim();

        string slug = SlugHelper.Generate(title);

        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException("ERROR - El titulo no puede generar un slug valido");
        }

        bool slugExists = await _postRepository.ExistsBySlugAsync(slug);

        if (slugExists)
        {
            throw new InvalidOperationException(
                "ERROR - Un post con ese slug ya existe"
            );
        }

        DateTime now = DateHelper.Now();

        Post post = new()
        {
            Id = Guid.NewGuid(),
            Title = title,
            Slug = slug,
            Content = request.Content,
            CreatedAt = request.CreatedAt,
            PublishedAt = now,
            UpdatedAt = now
        };

        return await _postRepository.CreateAsync(post);
    }

    public async Task<Post?> UpdatePostAsync(Post post)
    {
        return await _postRepository.UpdateAsync(post);
    }

    public async Task<bool> DeletePostAsync(Guid id)
    {
        return await _postRepository.DeleteAsync(id);
    }
}