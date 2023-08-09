using Chat.Core.Interfaces;
using System.Reflection;

namespace Chat.Core.Entities;

public class FollowingRequest : IEntity
{
    public int Id { get; set; }
    public string? FromID { get; set; }
    public string? ToID { get; set; }
}
