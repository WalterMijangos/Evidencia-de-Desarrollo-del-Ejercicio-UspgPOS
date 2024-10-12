using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UspgPOS.Models;

namespace UspgPOS.Controllers
{
	public class HomeController : Controller
	{
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewData["Layout"] = "_LayoutLogged"; // Layout para usuarios logueados
            }
            else
            {
                ViewData["Layout"] = "_Layout"; // Layout para usuarios no logueados
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
