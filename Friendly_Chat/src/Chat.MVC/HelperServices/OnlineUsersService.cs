using System.Collections.Concurrent;

namespace Chat.MVC.HelperServices;

public class OnlineUsersService
{
    private readonly ConcurrentDictionary<string, string> _onlineUsers = new();

    public void AddUser(string userId)
    {
        _onlineUsers.TryAdd(userId, userId);
    }

    public void RemoveUser(string userId)
    {
        _onlineUsers.TryRemove(userId, out _);
    }

    public IEnumerable<string> GetOnlineUsers()
    {
        return _onlineUsers.Values;
    }
}
