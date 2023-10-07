using Microsoft.AspNetCore.Identity;

namespace Chat.Core.Entities;

public class AppUser : IdentityUser
{
	public string Image { get; set; } = "default.png";
	public int IsOnline { get; set; } //0 not online  //1 online
	public string? ConnectionId { get; set; } //servere her qoşulduğunda ona verilen id 
}
