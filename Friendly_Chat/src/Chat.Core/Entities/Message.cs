using Chat.Core.Interfaces;

namespace Chat.Core.Entities;

public class Message : IEntity
{
    public int Id { get; set; }
    public string? FromUserID { get; set; }
    public string? ToUserID { get; set; }
    public string? Content { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    public int IsRead { get; set; } = 0;
    //IsRead = 1 oxunub
    //IsRead =0 oxunmuyub
}
