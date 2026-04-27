using Microsoft.AspNetCore.Mvc;

namespace digital_library.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Book");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Logout()
    {
        TempData["StatusMessage"] = "You have been logged out.";
        return RedirectToAction("Index", "Book");
    }
}
