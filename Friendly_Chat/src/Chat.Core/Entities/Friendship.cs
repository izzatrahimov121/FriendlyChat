using Chat.Core.Interfaces;

namespace Chat.Core.Entities;

public class Friendship : IEntity
{
	public int Id { get; set; }
	public string? UserId { get; set; }

	public AppUser? User { get; set; }
}
