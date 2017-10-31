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

        private string GetImageMimeTypeFromImageFileExtension(string extension)
        {
            string mimetype;

            switch (extension)
            {
                case ".png":
                    mimetype = "image/png";
                    break;
                case ".gif":
                    mimetype = "image/gif";
                    break;
                case ".jpg":
                case ".jpeg":
                    mimetype = "image/jpeg";
                    break;
                case ".bmp":
                    mimetype = "image/bmp";
                    break;
                case ".tiff":
                    mimetype = "image/tiff";
                    break;
                case ".wmf":
                    mimetype = "image/wmf";
                    break;
                case ".jp2":
                    mimetype = "image/jp2";
                    break;
                case ".svg":
                    mimetype = "image/svg+xml";
                    break;
                default:
                    mimetype = "application/octet-stream";
                    break;
            }

            return mimetype;
        }
    }
}
