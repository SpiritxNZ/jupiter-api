using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using jupiterCore.jupiterContext;
using jupiterCore.Models;
using Jupiter.ActionFilter;
using Jupiter.Controllers;
using Jupiter.Models;

namespace jupiterCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTypesController : BasicController
    {
        private readonly jupiterContext.jupiterContext _context;
        private readonly IMapper _mapper;

        public ProductTypesController(jupiterContext.jupiterContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/ProductTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductType>>> GetProductType()
        {
            return await _context.ProductType.
                Include(s=>s.Product).ThenInclude(s=>s.ProductMedia)
                .ToListAsync();
        }

        // GET: api/ProductTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductType>> GetProductType(int id)
        {
            var productType = await _context.ProductType
                .Include(s=>s.Product).ThenInclude(s=>s.ProductMedia)
                .FirstOrDefaultAsync(x=>x.ProdTypeId == id);
            if (productType == null)
            {
                return NotFound();
            }

            return productType;
        }

        // PUT: api/ProductTypes/5
        [CheckModelFilter]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductType(int id, ProductTypeModel productTypeModel)
        {
            var result = new Result<string>();
            Type typeType = typeof(ProductType);
            var updateType = await _context.Cart.Where(x=>x.CartId == id).FirstOrDefaultAsync();
            if (updateType == null)
            {
                return NotFound(DataNotFound(result));
            }
            UpdateTable(productTypeModel,typeType,updateType);
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

        // POST: api/ProductTypes
        [CheckModelFilter]
        [HttpPost]
        public async Task<ActionResult<ProductType>> PostProductType(ProductTypeModel productTypeModel)
        {
            var result = new Result<ProductType>();
            ProductType productType = new ProductType();
            _mapper.Map(productTypeModel, productType);
            try
            {
                result.Data = productType;
                await _context.ProductType.AddAsync(productType);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                result.IsFound = false;
            }
            return Ok(result);
        }

        // DELETE: api/ProductTypes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProductType>> DeleteProductType(int id)
        {
            var result = new Result<string>();
            var type = await _context.ProductType.FindAsync(id);
            if (type == null)
            {
                return NotFound(DataNotFound(result));
            }

            _context.ProductType.Remove(type);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                result.IsSuccess = false;
            }
            return Ok(result);
        }
    }
}
