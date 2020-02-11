using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using jupiterCore.jupiterContext;
using jupiterCore.Models;
using Jupiter.ActionFilter;
using Jupiter.Controllers;
using Jupiter.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using MimeKit;
using MimeKit.Utils;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace jupiterCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserCartsController : BasicController
    {
        private readonly jupiterContext.jupiterContext _context;
        private readonly IMapper _mapper;

        public UserCartsController(jupiterContext.jupiterContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Carts
        [HttpGet]
        //[Authorize]
        public ActionResult<List<Cart>> GetCart()
        {
            var cartsValue = _context.Cart.Where(x => x.IsActivate == 1 && x.UserId != null).Include(s => s.Contact).Include(s => s.CartProd).OrderByDescending(x=>x.EventStartDate).ToList();
            return Ok(cartsValue);
        }

        // GET: api/Carts/5
        [HttpGet("{userId}")]
        //[Authorize]
        public ActionResult GetCart(int userId)
        {
            var cart1 = _context.Cart.Include(s => s.Contact).Include(s => s.CartProd).Include(s=>s.CartStatus)
                .Where(s => s.UserId == userId);
            return Ok(cart1);
        }

        // PUT: api/Carts/5
        [CheckModelFilter]
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> PutCart(int id, CartModel cartModel)
        {
            var result = new Result<string>();
            Type cartType = typeof(Cart);
            var updateCart = await _context.Cart.Where(x => x.CartId == id).FirstOrDefaultAsync();
            if (updateCart == null)
            {
                return NotFound(DataNotFound(result));
            }
            UpdateTable(cartModel, cartType, updateCart);
            updateCart.UpdateOn = DateTime.Now;
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

        // POST: api/Carts
        [CheckModelFilter]
        [HttpPost("{id}")]
        public async Task<ActionResult<Cart>> PostCart(int id,CartContactModel cartContactModel)
        {

            var result = new Result<Cart>();
            try
            {
                //SendCartEmail(cartContactModel);
            }
            catch (Exception e)
            {
                result.ErrorMessage = $@"Sending Email Failed, {e.Message}";
                result.IsSuccess = false;
                return BadRequest(result);
            }
            Contact contact = new Contact();
            _mapper.Map(cartContactModel.ContactModel, contact);
            try
            {
                await _context.Contact.AddAsync(contact);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                result.IsSuccess = false;
                return BadRequest(result);
            }

            Cart cart = new Cart();
            _mapper.Map(cartContactModel.CartModel, cart);
            cart.CreateOn = toNZTimezone(DateTime.UtcNow);
            cart.IsActivate = 1;
            cart.IsPay = 0;
            cart.IsEmailSend = 0;
            cart.ContactId = contact.ContactId;
            cart.Location = cartContactModel.CartModel.Location;
            cart.Price = cartContactModel.CartModel.Price;
            //cart.RentalPaidFee = cartContactModel.CartModel.Price * 0.50m;
            cart.RentalPaidFee = 0;
            cart.DepositPaidFee = 0;
            cart.EventStartDate = cartContactModel.CartModel.EventStartDate;
            cart.EventEndDate = cartContactModel.CartModel.EventEndDate;
            cart.IsPickup = cartContactModel.CartModel.IsPickup;
            cart.Region = cartContactModel.CartModel.Region;           
            cart.UserId = id;
            cart.IsExpired = 0;

            try
            {
                await _context.Cart.AddAsync(cart);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                result.IsFound = false;
                return BadRequest(result);
            }
            cartContactModel.ProductTimetableModel.ToList().ForEach(s => {
                _context.ProductTimetable.Add(new ProductTimetable {
                    ProdDetailId = s.ProdDetailId,
                    ProdId = s.ProdId,
                    BeginDate = s.BeginDate,
                    EndDate = s.EndDate,
                    Quantity = s.Quantity,
                    CartId = cart.CartId,
                    IsActive = 1,
                    //IsExpired = 0,
                });
            });
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
            result.Data = cart;
            return Ok(result);
        }

        // DELETE: api/Carts/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<Cart>> DeleteCart(int id)
        {
            var result = new Result<string>();
            var cart = await _context.Cart.FindAsync(id);
            if (cart == null)
            {
                return NotFound(DataNotFound(result));
            }
            cart.IsActivate = 0;
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

        //private void SendCartEmail(CartContactModel cartContactModel)
        //{

        //    var sendgrid = _context.ApiKey.Find(1);
        //    var sendGridClient = new SendGridClient(sendgrid.ApiKey1);

        //    var myMessage = new SendGridMessage();


        //    //Todo:Uncommand when need to send email

        //    //myMessage.AddTo("Info@luxedreameventhire.co.nz");
        //    //myMessage.AddTo(cartContactModel.ContactModel.Email);
        //    myMessage.From = new EmailAddress("Info@luxedreameventhire.co.nz", "LuxeDreamEventHire");
        //    myMessage.SetTemplateId("d-8b50f89729a24c0590fcee9ef8bee1fe");

        //    var contactDetail = cartContactModel.ContactModel;
        //    var cartDetail = cartContactModel.CartModel;
        //    //return Ok(cartDetail.CartProd;
        //    //var cartProds = "";
        //    //foreach (var cart in cartDetail.CartProd)
        //    //{
        //    //    cartProds = cartProds + cart.Quantity + " of " + " " + cart.Title + "\r\n";
        //    //}

        //    myMessage.SetTemplateData(new
        //    {
        //        FirstName = contactDetail.FirstName,
        //        LastName = contactDetail.LastName,
        //        PlannedTime = cartDetail.PlannedTime.ToString("D"),
        //        Email = contactDetail.Email,
        //        PhoneNum = contactDetail.PhoneNum,
        //        //cartProds = cartProds,
        //        cartProds = cartDetail.CartProd,
        //        Message = contactDetail.Message
        //    });
        //    sendGridClient.SendEmailAsync(myMessage);


        //}
    }
}
