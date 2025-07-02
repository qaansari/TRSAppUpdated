using Microsoft.AspNetCore.Mvc;

namespace TRSAppUpdated.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
