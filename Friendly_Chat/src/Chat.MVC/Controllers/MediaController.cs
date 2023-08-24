using Chat.Core.Entities;
using Chat.DataAccess.Repository.Interfaces;
using Chat.MVC.Models.Media;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chat.MVC.Controllers;

public class MediaController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IPostRepository _postRepository;
    private readonly ILikeRepository _likeRepository;
    private readonly IFriendshipRepository _friendshipRepository;

    public MediaController(UserManager<AppUser> userManager, IPostRepository postRepository, ILikeRepository likeRepository, IFriendshipRepository friendshipRepository)
    {
        _userManager = userManager;
        _postRepository = postRepository;
        _likeRepository = likeRepository;
        _friendshipRepository = friendshipRepository;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Share()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Share(SharePostViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Make sure the information is entered correctly");
            return View(model);
        }
        var user = await _userManager.GetUserAsync(User);//login user

        return View(model);
    }
}
