using backend.Repositories;

namespace backend.Services;

public class PostTagService
{
    private readonly PostTagRepository _postTagRepository;
    private readonly PostRepository _postRepository;
    private readonly TagRepository _tagRepository;

    public PostTagService(
        PostTagRepository postTagRepository,
        PostRepository postRepository,
        TagRepository tagRepository)
    {
        _postTagRepository = postTagRepository;
        _postRepository = postRepository;
        _tagRepository = tagRepository;
    }

    public async Task<IEnumerable<Tag>> GetTagsByPostIdAsync(Guid postId)
    {
        Post? post = await _postRepository.GetByIdAsync(postId);

        if (post == null)
        {
            throw new KeyNotFoundException(
                "ERROR - El post no existe"
            );
        }

        return await _postTagRepository.GetTagsByPostIdAsync(postId);
    }

    public async Task<IEnumerable<Post>> GetPostsByTagIdAsync(Guid tagId)
    {
        Tag? tag = await _tagRepository.GetByIdAsync(tagId);

        if (tag == null)
        {
            throw new KeyNotFoundException(
                "ERROR - El tag no existe"
            );
        }

        return await _postTagRepository.GetPostsByTagIdAsync(tagId);
    }

    public async Task<bool> AddTagToPostAsync(Guid postId, Guid tagId)
    {
        Post? post = await _postRepository.GetByIdAsync(postId);

        if (post == null)
        {
            throw new KeyNotFoundException(
                "ERROR - El post no existe"
            );
        }

        Tag? tag = await _tagRepository.GetByIdAsync(tagId);

        if (tag == null)
        {
            throw new KeyNotFoundException(
                "ERROR - El tag no existe"
            );
        }

        bool relationExists = await _postTagRepository.ExistsAsync(
            postId,
            tagId
        );

        if (relationExists)
        {
            throw new InvalidOperationException(
                "ERROR - El tag ya está asociado al post"
            );
        }

        return await _postTagRepository.AddAsync(
            postId,
            tagId
        );
    }

    public async Task<bool> RemoveTagFromPostAsync(Guid postId, Guid tagId)
    {
        bool relationExists = await _postTagRepository.ExistsAsync(
            postId,
            tagId
        );

        if (!relationExists)
        {
            throw new KeyNotFoundException(
                "ERROR - El tag no está asociado al post"
            );
        }

        return await _postTagRepository.RemoveAsync(
            postId,
            tagId
        );
    }
}