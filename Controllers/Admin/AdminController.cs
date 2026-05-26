using Microsoft.AspNetCore.Mvc;

namespace FruitShopG4P.Controllers.Admin
{
    public class AdminController : Controller
    {
        public IActionResult IndexAdmin()
        {
            return View("~/Views/Admin/IndexAdmin.cshtml");
        }
        public IActionResult IndexNhanVien()
        {
            return View("~/Views/Admin/IndexNhanVien.cshtml");
        }
    }
}
