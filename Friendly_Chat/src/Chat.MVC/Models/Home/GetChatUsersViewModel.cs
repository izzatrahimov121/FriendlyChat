namespace Chat.MVC.Models.Home;

public class GetChatUsersViewModel
{
    public string? ToUserName { get; set; }
    public string? ToUserImage { get; set; }
    public string? LastMessage { get; set; }
    public int? NewMessagesCount { get; set; } = null;
    public bool? IsOnline { get; set; } = false;
}
