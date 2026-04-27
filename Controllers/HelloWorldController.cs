using Microsoft.AspNetCore.Mvc;

namespace digital_library.Controllers;

public class HelloWorldController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Book");
    }

    public IActionResult Welcome(string name, int numTimes = 1)
    {
        return RedirectToAction("Index", "Book");
    }
}
