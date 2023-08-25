using Chat.Core.Entities;
using Chat.DataAccess.Repository.Interfaces;
using Chat.MVC.HelperServices.Interfaces;
using Chat.MVC.Models.Media;
using Chat.MVC.Utilites.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
}
