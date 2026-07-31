using Microsoft.AspNetCore.Mvc;

namespace CoreIdentityWithOWIN.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
