using Microsoft.AspNetCore.Identity;

namespace Chat.Core.Entities;

public class AppUser : IdentityUser
{
	public string Image { get; set; } = "default.png";
}
