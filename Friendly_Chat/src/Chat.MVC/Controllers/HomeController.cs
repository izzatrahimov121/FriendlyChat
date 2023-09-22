using Chat.Core.Entities;
using Chat.DataAccess.Repository.Interfaces;
using Chat.MVC.Models.Home;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace Chat.MVC.Controllers;
public class HomeController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IRequestRepository _requestRepository;
    private readonly IMessageRepository _messageRepository;

    public HomeController(UserManager<AppUser> userManager, IFriendshipRepository friendshipRepository, IRequestRepository requestRepository, IMessageRepository messageRepository)
    {
        _userManager = userManager;
        _friendshipRepository = friendshipRepository;
        _requestRepository = requestRepository;
        _messageRepository = messageRepository;
    }

    public async Task<IActionResult> Index()
    {
        var username = HttpContext.User.Identity?.Name;
        if (username is null)
        {
            return RedirectToAction("Login", "Auth");
        }
        ViewBag.ActiveMenu = "Home";
        var user = await _userManager.FindByNameAsync(username);
        var IsNewRequest = await _requestRepository.FindAll().Where(r => r.ToID == user.Id && r.Status == 0).ToListAsync();
        if (IsNewRequest.Count != 0)
        {
            ViewBag.IsNewNotification = "yes";
        }
        return View();
    }


    [HttpGet]
    public async Task<JsonResult> GetUsers()
    {
        var loginUser = await _userManager.FindByNameAsync(HttpContext.User.Identity?.Name);
        List<GetChatUsersViewModel> results = new List<GetChatUsersViewModel>();


        var friends = await _friendshipRepository.FindAll().Where(f => f.UserID == loginUser.Id).ToListAsync();
        foreach (var friend in friends)
        {
            var FollowedUser = await _userManager.FindByIdAsync(friend.FollowedID);
            var newMessages = await _messageRepository.FindAll()
                    .Where(m => m.FromUserID == FollowedUser.Id && m.ToUserID == loginUser.Id && m.IsRead == 0)
                    .ToListAsync();
            GetChatUsersViewModel model = new()
            {
                ToUserName = FollowedUser.UserName,
                ToUserImage = FollowedUser.Image,
                LastMessage = "Test123 4241",
            };
            if (newMessages is not null) model.NewMessagesCount = newMessages.Count;
            if (FollowedUser.IsOnline == 1) model.IsOnline = true;
            results.Add(model);
        }



        var followers = await _friendshipRepository.FindAll().Where(f => f.FollowedID == loginUser.Id).ToListAsync();

        var followersNotInFriends = followers.Where(f => !friends.Any(fr => fr.UserID == f.UserID)).ToList();
        foreach (var follower in followersNotInFriends)
        {
            var FollowerUser = await _userManager.FindByIdAsync(follower.UserID);
            var newMessages = await _messageRepository.FindAll()
                    .Where(m => m.FromUserID == FollowerUser.Id && m.ToUserID == loginUser.Id && m.IsRead == 0)
                    .ToListAsync();
            GetChatUsersViewModel model = new()
            {
                ToUserName = FollowerUser.UserName,
                ToUserImage = FollowerUser.Image,
                LastMessage = "Test123 4241",
            };
            if (newMessages is not null) model.NewMessagesCount = newMessages.Count;
            if (FollowerUser.IsOnline == 1) model.IsOnline = true;
            results.Add(model);
        }
        return Json(results.DistinctBy(r => r.ToUserName).ToList());//eynileri silme
    }



    //-----------
    [HttpGet]
    public async Task<JsonResult> GetOldChat(string toUserName)
    {
        var fromUser = await _userManager.FindByNameAsync(HttpContext.User.Identity?.Name);
        var toUser = await _userManager.FindByNameAsync(toUserName);
        var messages = await _messageRepository.FindAll()
                                                .Where(m => (m.FromUserID == toUser.Id && m.ToUserID == fromUser.Id) ||
                                                           (m.FromUserID == fromUser.Id && m.ToUserID == toUser.Id))
                                                .OrderBy(m => m.CreatedAt)
                                                .ToListAsync();
        DateTime currentDate = DateTime.MinValue;
        List<GetOldChatViewModel> results = new List<GetOldChatViewModel>();
        foreach (var message in messages)
        {
            GetOldChatViewModel model = new GetOldChatViewModel();
            if (message.FromUserID == fromUser.Id) model.fromUserName = fromUser.UserName;
            if (message.ToUserID == toUser.Id) model.fromUserName = toUser.UserName;
            DateTime CreatedAt = (DateTime)message.CreatedAt;
            model.message = message.Content;
            model.time = CreatedAt.ToString("HH:mm");
            if (CreatedAt.Date != currentDate.Date)
            {
                model.Date = CreatedAt.ToString("dd/MM/yyyy");
                currentDate = CreatedAt;
            }
            results.Add(model);
            if (message.ToUserID == fromUser.UserName)
                message.IsRead = 1;
            _messageRepository.Update(message);
            await _messageRepository.SaveAsync();
        }
        return Json(results);
    }

    [HttpGet]
    public async Task<JsonResult> GetNewMessage(string toUserName)
    {
        var fromUser = await _userManager.FindByNameAsync(HttpContext.User.Identity?.Name);
        var toUser = await _userManager.FindByNameAsync(toUserName);
        var messages = await _messageRepository.FindAll()
                            .Where(m => m.FromUserID == toUser.Id && m.ToUserID == fromUser.Id && m.IsRead == 0)
                            .ToListAsync();
        List<GetMessageViewModel> results = new List<GetMessageViewModel>();
        foreach (var message in messages)
        {
            DateTime CreatedAt = (DateTime)message.CreatedAt;
            GetMessageViewModel model = new()
            {
                Message = message.Content,
                time = CreatedAt.ToString("HH:mm")
            };
            results.Add(model);
            message.IsRead = 1;
            _messageRepository.Update(message);
            await _messageRepository.SaveAsync();
        }
        return Json(results);
    }

    [HttpPost]
    public async Task SendMessage(string toUserName, string content)
    {
        var fromUser = await _userManager.FindByNameAsync(HttpContext.User.Identity?.Name);
        var toUser = await _userManager.FindByNameAsync(toUserName);
        Message newMessage = new()
        {
            FromUserID = fromUser.Id,
            ToUserID = toUser.Id,
            Content = content,
        };
        await _messageRepository.CreateAsync(newMessage);
        await _messageRepository.SaveAsync();
    }
}
