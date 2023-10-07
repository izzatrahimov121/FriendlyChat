using Chat.Core.Entities;
using Chat.MVC.HelperServices;
using Chat.MVC.Models.Home;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Chat.MVC.Hubs;

public class MyHub : Hub
{
    private readonly OnlineUsersService _onlineUsersService;
    private readonly UserManager<AppUser> _userManager;

    public MyHub(OnlineUsersService onlineUsersService, UserManager<AppUser> userManager)
    {
        _onlineUsersService = onlineUsersService;
        _userManager = userManager;
    }

    public override async Task OnConnectedAsync()
    {
        var user = await _userManager.GetUserAsync(Context.User);
        _onlineUsersService.AddUser(user.UserName);
        user.IsOnline = 1;
        user.ConnectionId = Context.ConnectionId.ToString();
        await _userManager.UpdateAsync(user);
        await Clients.All.SendAsync("UserConnected", user.UserName);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = await _userManager.GetUserAsync(Context.User);
        _onlineUsersService.RemoveUser(user.UserName);
        user.IsOnline= 0;
        await _userManager.UpdateAsync(user);
        await Clients.All.SendAsync("UserDisconnected", user.UserName);
    }

    //public async Task SendMessageAsync(string message,string userName)
    //{
    //    var user = await _userManager.FindByNameAsync(userName);
    //    if (user is not null)
    //    {
    //        var messageModel = new GetMessageViewModel();
    //        messageModel.Message = message;
    //        messageModel.time = DateTime.Now.ToString("HH:mm");
    //        await Clients.Client(user.ConnectionId).SendAsync("receiveMessage", messageModel);
    //    }
    //}
}
