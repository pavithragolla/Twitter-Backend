namespace Twitter_task.Models;

public record Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public int UserId { get; set; }
}