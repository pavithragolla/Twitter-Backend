using System.Text.Json.Serialization;

namespace Twitter_task.DTOs;

public record CommentDTO
{
     [JsonPropertyName("id")]
     public int Id { get; set; }
      [JsonPropertyName("comment")]
    public string Comments { get; set; }
     [JsonPropertyName("post_id")]

    public int PostId { get; set; }
     [JsonPropertyName("user_id")]

    public int UserId { get; set; }
     [JsonPropertyName("created_at")]

    public DateTimeOffset CreatedAt { get; set; }
}

public record CommentCreateDTO
{
      [JsonPropertyName("comment")]
    public string Comments { get; set; }
    //  [JsonPropertyName("post_id")]
    // public int PostId { get; set; }
    //  [JsonPropertyName("user_id")]
    // public int UserId { get; set; }
}