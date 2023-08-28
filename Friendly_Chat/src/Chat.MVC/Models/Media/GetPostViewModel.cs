namespace Chat.MVC.Models.Media;

public class GetPostViewModel
{
    public string? PostType { get; set; } // video or image
    public string? PostName { get; set; } 
    public string? UserName { get; set; }
    public string? UserPhoto { get; set; }
    public string? Date { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? Description { get; set; } = string.Empty;
}
