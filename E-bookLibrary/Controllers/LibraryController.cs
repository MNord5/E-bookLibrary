using Microsoft.AspNetCore.Mvc;

namespace E_bookLibrary.Controllers
{
    public class LibraryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
