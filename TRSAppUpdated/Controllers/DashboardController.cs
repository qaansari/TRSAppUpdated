using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TRSAppUpdated.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        [Authorize(Roles = "Super Admin,Admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
