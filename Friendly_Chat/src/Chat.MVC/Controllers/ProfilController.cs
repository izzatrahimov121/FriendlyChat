using Chat.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chat.MVC.Controllers;

public class ProfilController : Controller
{
    private readonly UserManager<AppUser> _userManager;

    public ProfilController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.ActiveMenu = "Profil";
        var username = HttpContext.User.Identity?.Name;
        if (username is null)
        {
            ViewBag.LoginUser = "User";
            return View();
        }
        ViewBag.LoginUser = username;
        return View();
    }
}
