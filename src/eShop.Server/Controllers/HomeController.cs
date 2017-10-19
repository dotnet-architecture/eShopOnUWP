using System;

using Microsoft.AspNetCore.Mvc;

namespace eShop.Server.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Host = Request.Host.ToString();
            return View();
        }
    }
}
