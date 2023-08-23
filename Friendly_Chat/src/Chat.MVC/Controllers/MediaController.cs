using Microsoft.AspNetCore.Mvc;

namespace Chat.MVC.Controllers;

public class MediaController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult SharePost()
    {
        return View();
    }
}
