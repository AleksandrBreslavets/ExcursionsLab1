using Microsoft.AspNetCore.Mvc;

namespace ExcursionsInfrastructure.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
