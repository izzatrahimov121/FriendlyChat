using Chat.Core.Entities;
using Chat.DataAccess.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chat.MVC.Controllers;

public class ProfilController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IRequestRepository _requestRepository;


    public ProfilController(UserManager<AppUser> userManager, IRequestRepository requestRepository)
    {
        _userManager = userManager;
        _requestRepository = requestRepository;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.ActiveMenu = "Profil";
        var username = HttpContext.User.Identity?.Name;
        if (username is null)
        {
            ViewBag.UserName = "User";
            ViewBag.Email = "excample@gmail.com";
            ViewBag.Image = "default.png";
            return View();
        }
        var user = await _userManager.FindByNameAsync(username);
        var IsNewRequest = await _requestRepository.FindAll().Where(r => r.ToID == user.Id && r.Status == 0).ToListAsync();
        if (IsNewRequest.Count != 0)
        {
            ViewBag.IsNewNotification = "yes";
        }
        ViewBag.UserName = username;
        ViewBag.Email = user.Email; 
        ViewBag.Image = user.Image;
        return View();
    }
}
