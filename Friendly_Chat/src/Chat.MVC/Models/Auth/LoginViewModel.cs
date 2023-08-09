using System.ComponentModel.DataAnnotations;

namespace Chat.MVC.Models.Auth;

public class LoginViewModel
{
    [Required, MaxLength(50)]
    public string? UserNameOrEmail { get; set; } = null!;
    


    [Required, DataType(DataType.Password), MinLength(8)]
    public string? Password { get; set; }
    public bool RememberMe { get; set; }
}
