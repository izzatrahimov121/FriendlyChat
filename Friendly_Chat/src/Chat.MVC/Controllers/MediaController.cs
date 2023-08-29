using Chat.Core.Entities;
using Chat.DataAccess.Repository.Interfaces;
using Chat.MVC.HelperServices.Interfaces;
using Chat.MVC.Models.Media;
using Chat.MVC.Utilites.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;

namespace Chat.MVC.Controllers;

public class MediaController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IPostRepository _postRepository;
    private readonly ILikeRepository _likeRepository;
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;
    private readonly IFileService _fileService;

    public MediaController(UserManager<AppUser> userManager, IPostRepository postRepository, ILikeRepository likeRepository, IFriendshipRepository friendshipRepository, IFileService fileService, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
    {
        _userManager = userManager;
        _postRepository = postRepository;
        _likeRepository = likeRepository;
        _friendshipRepository = friendshipRepository;
        _fileService = fileService;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.ActiveMenu = "Media";
        return View();
    }

    public IActionResult Share()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Share(SharePostViewModel model)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);//login user
            if (user is null)
            {
                ModelState.AddModelError("", "Please login first and try again");
                return View(model);
            }
            if (model.File is null)
            {
                ModelState.AddModelError("", "No file Selected");
                return View(model);
            }
            Post post = new Post();
            post.UserId = user.Id;
            if (model.Description is not null) post.Description = model.Description;
            if (model.File.CheckFileFormat("image/"))
            {
                post.PostType = "image";
                var fileName = await _fileService.CopyFileAsyncForMedia(model.File, _env.WebRootPath, "assets", "media", "photos");
                post.PostName = fileName;
            }
            if (model.File.CheckFileFormat("video/"))
            {
                post.PostType = "video";
                var fileName = await _fileService.CopyFileAsyncForMedia(model.File, _env.WebRootPath, "assets", "media", "videos");
                post.PostName = fileName;
            }
            if (post.PostType is null)
            {
                ModelState.AddModelError("", "Please selected only 'Image' and 'Video' file");
                return View(model);
            }
            await _postRepository.CreateAsync(post);
            await _postRepository.SaveAsync();
            return View();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }

    public async Task<List<GetPostViewModel>> Posts()
    {
        var loginUser = await _userManager.FindByNameAsync(HttpContext.User.Identity?.Name);

        List<GetPostViewModel> getPosts = new List<GetPostViewModel>();

        var friends = await _friendshipRepository.FindAll().Where(f => f.UserID == loginUser.Id).ToListAsync();
        if (friends is not null)
        {
            foreach (var friend in friends)
            {
                var posts = await _postRepository.FindAll().Where(p => p.UserId == friend.FollowedID).ToListAsync();
                if (posts is not null)
                {
                    var user = await _userManager.FindByIdAsync(friend.FollowedID);
                    foreach (var post in posts)
                    {
                        GetPostViewModel model = new GetPostViewModel();
                        model.PostType = post.PostType;
                        model.PostName = post.PostName;
                        model.UserName = user.UserName;
                        model.CreatedAt = post.CreatedAt;
                        model.UserPhoto = user.Image;
                        var d = (TimeSpan)(DateTime.UtcNow - post.CreatedAt);
                        if (d.Days >= 30)
                        {
                            model.Date = $"{d.Days / 30} moons ago";
                        }
                        else if (d.Days >= 1 && d.Days < 30)
                        {
                            model.Date = $"{d.Days} days ago";
                        }
                        else
                        {
                            if (d.Hours >= 1)
                            {
                                model.Date = $"{d.Hours} hours ago";
                            }
                            else
                            {
                                model.Date = $"{d.Minutes} mins ago";
                            }
                        }
                        if (post.Description is not null)
                        {
                            model.Description = post.Description;
                        }
                        getPosts.Add(model);
                    }
                }
            }
        }



        var followers = await _friendshipRepository.FindAll().Where(f => f.FollowedID == loginUser.Id).ToListAsync();
        var followersNotInFriends = followers.Where(f => !friends.Any(fr => fr.UserID == f.UserID)).ToList();
        if (followersNotInFriends is not null)
        {
            foreach (var follower in followersNotInFriends)
            {
                var posts = await _postRepository.FindAll().Where(p => p.UserId == follower.UserID).ToListAsync();
                if (posts is not null)
                {
                    var user = await _userManager.FindByIdAsync(follower.UserID);
                    foreach (var post in posts)
                    {
                        GetPostViewModel model = new GetPostViewModel();
                        model.PostType = post.PostType;
                        model.PostName = post.PostName;
                        model.CreatedAt = post.CreatedAt;
                        model.UserName = user.UserName;
                        model.UserPhoto = user.Image;
                        var d = (TimeSpan)(DateTime.UtcNow - post.CreatedAt);
                        if (d.Days >= 30)
                        {
                            model.Date = $"{d.Days / 30} moons ago";
                        }
                        else if (d.Days >= 1 && d.Days < 30)
                        {
                            model.Date = $"{d.Days} days ago";
                        }
                        else
                        {
                            if (d.Hours >= 1)
                            {
                                model.Date = $"{d.Hours} hours ago";
                            }
                            else
                            {
                                model.Date = $"{d.Minutes} mins ago";
                            }
                        }
                        if (post.Description is not null)
                        {
                            model.Description = post.Description;
                        }
                        getPosts.Add(model);
                    }
                }
            }
        }

        getPosts = getPosts.DistinctBy(r => r.PostName).ToList();
        return getPosts.OrderByDescending(p => p.CreatedAt).ToList();
    }

    [HttpGet]
    public async Task<JsonResult> GetPosts(int count)
    {
        var posts = await Posts();
        if (count == 0) { return Json(posts.Take(2).ToList()); }
        else { return Json(posts.Skip(count * 2).Take(2).ToList()); }
    }
}
