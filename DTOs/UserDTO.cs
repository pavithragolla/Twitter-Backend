using System.Text.Json.Serialization;

namespace Twitter_task.DTOs;

public record UserDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public String Name { get; set; }

    [JsonPropertyName("email")]
    public String Email { get; set; }

    [JsonPropertyName("password")]
    public String Password { get; set; }
}
public record UserLoginDTO
{
    [JsonPropertyName("email")]
    public String Email { get; set; }

    [JsonPropertyName("password")]
    public String Password { get; set; }
}
public record UserLoginResDTO
{
    [JsonPropertyName("email")]
    public String Email { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

     [JsonPropertyName("token")]
    public string token { get; set; }

     [JsonPropertyName("name")]
    public string Name { get; set; }
}
public record UserUpdateDTO
{
   [JsonPropertyName("name")]
    public string Name { get; set; }
}
public record UserCreateDTO
{
    public String Name { get; set; }
    public String Email { get; set; }
    public String Password { get; set; }
}