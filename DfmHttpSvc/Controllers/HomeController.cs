using Microsoft.AspNetCore.Mvc;

namespace DfmHttpSvc.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
