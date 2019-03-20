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
    public class CartProdsController : BasicController
    {
        private readonly jupiterContext.jupiterContext _context;
        private readonly IMapper _mapper; 

        public CartProdsController(jupiterContext.jupiterContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/CartProds
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartProd>>> GetCartProd()
        {
            return await _context.CartProd.ToListAsync();
        }

        // GET: api/CartProds/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CartProd>> GetCartProd(int id)
        {
            var cartProd = await _context.CartProd.FindAsync(id);

            if (cartProd == null)
            {
                return NotFound();
            }

            return cartProd;
        }

        // PUT: api/CartProds/5
        [CheckModelFilter]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCartProd(int id, CartProdModel cartProdModel)
        {
            var result = new Result<string>();
            Type cartProdType = typeof(CartProd);
            var updateCartProd = await _context.CartProd.Where(x=>x.Id == id).FirstOrDefaultAsync();
            if (updateCartProd == null)
            {
                return NotFound(DataNotFound(result));
            }
            UpdateTable(cartProdModel,cartProdType,updateCartProd);
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

        // POST: api/CartProds
        [CheckModelFilter]
        [HttpPost]
        public async Task<ActionResult<CartProd>> PostCartProd(CartProdModel cartProdModel)
        {
            var result = new Result<CartProd>();
            CartProd cartProd = new CartProd();
            _mapper.Map(cartProdModel, cartProd);
            try
            {
                result.Data = cartProd;
                await _context.CartProd.AddAsync(cartProd);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                result.IsFound = false;
            }
            return Ok(result);
        }

        // DELETE: api/CartProds/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CartProd>> DeleteCartProd(int id)
        {
            var result = new Result<string>();
            var cartProd = await _context.CartProd.FindAsync(id);
            if (cartProd == null)
            {
                return NotFound(DataNotFound(result));
            }
            _context.CartProd.Remove(cartProd);
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
