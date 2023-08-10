using Chat.Core.Interfaces;

namespace Chat.Core.Entities;

public class FollowingRequest : IEntity
{
    public int Id { get; set; }
    public string? FromID { get; set; }
    public string? ToID { get; set; }
    public DateTime? Date { get; set; } = DateTime.Now;
    public int? Status { get; set; } = 0;
    //status=0 userin yeni oxunmamış bildirishi var
    //satus=1 userin oxunmamish bildirişi yoxdur
}
