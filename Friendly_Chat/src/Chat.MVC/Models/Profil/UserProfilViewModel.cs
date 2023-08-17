using System.ComponentModel.DataAnnotations;

namespace Chat.MVC.Models.Profil;

public class UserProfilViewModel
{
    [Required(ErrorMessage = "Do not leave blank"), MaxLength(50, ErrorMessage = "Length exceeds 50")]
    public string? Username { get; set; }

    public string? Photo { get; set; }
    public IFormFile? Image { get; set; }


    [Required(ErrorMessage = "Do not leave blank"), DataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid email"), MaxLength(255)]
    public string? Email { get; set; }

    public int? Followers { get; set; }
    public int? Following { get; set; }
}
