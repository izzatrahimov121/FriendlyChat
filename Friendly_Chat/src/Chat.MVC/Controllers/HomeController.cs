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

    public HomeController(UserManager<AppUser> userManager, IFriendshipRepository friendshipRepository, IRequestRepository requestRepository)
    {
        _userManager = userManager;
        _friendshipRepository = friendshipRepository;
        _requestRepository = requestRepository;
    }

    public async Task<IActionResult> Index()
	{
        ViewBag.ActiveMenu = "Home";
        var username = HttpContext.User.Identity?.Name;
        if (username is null)
        {
            return View();
        }
        var user = await _userManager.FindByNameAsync(username);
        var IsRequest = await _requestRepository.FindAll().Where(r => r.ToID == user.Id).ToListAsync();
        ViewBag.IsNotification = "no";
        if (IsRequest is not null)
        {
            ViewBag.IsNotification = "yes";
        }
        return View();
	}
}
