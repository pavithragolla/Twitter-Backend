using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Twitter_task.DTOs;
using Twitter_task.Models;
using Twitter_task.Repositories;
using Twitter_task.utilities;

namespace Twitter_task.Controllers;

[ApiController]
[Route("api/user")]

public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserRepository _user;
    private IConfiguration _config;


    public UserController(ILogger<UserController> logger, IUserRepository user, IConfiguration config)
    {
        _logger = logger;
        _user = user;
        _config = config;
    }

    private int GetuserIdFromClaims(IEnumerable<Claim> claims)
    {
        return Convert.ToInt32(claims.Where(x => x.Type == UserConstants.Id).First().Value);
    }

    private bool IsValidEmailAddress(string email)
    {
        try
        {
            var emailChecked = new System.Net.Mail.MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private string Generate(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
       {
            new Claim(UserConstants.Id, user.Id.ToString()),
            new Claim(UserConstants.Name, user.Name.ToString()),
            new Claim(UserConstants.Email, user.Email),
        };

        var token = new JwtSecurityToken(_config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    [HttpPost("register")]

    public async Task<ActionResult<User>> Create([FromBody] UserCreateDTO Data)
    {
        //  var UserIde = GetuserIdFromClaims(User.Claims);

        var CreateUser = new User
        {
            Name = Data.Name,
            Email = Data.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(Data.Password)
        };
        var createdItem = await _user.Create(CreateUser);
        return Ok(createdItem);
    }

    [HttpPost("Login")]
    public async Task<ActionResult<UserLoginResDTO>> Login([FromBody] UserLoginDTO Data)
    {

        if(!IsValidEmailAddress(Data.Email))
        return BadRequest("Incorrect Email");

        var existing = await _user.GetByEmail(Data.Email);


        if (existing is null)
            return NotFound();

        if (!BCrypt.Net.BCrypt.Verify(Data.Password, existing.Password))
            return Unauthorized("Incorrect password");






        var token = Generate(existing);

        var res = new UserLoginResDTO
        {
            Id = existing.Id,
            Name = existing.Name,
            Email = existing.Email,
            token = token,

        };
        return Ok(res);
    }


    [HttpPut("Id")]
    [Authorize]

    public async Task<ActionResult> Update([FromBody] UserUpdateDTO Data)
    {
        var UserIde = GetuserIdFromClaims(User.Claims);

        var existing = await _user.GetById(UserIde);

        if (existing is null)
            return NotFound();

        if (existing.Id != UserIde)
            return StatusCode(403, "You cannot update other's name");

        var toUpdate = existing with
        {
            Name = Data.Name is null ? existing.Name : Data.Name.Trim(),
        };
        await _user.Update(toUpdate);
        return NoContent();
    }
}