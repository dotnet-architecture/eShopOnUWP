using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using eShop.Data;

namespace eShop.Server.Controllers
{
    public class PicController : Controller
    {
        private IHostingEnvironment _hostingEnvironment = null;

        public PicController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        private string ImageBaseUri => String.Format("{0}://{1}{2}", Request.Scheme, Request.Host, Url.Content("/Images"));

        // PUT api/upload
        [HttpPut]
        [Route("api/v1/catalog/items/{catalogItemId:int}/{extension}/pic")]
        public async Task<IActionResult> UploadImage(int catalogItemId, string extension)
        {
            string path = Path.Combine(_hostingEnvironment.WebRootPath, "Images");
            string fileName = Path.Combine(path, $"{catalogItemId}.{extension}");

            using (var db = new CatalogDb())
            {
                var item = db.CatalogItems.SingleOrDefault(i => i.Id == catalogItemId);
                if (item == null)
                {
                    return NotFound(new { Message = $"Item with id {catalogItemId} not found." });
                }

                using (var stream = new FileStream(fileName, FileMode.Create))
                {
                    await Request.Body.CopyToAsync(stream);
                }

                item.PictureFileName = $"{catalogItemId}.{extension}";
                item.PictureUri = $"{ImageBaseUri}/{item.PictureFileName}";
                db.SaveChanges();
            }
            return Ok();
        }
    }
}
