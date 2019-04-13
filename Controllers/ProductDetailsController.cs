using System;
using System.Collections;
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
using Microsoft.AspNetCore.Authorization;

namespace jupiterCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductDetailsController : BasicController
    {
        private readonly jupiterContext.jupiterContext _context;
        private readonly IMapper _mapper;

        public ProductDetailsController(jupiterContext.jupiterContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/ProductDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDetail>>> GetProductDetail()
        {
            return await _context.ProductDetail.ToListAsync();
        }

        // GET: api/ProductDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<ProductDetail>>> GetProductDetail(int id)
        {
            var productDetail = await _context.ProductDetail.Where(x=>x.ProdId == id).Select(x=>x).ToListAsync();

            return productDetail;
        }

        // PUT: api/ProductDetails/5
        [CheckModelFilter]
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutProductDetail(int id, List<ProductDetailModel> productDetailModelList)
        {
            var result = new Result<string>();
            Type prodDetailType = typeof(ProductDetail);

            foreach (var detail in productDetailModelList)
            {
                if (detail.Id == 0)
                {
                    ProductDetail productDetail = new ProductDetail();
                    _mapper.Map(detail, productDetail);
                    await _context.ProductDetail.AddAsync(productDetail);
                }
                else
                {
                    var existProdDetail = await _context.ProductDetail.FirstOrDefaultAsync(x => x.Id == detail.Id);
                    UpdateTable(detail,prodDetailType,existProdDetail);
                }
            }

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

        // POST: api/ProductDetails
        [CheckModelFilter]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ProductDetail>> PostProductDetail(ProductDetailModel productDetailModel)
        {
            var result = new Result<ProductDetail>();

            ProductDetail productDetail = new ProductDetail();
            _mapper.Map(productDetailModel, productDetail);

            try
            {
                result.Data = productDetail;
                await _context.ProductDetail.AddAsync(productDetail);
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

        // DELETE: api/ProductDetails/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<ProductDetail>> DeleteProductDetail(int id)
        {
            var productDetail = await _context.ProductDetail.FindAsync(id);
            if (productDetail == null)
            {
                return NotFound();
            }

            _context.ProductDetail.Remove(productDetail);
            await _context.SaveChangesAsync();

            return productDetail;
        }

        private bool ProductDetailExists(int id)
        {
            return _context.ProductDetail.Any(e => e.Id == id);
        }
    }
}
