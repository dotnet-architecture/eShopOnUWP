using System;
using System.IO;

using Microsoft.AspNetCore.Mvc;

namespace eShop.Server.Controllers
{
    public class JsonDataController : Controller
    {
        public IActionResult Index()
        {
            string json = System.IO.File.ReadAllText("_Db\\Catalog.json");
            return Content(json);
        }
    }
}
