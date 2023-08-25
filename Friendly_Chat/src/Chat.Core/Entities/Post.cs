using Chat.Core.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Chat.Core.Entities;

public class Post : IEntity
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public string? PostType { get; set; } //image or video
    public string? PostName { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    [MaxLength(1500)]
    public string? Description { get; set; } = string.Empty;
}
