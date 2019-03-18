using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using Jupiter.ActionFilter;
using Jupiter.Models;
//using JupiterEntity;
using Microsoft.AspNetCore.Mvc;

namespace Jupiter.Controllers
{   
    [ApiController]
    public class CartController : BaseController
    {
        private readonly IMapper _mapper;
        // GET api/values
        public CartController(IMapper Mapper)
        {
            _mapper = Mapper;
        }
        public IActionResult Get()
        {
            var result = new Result<List<CartModel>>();
            using (var db = new jupiterEntities())
            {
                List<Cart> carts = db.Carts.ToList();
                List<CartModel> cartModels = new List<CartModel>();
                _mapper.Map(carts, cartModels);
                result.Data = cartModels;
                return Json(result);
            }

        }
        // GET api/values/5
        public IActionResult Get(int id)
        {
            var result = new Result<CartModel>();
            using (var db = new jupiterEntities())
            {
                var carts = db.Carts.Where(x => x.CartId == id).Select(x =>x).FirstOrDefault();
                if (carts == null)
                {
                    return Json(DataNotFound(result));
                }
                CartModel cartModel = new CartModel();
                Mapper.Map(carts, cartModel);
                result.Data = cartModel;
                return Json(result);
            }
        }

        // POST api/values
        [CheckModelFilter]
        public IActionResult Post([FromBody]CartModel newCart)
        {
            var result = new Result<string>();
            using (var db = new jupiterEntities())
            {
                Cart newDb = new Cart();

                Mapper.Map(newCart, newDb);
                try
                {
                    db.Carts.Add(newDb);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    result.ErrorMessage = e.Message;
                    result.IsSuccess = false;
                }
                return Json(result);
            }
        }

        // PUT api/values/5
        [CheckModelFilter]
        public IActionResult Put(int id, [FromBody]CartModel cartModel)
        {
            var result = new Result<string>();
            Type cartType = typeof(Cart);
            using (var db = new jupiterEntities())
            {
                Cart updated = db.Carts.Where(x => x.CartId == id).Select(x => x).FirstOrDefault();
                if (updated == null)
                {
                    return Json(DataNotFound(result));
                }
                UpdateTable(cartModel,cartType,updated);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    result.ErrorMessage = e.Message;
                    result.IsSuccess = false;
                    return Json(result);
                }
                return Json(result);
            }
        }
        // DELETE api/values/5
        public IActionResult Delete(int id)
        {
            var result = new Result<string>();
            using (var db = new jupiterEntities())
            {
                Cart del = db.Carts.Where(x => x.CartId == id).Select(x => x).FirstOrDefault();
                if (del == null)
                {
                    return Json(DataNotFound(result));
                }
                try
                {
                    del.IsActivate = 0;
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    result.ErrorMessage = e.Message;
                    result.IsSuccess = false;
                }
                return Json(result);
            }

        }
    }
}