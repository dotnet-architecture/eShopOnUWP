using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

using eShop.Data;

namespace eShop.Server.Controllers
{
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class CatalogController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;

        public CatalogController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        private string ImageBaseUri => String.Format("{0}://{1}{2}", Request.Scheme, Request.Host, Url.Content("/Images"));

        // GET api/v1/[controller]/CatalogTypes
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(List<CatalogType>), (int)HttpStatusCode.OK)]
        public IActionResult CatalogTypes()
        {
            using (var db = new CatalogDb())
            {
                return Ok(db.CatalogTypes);
            }
        }

        // GET api/v1/[controller]/CatalogBrands
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(List<CatalogBrand>), (int)HttpStatusCode.OK)]
        public IActionResult CatalogBrands()
        {
            using (var db = new CatalogDb())
            {
                return Ok(db.CatalogBrands);
            }
        }

        // GET api/v1/[controller]/items[?pageSize=3&pageIndex=10]
        [HttpGet]
        [Route("[action]")]
        public IActionResult Items([FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0)
        {
            using (var db = new CatalogDb())
            {
                long totalItems = db.CatalogItems.LongCount();

                var itemsOnPage = db.CatalogItems.OrderBy(c => c.Name).Skip(pageSize * pageIndex).Take(pageSize).ToList();
                itemsOnPage = ChangeUriPlaceholder(itemsOnPage);
                var model = new PaginatedItems<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage);

                return Ok(model);
            }
        }

        // GET api/v1/[controller]/items/withname/samplename[?pageSize=3&pageIndex=10]
        [HttpGet]
        [Route("[action]/withname/{name:minlength(1)}")]
        [ProducesResponseType(typeof(PaginatedItems<CatalogItem>), (int)HttpStatusCode.OK)]
        public IActionResult Items(string name, [FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0)
        {
            using (var db = new CatalogDb())
            {
                long totalItems = db.CatalogItems.Where(c => c.Name.StartsWith(name)).LongCount();

                var itemsOnPage = db.CatalogItems.Where(c => c.Name.StartsWith(name)).Skip(pageSize * pageIndex).Take(pageSize).OrderBy(c => c.Name).ToList();
                itemsOnPage = ChangeUriPlaceholder(itemsOnPage);

                var model = new PaginatedItems<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage);
                return Ok(model);
            }
        }

        // GET api/v1/[controller]/items/type/1/brand/null[?pageSize=3&pageIndex=10]
        [HttpGet]
        [Route("[action]/type/{catalogTypeId}/brand/{catalogBrandId}")]
        [ProducesResponseType(typeof(PaginatedItems<CatalogItem>), (int)HttpStatusCode.OK)]
        public IActionResult Items(int? catalogTypeId, int? catalogBrandId, [FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0)
        {
            using (var db = new CatalogDb())
            {
                IEnumerable<CatalogItem> items = db.CatalogItems;

                if (catalogTypeId != null && catalogTypeId > -1)
                {
                    items = items.Where(r => r.CatalogTypeId == catalogTypeId);
                }

                if (catalogBrandId != null && catalogBrandId > -1)
                {
                    items = items.Where(r => r.CatalogBrandId == catalogBrandId);
                }

                long totalItems = items.LongCount();
                var itemsOnPage = items.Skip(pageSize * pageIndex).Take(pageSize).OrderBy(c => c.Name).ToList();
                itemsOnPage = ChangeUriPlaceholder(itemsOnPage);

                var model = new PaginatedItems<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage);
                return Ok(model);
            }
        }

        [HttpGet]
        [Route("items/{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(CatalogItem), (int)HttpStatusCode.OK)]
        public IActionResult GetItemById(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            using (var db = new CatalogDb())
            {
                var item = db.CatalogItems.SingleOrDefault(ci => ci.Id == id);
                if (item != null)
                {
                    ChangeUriPlaceholder(item);
                    return Ok(item);
                }

                return NotFound();
            }
        }

        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public IActionResult CreateProduct([FromBody]CatalogItem product)
        {
            using (var db = new CatalogDb())
            {
                var item = new CatalogItem
                {
                    CatalogBrandId = product.CatalogBrandId,
                    CatalogTypeId = product.CatalogTypeId,
                    Description = product.Description,
                    Name = product.Name,
                    PictureFileName = product.PictureFileName,
                    Price = product.Price
                };
                item.Id = db.CatalogItems.Max(r => r.Id) + 1;
                ChangeUriPlaceholder(item);

                db.CatalogItems.Add(item);
                db.SaveChanges();

                return CreatedAtAction(nameof(GetItemById), new { id = item.Id }, item);
            }
        }

        //PUT api/v1/[controller]/items
        [Route("items")]
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public IActionResult UpdateProduct([FromBody]CatalogItem productToUpdate)
        {
            using (var db = new CatalogDb())
            {
                var item = db.CatalogItems.SingleOrDefault(i => i.Id == productToUpdate.Id);
                if (item == null)
                {
                    return NotFound(new { Message = $"Item with id {productToUpdate.Id} not found." });
                }
                db.CatalogItems.Remove(item);

                ChangeUriPlaceholder(productToUpdate);
                db.CatalogItems.Add(productToUpdate);
                db.SaveChanges();

                return CreatedAtAction(nameof(GetItemById), new { id = productToUpdate.Id }, null);
            }
        }

        //DELETE api/v1/[controller]/id
        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public IActionResult DeleteProduct(int id)
        {
            using (var db = new CatalogDb())
            {
                var product = db.CatalogItems.SingleOrDefault(x => x.Id == id);

                if (product == null)
                {
                    return NotFound();
                }

                db.CatalogItems.Remove(product);
                db.SaveChanges();

                return NoContent();
            }
        }


        private List<CatalogItem> ChangeUriPlaceholder(List<CatalogItem> items)
        {
            var baseUri = ImageBaseUri;

            items.ForEach(item =>
            {
                if (item.PictureUri != null)
                {
                    if (!item.PictureUri.StartsWith("ms-appx:"))
                    {
                        item.PictureUri = $"{baseUri}/{item.PictureFileName}";
                    }
                }
            });

            return items;
        }
        private CatalogItem ChangeUriPlaceholder(CatalogItem item)
        {
            if (item.PictureUri != null)
            {
                if (!item.PictureUri.StartsWith("ms-appx:"))
                {
                    item.PictureUri = $"{ImageBaseUri}/{item.PictureFileName}";
                }
            }
            return item;
        }
    }
}
