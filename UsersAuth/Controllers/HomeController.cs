using Microsoft.AspNetCore.Mvc;

namespace UsersAuth.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View("");
    }
}
