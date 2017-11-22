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
        [Route("api/v1/catalog/items/{catalogItemId:int}/pic")]
        public async Task<IActionResult> UploadImage(int catalogItemId)
        {
            using (var db = new CatalogDb())
            {
                var item = db.CatalogItems.SingleOrDefault(i => i.Id == catalogItemId);
                if (item == null)
                {
                    return NotFound(new { Message = $"Item with id {catalogItemId} not found." });
                }

                string rootPath = _hostingEnvironment.WebRootPath ?? "wwwroot";
                string imagePath = Path.Combine(rootPath, "Images");

                // Create if no exists
                Directory.CreateDirectory(imagePath);

                string extension = GetExtensionFromContentType(Request.ContentType);
                string fileName = Path.Combine(imagePath, $"{catalogItemId}{extension}");

                using (var stream = new FileStream(fileName, FileMode.Create))
                {
                    await Request.Body.CopyToAsync(stream);
                }

                item.PictureFileName = Path.GetFileName(fileName);
                item.PictureUri = $"{ImageBaseUri}/{item.PictureFileName}";
                db.SaveChanges();
            }

            return Ok();
        }

        private string GetExtensionFromContentType(string contentType)
        {
            contentType = contentType ?? "";
            switch (contentType.ToLower())
            {
                case "image/png":
                    return ".png";
                case "image/gif":
                    return ".gif";
                case "image/jpeg":
                    return ".jpg";
                case "image/bmp":
                    return ".bmp";
                case "image/tiff":
                    return ".tiff";
                case "image/wmf":
                    return ".wmf";
                case "image/jp2":
                    return ".jp2";
                case "image/svg+xml":
                    return ".svg";
                default:
                    return "";
            }
        }

        static public string GetContentTypeFromExtension(string extension)
        {
            switch (extension)
            {
                case ".png":
                    return "image/png";
                case ".gif":
                    return "image/gif";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".bmp":
                    return "image/bmp";
                case ".tiff":
                    return "image/tiff";
                case ".wmf":
                    return "image/wmf";
                case ".jp2":
                    return "image/jp2";
                case ".svg":
                    return "image/svg+xml";
                default:
                    return "application/octet-stream";
            }
        }
    }
}
