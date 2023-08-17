using Chat.Core.Entities;
using Chat.MVC.HelperServices.Interfaces;
using Chat.MVC.Models.Auth;
using Chat.MVC.Models.Profil;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chat.MVC.Controllers;

public class AuthController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }


    #region Register

    public IActionResult Register()
    {
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) { return View(model); }

        AppUser user = new()
        {
            Email = model.Email,
            UserName = model.UserName,
        };

        var users = await _userManager.Users.ToListAsync();
        foreach (var u in users)
        {
            if (model.UserName == u.UserName)
            {
                ModelState.AddModelError("", "This Username already exists");
                return View(model);
            }
        }
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        return RedirectToAction(nameof(Login));
    }



    #endregion

    #region Login
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel loginVM)
    {
        var user = await _userManager.FindByEmailAsync(loginVM.UserNameOrEmail);
        if (user is null)
        {
            user = await _userManager.FindByNameAsync(loginVM.UserNameOrEmail);
            if (user is null)
            {
                ModelState.AddModelError("", "Username/Email or Password incorrect");
                return View(loginVM);
            }
        }

        var signInResult = await _signInManager.PasswordSignInAsync(user, loginVM.Password, (bool)loginVM.RememberMe, true);

        if (signInResult.IsLockedOut)
        {
            ModelState.AddModelError("", "Please try again soon");
            return View(loginVM);
        }

        if (!signInResult.Succeeded)
        {
            ModelState.AddModelError("", "Username/Email or Password incorrect");
            return View(loginVM);
        }

        return RedirectToAction("Index","Home");
    }
    #endregion

    #region Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }
    #endregion
}
