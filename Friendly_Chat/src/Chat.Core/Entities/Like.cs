using Chat.Core.Interfaces;

namespace Chat.Core.Entities;

public class Like:IEntity
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string? LikedUserId { get; set; }
}
