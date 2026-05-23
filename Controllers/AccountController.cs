using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using digital_library.Data;
using digital_library.Models;

namespace digital_library.Controllers;

public class AccountController : Controller
{
    private const string SessionUserId = "UserId";
    private const string SessionUsername = "Username";

    private readonly digital_libraryContext _context;

    public AccountController(digital_libraryContext context)
    {
        _context = context;
    }

    // GET: Account/Login
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (HttpContext.Session.GetInt32(SessionUserId) is not null)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    // POST: Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var input = model.UsernameOrEmail.Trim();
        var user = _context.User.FirstOrDefault(u => u.Username == input || u.Email == input);

        if (user is null || !PasswordHasher.Verify(model.Password, user.PasswordHash))
        {
            ModelState.AddModelError(string.Empty, "Invalid username/email or password.");
            return View(model);
        }

        SignIn(user);
        TempData["StatusMessage"] = $"Welcome back, {user.Username}!";

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    // GET: Account/Register
    [HttpGet]
    public IActionResult Register()
    {
        if (HttpContext.Session.GetInt32(SessionUserId) is not null)
        {
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    // POST: Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var username = model.Username.Trim();
        var email = model.Email.Trim();

        if (_context.User.Any(u => u.Username == username))
        {
            ModelState.AddModelError(nameof(model.Username), "That username is already taken.");
            return View(model);
        }

        if (_context.User.Any(u => u.Email == email))
        {
            ModelState.AddModelError(nameof(model.Email), "That email is already registered.");
            return View(model);
        }

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = PasswordHasher.Hash(model.Password),
            CreatedAt = DateTime.UtcNow
        };

        _context.User.Add(user);
        _context.SaveChanges();

        SignIn(user);
        TempData["StatusMessage"] = $"Welcome, {user.Username}! Your account has been created.";
        return RedirectToAction("Index", "Home");
    }

    // POST: Account/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        TempData["StatusMessage"] = "You have been logged out.";
        return RedirectToAction("Login");
    }

    private void SignIn(User user)
    {
        HttpContext.Session.SetInt32(SessionUserId, user.Id);
        HttpContext.Session.SetString(SessionUsername, user.Username);
    }
}
