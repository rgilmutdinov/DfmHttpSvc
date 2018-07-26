using Microsoft.AspNetCore.Mvc;

namespace DfmWeb.App.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
