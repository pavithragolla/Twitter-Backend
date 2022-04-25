using System.Text.Json.Serialization;

namespace Twitter_task.DTOs;

public record PostDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public int UserId { get; set; }
}
public record PostCreateDTO
{
      [JsonPropertyName("title")]
    public string Title { get; set; }
    // public int UserId { get; set; }
}
public record PostUpdateDTO
{
      [JsonPropertyName("title")]
    public string Title { get; set; }

    // public DateTimeOffset UpdatedAt { get; set; }
}