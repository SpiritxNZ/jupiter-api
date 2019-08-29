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

namespace jupiterCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : BasicController
    {
        private readonly jupiterContext.jupiterContext _context;
        private readonly IMapper _mapper;

        public CartsController(jupiterContext.jupiterContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Carts
        [HttpGet]
        [Authorize]
        public ActionResult<List<Cart>> GetCart()
        {
            var cartsValue = _context.Cart.Where(x=>x.IsActivate==1).Include(s => s.Contact).Include(s => s.CartProd).ToList();
            return Ok(cartsValue);
        }

        // GET: api/Carts/5
        [HttpGet("{id}")]
        [Authorize]

        public ActionResult GetCart(int id)
        {
            var cart1 =  _context.Cart.Include(s => s.Contact).Include(s => s.CartProd)
                .FirstOrDefault(s => s.CartId == id);
            return Ok(cart1);
        }

        // PUT: api/Carts/5
        [CheckModelFilter]
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> PutCart(int id,  CartModel cartModel)
        {
            var result = new Result<string>();
            Type cartType = typeof(Cart);
            var updateCart = await _context.Cart.Where(x=>x.CartId == id).FirstOrDefaultAsync();
            if (updateCart == null)
            {
                return NotFound(DataNotFound(result));
            }
            UpdateTable(cartModel,cartType,updateCart);
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
        [HttpPost]
        public async Task<ActionResult<Cart>> PostCart(CartContactModel cartContactModel)
        {

            var result = new Result<Cart>();
            try
            {
                SendCartEmail(cartContactModel);
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
            catch(Exception e)
            {
                result.ErrorMessage = e.Message;
                result.IsSuccess = false;
                return BadRequest(result);
            }

            Cart cart = new Cart();
            _mapper.Map(cartContactModel.CartModel, cart);
            cart.CreateOn = DateTime.Now;
            cart.IsActivate = 1;
            cart.ContactId = contact.ContactId;

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

        public void SendCartEmail(CartContactModel cartContactModel)
        {
            var message = new MimeMessage();
            message.To.Add(new MailboxAddress(cartContactModel.ContactModel.Email));
            message.To.Add(new MailboxAddress("luxedreameventhire@gmail.com"));
            message.From.Add(new MailboxAddress("LuxeDreamEventHire","luxecontacts94@gmail.com"));
            message.Subject = "Luxe Dream Event Hire Customer Email";
            var builder = new BodyBuilder();
            var contactDetail = cartContactModel.ContactModel;
            var cartDetail = cartContactModel.CartModel;

            var pathImage = Path.Combine("wwwroot", "Images", "Icon","Icon.png");
            var image = builder.LinkedResources.Add(pathImage);
            image.ContentId = MimeUtils.GenerateMessageId ();

            var cartProds = "";
            foreach (var cart in cartDetail.CartProd)
            {
                cartProds = cartProds +cart.Quantity+ " of " + " " + cart.Title + "<br>";
            }

            builder.HtmlBody =
                $@"Hi {contactDetail.FirstName} {contactDetail.LastName}<br><br>Thank you for ordering at Luxe Dream Event Hire.<br><br>
                Your planned event date is: {cartDetail.PlannedTime:D}<br>
                Your email address: {contactDetail.Email}<br>
                Your Phone Number: {contactDetail.PhoneNum}<br><br>
                Your ordered items are:<br><br>{cartProds}<br>
                Your Message: {contactDetail.Message}<br><br>
                Please let us know if you would like to change your order.<br><br>
                We will be in touch very shortly.<br><br>
                Many thanks<br>
                <br>
<div style=""display:inline-block;border-right:1.5px solid #e6e6e6;padding-right:10px;float:left;"">
<b>Emma</b><br> <span style=""font-size:12px;"">Luxe Dream Event Hire</span><br><br>
<b style=""color: #c48f45;"">E </b> <span style=""font-size:11px;"">luxedreameventhire@gmail.com</span><br>
<b style=""color: #c48f45;"">P </b> <span style=""font-size:11px;"">(64) 2108793899</span><br>
<span style=""font-size:11px;"">http://luxedreameventhire.co.nz</span>
</div>
<div style=""display:inline-block; margin-left:16px;"">
<img src = ""cid:{image.ContentId}"">
</div>
";
            message.Body = builder.ToMessageBody ();

            using (var emailClient = new SmtpClient())
            {
                emailClient.Connect("smtp.gmail.com", 587, false);
                emailClient.Authenticate("luxecontacts94@gmail.com","luxe1234");
                emailClient.Send(message);
                emailClient.Disconnect(true);
            }
        }
    }
}
