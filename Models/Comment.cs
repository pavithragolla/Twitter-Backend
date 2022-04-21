namespace Twitter_task.Models;

public record Comment
{
     public int Id { get; set; }
    public string Comments { get; set; }
    public int PostId { get; set; }
    public int UserId { get; set; }
}