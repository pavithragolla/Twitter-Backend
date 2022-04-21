using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Twitter_task.DTOs;
using Twitter_task.Models;
using Twitter_task.utilities;

namespace Twitter_task.Controllers;

[ApiController]
[Route("api/comment")]
[Authorize]

public class CommentController : ControllerBase
{
    private readonly ILogger<CommentController> _logger;
    private readonly ICommentRepository _comment;

    public object TodoConstants { get; private set; }

    public CommentController(ILogger<CommentController> logger, ICommentRepository comment)
    {
        _logger = logger;
        _comment = comment;
    }
     private int GetuserIdFromClaims(IEnumerable<Claim> claims)
    {
      return Convert.ToInt32 (claims.Where(x => x.Type == UserConstants.Id).First().Value);

    }

    [HttpPost]

    public async Task<ActionResult<Comment>> Create( [FromQuery] int post_id, [FromBody] CommentCreateDTO Data)
    {
        var UserIde = GetuserIdFromClaims(User.Claims);
        // var PostIde = GetuserIdFromClaims(User.Claims);

         var CreateItem = new Comment
         {
             Comments = Data.Comments.Trim(),
             UserId = UserIde,
             PostId = post_id


         };
          var createdItem = await _comment.Create(CreateItem);
          return Ok(createdItem);
    }

    [HttpDelete("{Id}")]

         public async Task<ActionResult> Delete([FromRoute] int Id)
         {

        var UserIde = GetuserIdFromClaims(User.Claims);

      var existing = await _comment.GetByComments(Id);
        // // var currentUserId = GetCurrentUserId();
        // if (Int32.Parse(currentUserId) != existing.UserId)
        //     return Unauthorized("Your are not Authorized");


        if (existing is null)
            return NotFound();

              if (existing.UserId != UserIde)
            return StatusCode (403,"You cannot delete other's todo");


        await _comment.Delete(Id);
        return NoContent();
    }

     [HttpGet]

      public async Task<ActionResult<List<Comment>>> GetAllComments([FromQuery] int post_id)
      {
          var allTodos = await _comment.GetAllComment(post_id);

          return Ok(allTodos);
      }
}