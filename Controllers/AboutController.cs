using Microsoft.AspNetCore.Mvc;

namespace GiftLab.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
