using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagController : ControllerBase
{
    private readonly TagService _tagService;

    public TagController(TagService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tags = await _tagService.GetAllTagsAsync();

        return Ok(tags);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var tag = await _tagService.GetTagByIdAsync(id);

        if (tag is null)
        {
            return NotFound();
        }

        return Ok(tag);
    }

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        var tag = await _tagService.GetTagByNameAsync(name);

        if (tag is null)
        {
            return NotFound();
        }

        return Ok(tag);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] string name)
    {
        try
        {
            var tag = await _tagService.CreateAsync(name);

            return CreatedAtAction(
                nameof(GetById),
                new { id = tag.Id },
                tag
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _tagService.DeleteTagAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}