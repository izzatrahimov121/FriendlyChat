using Chat.Core.Entities;
using Chat.MVC.HelperServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Chat.MVC.Hubs;

public class OnlineUsersHub : Hub
{
    private readonly OnlineUsersService _onlineUsersService;
    private readonly UserManager<AppUser> _userManager;

    public OnlineUsersHub(OnlineUsersService onlineUsersService, UserManager<AppUser> userManager)
    {
        _onlineUsersService = onlineUsersService;
        _userManager = userManager;
    }

    public override async Task OnConnectedAsync()
    {
        var user = await _userManager.GetUserAsync(Context.User);
        _onlineUsersService.AddUser(user.UserName);
        user.IsOnline = 1;
        await _userManager.UpdateAsync(user);
        await Clients.All.SendAsync("UserConnected", user.UserName);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var user = await _userManager.GetUserAsync(Context.User);
        _onlineUsersService.RemoveUser(user.UserName);
        user.IsOnline= 0;
        await _userManager.UpdateAsync(user);
        await Clients.All.SendAsync("UserDisconnected", user.UserName);
    }
}
