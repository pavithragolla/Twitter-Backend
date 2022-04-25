using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Twitter_task.DTOs;
using Twitter_task.Models;
using Twitter_task.Repositories;
using Twitter_task.utilities;

namespace Twitter_task.Controllers;

[ApiController]
[Route("api/post")]
[Authorize]

public class PostController : ControllerBase
{
    private readonly ILogger<PostController> _logger;
    private readonly IPostRepository _post;

    public PostController(ILogger<PostController> logger, IPostRepository post)
    {
        _logger = logger;
        _post = post;
    }

    private int GetuserIdFromClaims(IEnumerable<Claim> claims)
    {
        return Convert.ToInt32(claims.Where(x => x.Type == UserConstants.Id).First().Value);
    }


    [HttpPost]

    public async Task<ActionResult<Post>> Create([FromBody] PostCreateDTO Data)
    {
        var UserIde = GetuserIdFromClaims(User.Claims);

        var postCount = (await _post.GetPostByuserId(UserIde)).Count;
        if (postCount >= 5)
        return BadRequest("You can Create only 5 posts");

        var CreatePost = new Post
        {
            Title = Data.Title.Trim(),
            UserId = UserIde,
        };
        var created = await _post.Create(CreatePost);

        return Ok(created);
    }

    [HttpPut("{post_id}")]

    public async Task<ActionResult> Update([FromRoute] int post_id, [FromBody] PostUpdateDTO Data)
    {
        var UserIde = GetuserIdFromClaims(User.Claims);

        var existing = await _post.GetById(post_id);
        if (existing is null)
            return NotFound();

        if (existing.UserId != UserIde)
            return StatusCode(403, "You cannot update other's todo");

        var toUpdatePost = existing with
        {
            Title = Data.Title is null ? existing.Title : Data.Title.Trim(),
            // UpdatedAt = Data.UpdatedAt,
        };
        await _post.Update(toUpdatePost);
        return NoContent();
    }
    [HttpDelete("{Id}")]

         public async Task<ActionResult> DeleteTodo([FromRoute] int Id)
         {

        var UserIde = GetuserIdFromClaims(User.Claims);

      var existing = await _post.GetById(Id);
        // // var currentUserId = GetCurrentUserId();
        // if (Int32.Parse(currentUserId) != existing.UserId)
        //     return Unauthorized("Your are not Authorized");


        if (existing is null)
            return NotFound();

              if (existing.UserId != UserIde)
            return StatusCode (403,"You cannot delete other's todo");


        await _post.Delete(Id);
        return NoContent();
    }


     [HttpGet]

      public async Task<ActionResult<List<Post>>> GetAllTodos([FromQuery] int Limit, int PageNumber)
      {


          var allTodos = await _post.GetAllPost(Limit, PageNumber);

          return Ok(allTodos);
      }



       [HttpGet("{Id}")]

      public async Task<ActionResult<Post>> GetByPostId(int Id)
      {
          var allTodos = await _post.GetById(Id);

          return Ok(allTodos);
      }
}