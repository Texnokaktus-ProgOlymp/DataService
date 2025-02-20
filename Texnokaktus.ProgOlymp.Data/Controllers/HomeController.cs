using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Texnokaktus.ProgOlymp.Data.Models;

namespace Texnokaktus.ProgOlymp.Data.Controllers;

public class HomeController(ILogger<HomeController> logger) : Controller
{
    public IActionResult Index() => RedirectToAction(nameof(Index), "Contests");

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
