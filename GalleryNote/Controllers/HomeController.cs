using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GalleryNote.Controllers
{
    public class HomeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        
        // GET: Upload
        [Authorize]
        public IActionResult Upload()
        {
         return View();
        }
    }
}