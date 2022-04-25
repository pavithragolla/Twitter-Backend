using Dapper;
using Microsoft.Extensions.Caching.Memory;
using task.Repositories;
using Twitter_task.Models;
using Twitter_task.utilities;

namespace Twitter_task.Repositories;

public interface IUserRepository
{
    Task<User> Create(User Item);
    Task Update(User Item);
    Task<User> GetByEmail(string Email);
    Task<User> GetById(int Id);
}

public class UserRepository : BaseRepository, IUserRepository
{
    private readonly IMemoryCache _memoryCache;

    public UserRepository(IConfiguration configuration, IMemoryCache memoryCache) : base(configuration)
    {
        _memoryCache = memoryCache;
    }

    public async Task<User> Create(User Item)
    {
        var query = $@"INSERT INTO ""{TableNames.user}"" (name, email, password) VALUES (@Name, @Email, @Password)";
         using (var connection = NewConnection)
        {
           return await connection.QuerySingleOrDefaultAsync<User>(query, Item);

        }
    }

    public async Task<User> GetByEmail(string Email)
    {
        var Postmem = _memoryCache.Get<User>(key: $"user {Email}");
        if (Postmem is null)
        {
        var query = $@"SELECT * FROM ""{TableNames.user}"" WHERE email = @Email";
         using (var con = NewConnection)
           Postmem = await con.QuerySingleOrDefaultAsync<User>(query, new { Email });
          _memoryCache.Set(key:"user",Postmem, TimeSpan.FromMinutes(value:1));
        }
        return Postmem;
    }

    public async Task<User> GetById(int Id)
    {
        var query = $@"SELECT * FROM""{TableNames.user}"" WHERE id= @Id";
         using (var con = NewConnection)
             return await con.QuerySingleOrDefaultAsync<User>(query, new { Id });
        }


    public async Task Update(User Item)
    {
        var query = $@"UPDATE ""{TableNames.user}"" SET name = @Name WHERE id = @Id";

        using (var connection = NewConnection)

          await connection.ExecuteAsync(query, Item);
    }
}
