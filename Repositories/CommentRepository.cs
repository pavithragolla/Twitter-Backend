using Dapper;
using Microsoft.Extensions.Caching.Memory;
using task.Repositories;
using Twitter_task.Models;
using Twitter_task.utilities;

namespace Twitter_task.Controllers;

public interface ICommentRepository
{
     Task<Comment> Create(Comment Item);
    Task Delete(int Id);
    Task<List<Comment>> GetAllComment(int PostId, int Limit, int PageNumber);
    Task<Comment> GetByComments(int Id);

}
public class CommentRepository : BaseRepository, ICommentRepository
{
    private readonly IMemoryCache _memoryCache;

    public CommentRepository(IConfiguration configuration, IMemoryCache memoryCache) : base(configuration)
    {
        _memoryCache = memoryCache;

    }

    public async Task<Comment> Create(Comment Item)
    {
        var query = $@"INSERT INTO ""{TableNames.comment}"" (
	  comments, user_id, post_id) VALUES (@Comments, @UserId, @PostId) RETURNING * ";
      using (var connection = NewConnection)
        {
           return await connection.QuerySingleOrDefaultAsync<Comment>(query, Item);

        }
    }

    public async Task Delete(int Id)
    {
              var query = $@"DELETE FROM ""{TableNames.comment}"" WHERE id = @Id ";
    using (var connection = NewConnection)
        {
          await connection.QuerySingleOrDefaultAsync<Comment>(query, new{ Id } );

        }
    }

    public async Task<List<Comment>> GetAllComment(int PostId, int Limit, int PageNumber)
    {
         var Postmem = _memoryCache.Get<List<Comment>>(key: $"comment {Limit} : {PageNumber}");
        if (Postmem is null)
        {
        var query = $@"SELECT * FROM ""{TableNames.comment}"" WHERE post_id = @PostId
            ORDER BY Id OFFSET @PageNumber
	        LIMIT @Limit";
        using (var con = NewConnection)
            Postmem = (await con.QueryAsync<Comment>(query, new { PostId, @PageNumber = (PageNumber-1)*Limit,Limit })).AsList();
          _memoryCache.Set(key:"comment",Postmem, TimeSpan.FromMinutes(value:1));
        }
        return Postmem;
    }

    public async Task<Comment> GetByComments(int Id)
    {
         var query = $@"SELECT * FROM ""{TableNames.comment}"" WHERE id= @Id";
        using (var con = NewConnection)
             return await con.QuerySingleOrDefaultAsync<Comment>(query, new { Id });
    }
}