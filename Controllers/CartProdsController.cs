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
using Newtonsoft.Json.Linq;

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
            var cartProdValue = await _context.CartProd.Include(s => s.Cart).Include(s => s.Prod).ToListAsync();
            return cartProdValue ;
        }

        // GET: api/CartProds/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CartProd>> GetCartProd(int id)
        {
            var cartProd = await _context.CartProd.Include(s => s.Cart).Include(s => s.Prod).FirstOrDefaultAsync(x=>x.Id == id);

            if (cartProd == null)
            {
                return NotFound();
            }

            return cartProd;
        }

        //GET:api/CartProds
        [Route("[action]/{id}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartProd>>> GetCartProdByCart(int id)
        {
            var cartProdValue = await _context.CartProd
                .Where(x=>x.CartId == id)
                .Select(x=>x)
                .ToListAsync();
            return cartProdValue ;

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
        public async Task<ActionResult> PostCartProd(IEnumerable<CartProdModel> cartProdModelList)
        {
            var result = new Result<string>();
            var cart = await _context.Cart.Where(x => x.CartId == cartProdModelList.ToArray()[0].CartId).FirstOrDefaultAsync();
            if(cart == null)
            {
                return NotFound(DataNotFound(result));
            }
 
            cartProdModelList.ToList().ForEach(async s => {
                _context.CartProd.Add(new CartProd
                {
                    ProdDetailId = s.ProdDetailId,
                    ProdId = s.ProdId,
                    Quantity = s.Quantity,
                    CartId = s.CartId,
                    Price = s.Price,
                    Title = s.Title,
                });
                cart.Price += s.Price;
                if (s.ProdDetailId != null)
                {
                    var cardprod = await _context.CartProd.Where(x => x.CartId == s.CartId && x.ProdId == s.ProdId && x.ProdDetailId == s.ProdDetailId).FirstOrDefaultAsync();
                    if (cardprod != null)
                    {
                        result.IsSuccess = false;
                        result.ErrorMessage = "This product already exist in this cart.";
                    }
                    else
                    {
                        _context.ProductTimetable.Add(new ProductTimetable
                        {
                            ProdDetailId = s.ProdDetailId,
                            Quantity = s.Quantity,
                            CartId = s.CartId,
                            IsActive = 1,
                            BeginDate = cart.EventStartDate,
                            EndDate = cart.EventEndDate
                        });
                    }
                   
                }
                else
                {
                    var cardprod = await _context.CartProd.Where(x => x.CartId == s.CartId && x.ProdId == s.ProdId).FirstOrDefaultAsync();
                    if (cardprod != null)
                    {
                        result.IsSuccess = false;
                        result.ErrorMessage = "This product already exist in this cart.";
                    }
                    else
                    {
                        _context.ProductTimetable.Add(new ProductTimetable
                        {
                            ProdId = s.ProdId,
                            Quantity = s.Quantity,
                            CartId = s.CartId,
                            IsActive = 1,
                            BeginDate = cart.EventStartDate,
                            EndDate = cart.EventEndDate
                        });
                    }
                   
                }
                
            });

            try
            {
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

            var cart = await _context.Cart.Where(x => x.CartId == cartProd.CartId).FirstOrDefaultAsync();
;
            if (cartProd.ProdDetailId == null)
            {
                var time = await _context.ProductTimetable.Where(x => x.CartId == cart.CartId && x.ProdId == cartProd.ProdId).FirstOrDefaultAsync();
                time.IsActive = 0;
                _context.ProductTimetable.Remove(time);
            }
            else
            {
                var time = await _context.ProductTimetable.Where(x => x.CartId == cart.CartId && x.ProdDetailId == cartProd.ProdDetailId).FirstOrDefaultAsync();
                time.IsActive = 0;
                _context.ProductTimetable.Remove(time);
            }
            if (cartProd == null || cart==null)
            {
                return NotFound(DataNotFound(result));
            }
            cart.Price = cart.Price - cartProd.Price * cartProd.Quantity;
            _context.Cart.Update(cart);
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
