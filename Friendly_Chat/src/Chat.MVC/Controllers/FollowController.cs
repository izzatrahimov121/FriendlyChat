using Chat.Core.Entities;
using Chat.DataAccess.Repository.Interfaces;
using Chat.MVC.Models.Follow;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chat.MVC.Controllers;

public class FollowController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IRequestRepository _requestRepository;

    public FollowController(IRequestRepository requestRepository,UserManager<AppUser> userManager)
    {
        _requestRepository = requestRepository;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        ViewBag.ActiveMenu = "Follow";
        return View();
    }


    [HttpGet]
    public async Task<IActionResult> SearchByUsername(string username)
    {
        var users = await _userManager.Users.Where(u => u.UserName.Contains(username)).ToListAsync();
        List<GetUsersViewModel> resultList = new List<GetUsersViewModel>();
        foreach (var user in users)
        {
            GetUsersViewModel result = new()
            {
                Image=user.Image,
                UserName=user.UserName,
            };
            resultList.Add(result);
        }
        return Json(resultList);
    }


    [HttpPost]
    public async Task  SendFollowRequest(string username)
    {
        var user = await _userManager.FindByNameAsync(HttpContext.User.Identity?.Name);
        var ToRequestUser = await _userManager.FindByNameAsync(username);
        if (ToRequestUser != null)
        {
            FollowingRequest followingRequest = new()
            {
                FromID = user.Id,
                ToID = ToRequestUser.Id
            };
            await _requestRepository.CreateAsync(followingRequest);
            await _requestRepository.SaveAsync();
        }
    }


}
