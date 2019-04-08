using System;
using System.Collections.Generic;
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

namespace jupiterCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoriesController : BasicController
    {
        private readonly jupiterContext.jupiterContext _context;
        private readonly IMapper _mapper;

        public ProductCategoriesController(jupiterContext.jupiterContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/ProductCategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductCategory>>> GetProductCategory()
        {
            return await _context.ProductCategory.Include(x=>x.Product).ThenInclude(s=>s.ProductMedia).ToListAsync();
        }

        // GET: api/ProductCategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductCategory>> GetProductCategory(int id)
        {
            var productCategory = await _context.ProductCategory.Include(x=>x.Product).ThenInclude(s=>s.ProductMedia).FirstOrDefaultAsync(s=>s.CategoryId==id);

            if (productCategory == null)
            {
                return NotFound();
            }

            if (productCategory.Product !=null && productCategory.Product.Count()!=0)
            {
                foreach (var prod in productCategory.Product.ToList())
                {
                    if (prod.IsActivate == 0)
                    {
                        productCategory.Product.Remove(prod);
                    }
                }
            }

            return productCategory;
        }

        // PUT: api/ProductCategories/5
        //[CheckModelFilter]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductCategory(int id, ProductCategoryModel productCategoryModel)
        {
            var result = new Result<string>();
            Type prodCateType = typeof(ProductCategory);
            var updateCate = await _context.ProductCategory.Where(x=>x.CategoryId == id).FirstOrDefaultAsync();
            if (updateCate == null)
            {
                return NotFound(DataNotFound(result));
            }
            UpdateTable(productCategoryModel,prodCateType,updateCate);
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

        // POST: api/ProductCategories
        [CheckModelFilter]
        [HttpPost]
        public async Task<ActionResult<ProductCategory>> PostProductCategory(ProductCategoryModel productCategoryModel)
        {
            var result = new Result<ProductCategory>();
            ProductCategory productCategory = new ProductCategory();
            _mapper.Map(productCategoryModel, productCategory);
            try
            {
                result.Data = productCategory;
                await _context.ProductCategory.AddAsync(productCategory);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                result.IsFound = false;
            }
            return Ok(result);
        }

        // DELETE: api/ProductCategories/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<ProductCategory>> DeleteProductCategory(int id)
        //{
        //    var productCategory = await _context.ProductCategory.FindAsync(id);
        //    if (productCategory == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.ProductCategory.Remove(productCategory);
        //    await _context.SaveChangesAsync();

        //    return productCategory;
        //}
    }
}
