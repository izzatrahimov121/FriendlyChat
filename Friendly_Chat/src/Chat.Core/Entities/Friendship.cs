using Chat.Core.Interfaces;

namespace Chat.Core.Entities;

public class Friendship : IEntity
{
	public int Id { get; set; }
	public string UserID { get; set; }//kim
	public string FollowedID { get; set; }//kimi
	public DateTime Date { get; set; } = DateTime.Now;
}
