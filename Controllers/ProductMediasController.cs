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
    public class ProductMediasController : BasicController
    {
        private readonly jupiterContext.jupiterContext _context;
        private readonly IMapper _mapper;

        public ProductMediasController(jupiterContext.jupiterContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/ProductMedias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductMedia>>> GetProductMedia()
        {
            return await _context.ProductMedia.ToListAsync();
        }

        // GET: api/ProductMedias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductMedia>> GetProductMedia(int id)
        {
            var productMedia = await _context.ProductMedia.FindAsync(id);

            if (productMedia == null)
            {
                return NotFound();
            }

            return productMedia;
        }

        // PUT: api/ProductMedias/5
        [CheckModelFilter]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductMedia(int id, ProductMediaModel productMediaMediaModel)
        {
            var result = new Result<string>();
            Type mediaType = typeof(ProductMedia);
            var updateMedia = await _context.ProductMedia.Where(x=>x.Id == id).FirstOrDefaultAsync();
            if (updateMedia == null)
            {
                return NotFound(DataNotFound(result));
            }
            UpdateTable(productMediaMediaModel,mediaType,updateMedia);
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

        // POST: api/ProductMedias
        [CheckModelFilter]
        [HttpPost]
        public async Task<ActionResult<ProductMedia>> PostProductMedia(ProductMediaModel productMediaModel)
        {
            var result = new Result<ProductMedia>();
            ProductMedia productMedia = new ProductMedia();
            _mapper.Map(productMediaModel, productMedia);
            try
            {
                result.Data = productMedia;
                await _context.ProductMedia.AddAsync(productMedia);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                result.IsFound = false;
            }
            return Ok(result);
        }

        // DELETE: api/ProductMedias/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProductMedia>> DeleteProductMedia(int id)
        {
            var result = new Result<string>();
            var media = await _context.ProductMedia.FindAsync(id);
            if (media == null)
            {
                return NotFound(DataNotFound(result));
            }

            _context.ProductMedia.Remove(media);
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
