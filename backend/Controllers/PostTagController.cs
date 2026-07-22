using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace backend.Controllers;

[ApiController]
[Route("api/post-tags")]
public class PostTagController : ControllerBase
{
    private readonly PostTagService _postTagService;

    public PostTagController(PostTagService postTagService)
    {
        _postTagService = postTagService;
    }

    [HttpGet("post/{postId}")]
    public async Task<IActionResult> GetTagsByPostId(Guid postId)
    {
        try
        {
            IEnumerable<Tag> tags = await _postTagService.GetTagsByPostIdAsync(postId);

            return Ok(tags);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet("tag/{tagId}")]
    public async Task<IActionResult> GetPostsByTagId(Guid tagId)
    {
        try
        {
            IEnumerable<Post> posts = await _postTagService.GetPostsByTagIdAsync(tagId);

            return Ok(posts);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPost("post/{postId}/tag/{tagId}")]
    [Authorize]
    public async Task<IActionResult> AddTagToPost(
        Guid postId,
        Guid tagId)
    {
        try
        {
            await _postTagService.AddTagToPostAsync(
                postId,
                tagId
            );

            return Ok(new
            {
                message = "Tag agregado al post correctamente"
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    [HttpDelete("post/{postId}/tag/{tagId}")]
    [Authorize]
    public async Task<IActionResult> RemoveTagFromPost(
        Guid postId,
        Guid tagId)
    {
        try
        {
            await _postTagService.RemoveTagFromPostAsync(
                postId,
                tagId
            );

            return Ok(new
            {
                message = "Tag eliminado del post correctamente"
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }
}