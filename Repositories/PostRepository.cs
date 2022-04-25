using Dapper;
using Microsoft.Extensions.Caching.Memory;
using task.Repositories;
using Twitter_task.Models;
using Twitter_task.utilities;

namespace Twitter_task.Repositories;

public interface IPostRepository
{
    Task<Post> Create(Post Item);
    Task Update(Post Item);
    Task Delete(int Id);
    Task<List<Post>> GetAllPost(int Limit, int PageNumber);
    Task<Post> GetById(int Id);
    Task<List<Post>> GetPostByuserId(int UserIde);
}

public class PostRepository : BaseRepository, IPostRepository
{
    private readonly IMemoryCache _memoryCache;


    public PostRepository(IConfiguration configuration, IMemoryCache memoryCache) : base(configuration)
    {
        _memoryCache = memoryCache;

    }

    public async Task<Post> Create(Post Item)
    {

        var query = $@"INSERT INTO {TableNames.post} (title, user_id) VALUES(@Title, @UserId) ";
         using (var connection = NewConnection)
        {
           return await connection.QuerySingleOrDefaultAsync<Post>(query, Item);

        }
    }

    public async Task Delete(int Id)
    {
         var query = $@"DELETE FROM {TableNames.post} WHERE id = @Id ";
    using (var connection = NewConnection)
        {
          await connection.QuerySingleOrDefaultAsync<Post>(query, new{ Id } );

        }
    }

    public async Task<List<Post>> GetAllPost(int Limit, int PageNumber)
    {
        var Postmem = _memoryCache.Get<List<Post>>(key: $"post {Limit} : {PageNumber}");
        if (Postmem is null)
        {
          var query = $@"SELECT * FROM {TableNames.post} ORDER BY Id OFFSET @PageNumber
	LIMIT @Limit";

        using (var con = NewConnection)
           Postmem = (await con.QueryAsync<Post>(query, new {@PageNumber = (PageNumber-1)*Limit,Limit})).AsList();
          _memoryCache.Set(key:"post",Postmem, TimeSpan.FromMinutes(value:1));
        }
        return Postmem;
    }

    public async Task<Post> GetById(int Id)
    {
        var query = $@"SELECT * FROM {TableNames.post} WHERE id= @Id";
          using (var con = NewConnection)
             return await con.QuerySingleOrDefaultAsync<Post>(query, new { Id });
    }

    public async Task<List<Post>> GetPostByuserId(int UserIde)
    {
       var query = $@" SELECT * FROM {TableNames.post} WHERE user_id = @UserId";

        using(var con = NewConnection)
        return (await con.QueryAsync<Post>(query, new{ UserId = UserIde})).AsList();
    }

    public async Task Update(Post Item)
    {
        var query = $@"UPDATE {TableNames.post} SET title = @Title, updated_at = now() WHERE id = @Id";

         using (var connection = NewConnection)

          await connection.ExecuteAsync(query, Item);
    }


}