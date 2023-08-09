using Chat.Core.Entities;
using Chat.DataAccess.Repository.Interfaces;
using Chat.MVC.Models.Notification;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chat.MVC.Controllers;

public class NotificationController : Controller
{
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly IRequestRepository _requestRepository;

    public NotificationController(IFriendshipRepository friendshipRepository, UserManager<AppUser> userManager, IRequestRepository requestRepository)
    {
        _friendshipRepository = friendshipRepository;
        _userManager = userManager;
        _requestRepository = requestRepository;
    }



    public IActionResult Index()
    {
        ViewBag.ActiveMenu = "Notification";
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        var username = HttpContext.User.Identity?.Name;
        if (username is null)
        {
            return Json(null);
        }
        var user = await _userManager.FindByNameAsync(username);
        var notifications = await _requestRepository.FindAll().Where(n=>n.ToID == user.Id).ToListAsync();
        List<GetNotificationViewModel> list = new List<GetNotificationViewModel>();
        foreach (var notification in notifications)
        {
            var FromUser = await _userManager.FindByIdAsync(notification.FromID);
            GetNotificationViewModel n = new()
            {
                FromUser = FromUser.UserName,
                Image = FromUser.Image,
            };
            list.Add(n);
        }
        return Json(list);
    }



}
