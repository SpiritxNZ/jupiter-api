using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using jupiterCore.jupiterContext;
using Jupiter.ActionFilter;
using Jupiter.Controllers;
using Jupiter.Models;
using Microsoft.AspNetCore.Authorization;

namespace jupiterCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : BasicController
    {
        private readonly jupiterContext.jupiterContext _context;
        private readonly IMapper _mapper;

        public ProductsController(jupiterContext.jupiterContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProduct()
        {
            var productValue = await _context.Product
                .Include(x=>x.ProductMedia)
                .Include(x=>x.Category)
                .Include(x=>x.ProdType)
                .Include(x=>x.ProductDetail)
                .ToListAsync();
            return productValue;
        }

        [Route("[action]")]
        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetSpecialProduct()
        {
            var result = new Result<IEnumerable<Product>>();
            var specialProducts = _context.Product.Where(x=>x.IsActivate == 1 && x.SpecialOrder > 0).Select(x => x)
                .Include(s=>s.ProductMedia)
                .Include(s=>s.ProductDetail)
                .OrderBy(x=>x.SpecialOrder).ToList();
            result.Data = specialProducts;
            if (specialProducts.Count == 0)
            {
                return NotFound(DataNotFound(result));
            }

            return Ok(result);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Product
                .Include(x=>x.ProductMedia)
                .Include(x=>x.Category)
                .Include(x=>x.ProdType)
                .Include(x=>x.ProductDetail)
                .FirstOrDefaultAsync(s=>s.ProdId == id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        [CheckModelFilter]
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutProduct(int id, ProductModel productModel)
        {
            var result = new Result<string>();
            Type prodType = typeof(Product);
            var updateProd = await _context.Product.Where(x=>x.ProdId == id).FirstOrDefaultAsync();
            if (updateProd == null)
            {
                return NotFound(DataNotFound(result));
            }
            UpdateTable(productModel,prodType,updateProd);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                result.IsSuccess = false;
                return BadRequest(result);
            }
            return Ok(result);
        }

        // POST: api/Products
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Product>> PostProduct(ProductModel productModel )
        {
            var result = new Result<Product>();

            Product product = new Product();
            _mapper.Map(productModel, product);
            product.CreateOn = DateTime.Now;
            product.IsActivate = 1;

            try
            {
                result.Data = product;
                await _context.Product.AddAsync(product);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                result.IsFound = false;
                return BadRequest(result);
            }

            return Ok(result);

        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
//            _context.Product.Remove(product);
            product.IsActivate = 0;
            // delete product images
            var imageList = await _context.ProductMedia.Where(x => x.ProdId == id).ToListAsync();
            if (imageList.Count() > 0)
            {
                foreach (var image in imageList)
                {
                    var path = Path.Combine("wwwroot", image.Url);
                    FileInfo file = new FileInfo(path); 
                    file.Delete();
                    _context.ProductMedia.Remove(image);
                }
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return product;
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProdId == id);
        }
    }
}
