namespace Twitter_task.Models;

public record User
{
    public int Id { get; set; }
    public String Name { get; set; }
    public String Email { get; set; }
    public String Password { get; set; }
}