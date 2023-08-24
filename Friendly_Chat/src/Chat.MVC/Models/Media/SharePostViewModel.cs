using System.ComponentModel.DataAnnotations;

namespace Chat.MVC.Models.Media;

public class SharePostViewModel
{
    [Required(ErrorMessage = "Do not leave blank")]
    public IFormFile File { get; set; }
    public string? Description { get; set; }
}
