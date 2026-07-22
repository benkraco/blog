using backend.Models.Requests;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/posts")]
public class PostController : ControllerBase
{
    private readonly PostService _postService;

    public PostController(PostService postService)
    {
        _postService = postService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPosts()
    {
        var posts = await _postService.GetAllPostsAsync();

        return Ok(posts);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPostById(Guid id)
    {
        var post = await _postService.GetPostByIdAsync(id);

        if (post == null)
        {
            return NotFound();
        }

        return Ok(post);
    }

    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetPostBySlug(string slug)
    {
        var post = await _postService.GetPostBySlugAsync(slug);

        if (post == null)
        {
            return NotFound();
        }

        return Ok(post);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreatePost(
        [FromBody] CreatePostRequest request)
    {
        try
        {
            var post = await _postService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetPostById),
                new { id = post.Id },
                post
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
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

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdatePost(
        [FromBody] Post post)
    {
        var updatedPost = await _postService.UpdatePostAsync(post);

        if (updatedPost == null)
        {
            return NotFound();
        }

        return Ok(updatedPost);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeletePost(Guid id)
    {
        bool deleted = await _postService.DeletePostAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}