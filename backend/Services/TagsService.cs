using backend.Repositories;

namespace backend.Services;

public class TagService
{
    private readonly TagRepository _tagRepository;

    public TagService(TagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<IEnumerable<Tag>> GetAllTagsAsync()
    {
        return await _tagRepository.GetAllAsync();
    }

    public async Task<Tag?> GetTagByIdAsync(Guid id)
    {
        return await _tagRepository.GetByIdAsync(id);
    }

    public async Task<Tag?> GetTagByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "ERROR - El nombre del tag no puede ser vacio"
            );
        }

        string tagName = name.Trim();

        return await _tagRepository.GetByNameAsync(tagName);
    }

    public async Task<Tag> CreateAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "ERROR - El nombre del tag no puede ser vacio"
            );
        }

        string tagName = name.Trim();

        Tag? existingTag = await _tagRepository.GetByNameAsync(tagName);

        if (existingTag is not null)
        {
            throw new InvalidOperationException(
                "ERROR - Un tag con ese nombre ya existe"
            );
        }

        Tag tag = new()
        {
            Id = Guid.NewGuid(),
            Name = tagName
        };

        Tag? createdTag = await _tagRepository.CreateAsync(tag);

        if (createdTag is null)
        {
            throw new InvalidOperationException(
                "ERROR - No se pudo crear el tag"
            );
        }

        return createdTag;
    }

    public async Task<bool> DeleteTagAsync(Guid id)
    {
        Tag? tag = await _tagRepository.GetByIdAsync(id);

        if (tag is null)
        {
            return false;
        }

        return await _tagRepository.DeleteAsync(id);
    }
}