using Microsoft.AspNetCore.Mvc;

namespace Golfklubb_Centar_Webbshop.Controllers
{
    public class NotificationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
