using Chat.Core.Entities;
using Chat.DataAccess.Repository.Interfaces;
using Chat.MVC.HelperServices.Interfaces;
using Chat.MVC.Models.Profil;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;

namespace Chat.MVC.Controllers;

public class ProfilController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IRequestRepository _requestRepository;
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;
    private readonly IFileService _fileService;
    private readonly SignInManager<AppUser> _signInManager;



    public ProfilController(UserManager<AppUser> userManager, IRequestRepository requestRepository, IFriendshipRepository friendshipRepository, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IFileService fileService, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _requestRepository = requestRepository;
        _friendshipRepository = friendshipRepository;
        _env = env;
        _fileService = fileService;
        _signInManager = signInManager;
    }

    public async Task<IActionResult> Index()
    {
        UserProfilViewModel model = new UserProfilViewModel();
        var username = HttpContext.User.Identity?.Name;
        if (username is null)
        {
            return RedirectToAction("Login", "Auth");
        }
        ViewBag.ActiveMenu = "Profil";

        var user = await _userManager.FindByNameAsync(username);
        var IsNewRequest = await _requestRepository.FindAll().Where(r => r.ToID == user.Id && r.Status == 0).ToListAsync();
        if (IsNewRequest.Count != 0)
        {
            ViewBag.IsNewNotification = "yes";
        }
        //ViewBag.UserName = username;
        //ViewBag.Email = user.Email;
        //ViewBag.Image = user.Image;
        model.Username = username;
        model.Email = user.Email;
        model.Photo = user.Image;

        var followers = await _friendshipRepository.FindAll().Where(f => f.FollowedID == user.Id).ToListAsync();
        var following = await _friendshipRepository.FindAll().Where(f => f.UserID == user.Id).ToListAsync();
        //ViewBag.Followers = followers.Count;
        //ViewBag.Following = following.Count;
        model.Following = following.Count;
        model.Followers = followers.Count;
        return View(model);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(UserProfilViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Make sure the information is entered correctly");
                return View(model);
            }
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity?.Name);
            var users = await _userManager.Users.ToListAsync();
            users.Remove(user);
            foreach(var u in users)
            {
                if(u.UserName == model.Username)
                {
                    throw new Exception("This username has already been used. Please choose another name");
                }
                if(u.Email == model.Email)
                {
                    throw new Exception("This email is currently in use. Please choose another email");
                }
            }
            if (user == null) return RedirectToAction("Login", "Auth");

            var fileName = user.Image;
            if (model.Image is not null)
            {
                fileName = await _fileService.CopyFileAsync(model.Image, _env.WebRootPath, "assets", "images");
            }
            user.UserName = model.Username;
            user.Email = model.Email;
            user.Image = fileName;
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Index", "Profil");
        }
        catch (Exception ex)
        {
            var username = HttpContext.User.Identity?.Name;
            ViewBag.ActiveMenu = "Profil";

            var user = await _userManager.FindByNameAsync(username);
            var IsNewRequest = await _requestRepository.FindAll().Where(r => r.ToID == user.Id && r.Status == 0).ToListAsync();
            if (IsNewRequest.Count != 0)
            {
                ViewBag.IsNewNotification = "yes";
            }
            model.Username = username;
            model.Email = user.Email;
            model.Photo = user.Image;

            var followers = await _friendshipRepository.FindAll().Where(f => f.FollowedID == user.Id).ToListAsync();
            var following = await _friendshipRepository.FindAll().Where(f => f.UserID == user.Id).ToListAsync();
            model.Following = following.Count;
            model.Followers = followers.Count;
            ModelState.AddModelError("",ex.Message);
            return View(model);
        }
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
