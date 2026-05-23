using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace digital_library.Controllers;

public class HomeController : Controller
{
    // GET: / or /Home/Index
    public IActionResult Index()
    {
        // Not signed in -> send the visitor to the login view.
        if (HttpContext.Session.GetInt32("UserId") is null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Signed in -> render the home view.
        ViewData["Username"] = HttpContext.Session.GetString("Username");
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
}
