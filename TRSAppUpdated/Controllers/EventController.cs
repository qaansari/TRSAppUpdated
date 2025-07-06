using Microsoft.AspNetCore.Mvc;

namespace TRSAppUpdated.Controllers
{
    public class EventController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
