using Chat.Core.Entities;
using Chat.DataAccess.Repository.Interfaces;
using Chat.MVC.Models.Profil;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chat.MVC.Controllers;

public class ProfilController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IRequestRepository _requestRepository;
    private readonly IFriendshipRepository _friendshipRepository;


    public ProfilController(UserManager<AppUser> userManager, IRequestRepository requestRepository, IFriendshipRepository friendshipRepository)
    {
        _userManager = userManager;
        _requestRepository = requestRepository;
        _friendshipRepository = friendshipRepository;
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
            ViewBag.Followers = "00";
            ViewBag.Following = "00";

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
        var followers = await _friendshipRepository.FindAll().Where(f=>f.FollowedID==user.Id).ToListAsync();
        var following = await _friendshipRepository.FindAll().Where(f=>f.UserID==user.Id).ToListAsync();
        ViewBag.Followers = followers.Count;
        ViewBag.Following = following.Count;
        return View();
    }


    [HttpGet]
    public async Task<JsonResult> GetFollowers()//takipçiler
    {
        var loginuser = HttpContext.User.Identity?.Name;
        var user = await _userManager.FindByNameAsync(loginuser);
        if (user == null)
        {
            return Json(null);
        }
        var followers = await _friendshipRepository.FindAll()
                                                   .Where(f => f.FollowedID == user.Id).ToListAsync();
        List<GetFollowViewModel> results = new List<GetFollowViewModel>();
        foreach (var f in followers)
        {
            var followingUser = await _userManager.FindByIdAsync(f.UserID);
            GetFollowViewModel model = new()
            {
                followingUserName = followingUser.UserName,
                followingUserEmail = followingUser.Email,
                followingUserImage = followingUser.Image,
            };
            results.Add(model);
        }
        return Json(results);
    }

    [HttpGet]
    public async Task<JsonResult> GetFollowing()//takip edilenler
    {
        var loginuser = HttpContext.User.Identity?.Name;
        var user = await _userManager.FindByNameAsync(loginuser);
        if (user == null)
        {
            return Json(null);
        }
        var followers = await _friendshipRepository.FindAll()
                                                   .Where(f => f.UserID == user.Id).ToListAsync();
        List<GetFollowViewModel> results = new List<GetFollowViewModel>();
        foreach (var f in followers)
        {
            var followingUser = await _userManager.FindByIdAsync(f.FollowedID);
            GetFollowViewModel model = new()
            {
                followingUserName = followingUser.UserName,
                followingUserEmail = followingUser.Email,
                followingUserImage = followingUser.Image,
            };
            results.Add(model);
        }
        return Json(results);
    }

}
