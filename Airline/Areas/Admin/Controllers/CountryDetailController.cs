using Microsoft.AspNetCore.Mvc;

namespace Airline.Areas.Admin.Controllers
{
    public class CountryDetailController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
