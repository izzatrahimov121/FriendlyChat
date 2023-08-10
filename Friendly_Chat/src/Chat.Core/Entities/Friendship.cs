using Chat.Core.Interfaces;

namespace Chat.Core.Entities;

public class Friendship : IEntity
{
	public int Id { get; set; }
	public string UserID { get; set; }
	public string FollowedID { get; set; }
	public DateTime Date { get; set; } = DateTime.Now;
}
