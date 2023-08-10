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
    private readonly IFriendshipRepository _friendshipRepository;

    public FollowController(IRequestRepository requestRepository
                          , UserManager<AppUser> userManager
                          , IFriendshipRepository friendshipRepository)
    {
        _requestRepository = requestRepository;
        _userManager = userManager;
        _friendshipRepository = friendshipRepository;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.ActiveMenu = "Follow";
        var username = HttpContext.User.Identity?.Name;
        if (username is null)
        {
            return View();
        }
        var user = await _userManager.FindByNameAsync(username);
        var IsNewRequest = await _requestRepository.FindAll().Where(r => r.ToID == user.Id && r.Status == 0).ToListAsync();
        if (IsNewRequest.Count != 0)
        {
            ViewBag.IsNewNotification = "yes";
        }
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
                ToID = ToRequestUser.Id,
            };
            await _requestRepository.CreateAsync(followingRequest);
            await _requestRepository.SaveAsync();
        }
    }


    [HttpPost]
    public async Task ToAcceptRequest(string username)
    {
        var loginUser = await _userManager.FindByNameAsync(HttpContext.User.Identity?.Name);
        var user = await _userManager.FindByNameAsync(username);

        Friendship friendship = new()
        {
            UserID = user.Id,
            FollowedID = loginUser.Id,
        };

        await _friendshipRepository.CreateAsync(friendship);
        await _friendshipRepository.SaveAsync();

        var request = await _requestRepository.FindAll()
                            .FirstOrDefaultAsync(r => r.FromID == user.Id && r.ToID == loginUser.Id);
        if (request != null)
        {
            _requestRepository.Delete(request);
            await _requestRepository.SaveAsync();
        }
    }


    [HttpPost]
    public async Task ToRejectRequest(string username)
    {
        var loginUser = await _userManager.FindByNameAsync(HttpContext.User.Identity?.Name);
        var user = await _userManager.FindByNameAsync(username);
        var request = await _requestRepository.FindAll()
                            .FirstOrDefaultAsync(r=>r.FromID==user.Id && r.ToID == loginUser.Id);
        if (request != null)
        {
            _requestRepository.Delete(request);
            await _requestRepository.SaveAsync();
        }
    }
}
