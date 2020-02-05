using System;
using Jupiter.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PaymentExpress.PxPay;
using jupiterCore.jupiterContext;
using System.Linq;
using jupiterCore.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.EntityFrameworkCore;
using Jupiter.Models;
using System.Collections.Generic;
using AutoMapper;

namespace jupiterCore.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class PxPayController : BasicController
    {
        private string _PxPayUserId;
        private string _PxPayKey;
        private readonly IConfiguration _configuration;
        private readonly jupiterContext.jupiterContext _context;
        private readonly IMapper _mapper;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="PxPayUserId"></param>
        /// <param name="PxPayKey"></param>
        public PxPayController(IConfiguration configuration, jupiterContext.jupiterContext context, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
            _PxPayUserId = _configuration.GetSection("WindCave:PxPayUserId").Value;
            _PxPayKey = _configuration.GetSection("WindCave:PxPayKey").Value;
        }

        public class RequestJson
        {
            public string Url { get; set; }
        }

        [HttpGet]
        [Route("[action]")]
        public RequestJson RequestPaymentUrl(int cartId)
        {
            string PxPayUserId = _configuration.GetSection("WindCave:PxPayUserId").Value;//.AppSettings["PxPayUserId"];
            string PxPayKey = _configuration.GetSection("WindCave:PxPayKey").Value;

            PxPay WS = new PxPay(PxPayUserId, PxPayKey);

            RequestInput input = new RequestInput();
            var card = _context.Cart.Find(cartId);

            input.AmountInput = card.Price.ToString();
            input.CurrencyInput = "NZD";
            input.MerchantReference = "My Reference";
            input.TxnType = "Purchase";

            input.UrlFail = "http://45.76.123.59:80/paymentresult";
            input.UrlSuccess = "http://45.76.123.59:80/paymentresult";

            //input.UrlFail = "http://localhost:4240/paymentresult";
            //input.UrlSuccess = "http://localhost:4240/paymentresult";

            // TODO: GUID representing unique identifier for the transaction within the shopping cart (normally would be an order ID or similar)
            Guid orderId = Guid.NewGuid();
            input.TxnId = orderId.ToString().Substring(0, 16);
            Payment payment = new Payment();
            payment.TxnId = input.TxnId;
            payment.CardId = cartId;

            RequestOutput output = WS.GenerateRequest(input);

            if (output.valid == "1")
            {
                payment.url = output.Url;
                _context.Payment.AddAsync(payment);
                _context.SaveChangesAsync();
                return new RequestJson { Url = output.Url };
            }

            return new RequestJson { Url = output.Url };
        }


        public class Url
        {
            public string url { get; set; }
        }

        [HttpPost]
        [Route("[action]")]
        public ResponseOutput ResponseOutput(Url url)
        {
            string PxPayUserId = _configuration.GetSection("WindCave:PxPayUserId").Value;//.AppSettings["PxPayUserId"];
            string PxPayKey = _configuration.GetSection("WindCave:PxPayKey").Value;
            //string PxPayKey = ConfigurationManager.AppSettings["PxPayKey"];

            PxPay WS = new PxPay(PxPayUserId, PxPayKey);

            ResponseOutput response = WS.ProcessResponse(url.url);
            var payment = _context.Payment.Where(x => x.TxnId == response.TxnId).First();
            var cart = _context.Cart.Where(x => x.CartId == payment.CardId).First();

            var cartpord = _context.CartProd.Where(x => x.CartId == payment.CardId).ToList();
            var contact = _context.Contact.Where(x => x.ContactId == cart.ContactId).First();
            var producttimes = _context.ProductTimetable.Where(x => x.CartId == payment.CardId).ToList();


            List<CartProd> cartProd = new List<CartProd>();
            cartpord.ForEach(s => {
                cartProd.Add(new CartProd
                {
                    ProdId = s.ProdId,
                    Price = s.Price,
                    Title = s.Title,
                    Quantity = s.Quantity

                });
            });

            CartModel cartModel = new CartModel
            {
                Location = cart.Location,
                Price = cart.Price,
                //PlannedTime = (DateTime)cart.PlannedTime,
                CartProd = cartProd,
                Contact = contact,
            };



            payment.Success = int.Parse(response.Success);
            //payment.TxnId = response.TxnId;
            payment.ClientInfo = response.ClientInfo;
            payment.ResponseText = response.ResponseText;
            payment.AmountSettlemen = response.AmountSettlement;
            payment.cardName = response.CardName;
            payment.cardNumber = response.CardNumber;
            payment.dateExpiry = response.DateExpiry;
            payment.cardHolderName = response.CardHolderName;
            payment.currencySettlement = response.CurrencySettlement;
            payment.currencyInput = response.CurrencyInput;
            payment.txnMac = response.TxnMac;
            if (payment.Success == 1)
            {

                SendCartEmail(cartModel);

                cart.IsPay = 1;
                cart.CartStatusId = 1;
                foreach (var producttime in producttimes)
                {
                    producttime.IsActive = 1;
                    _context.ProductTimetable.Update(producttime);
                }
            }
            _context.Payment.Update(payment);
            _context.Cart.Update(cart);

            _context.SaveChanges();
            return response;

        }


        private void SendCartEmail(CartModel cartContactModel)
        {

            var sendgrid = _context.ApiKey.Find(1);
            var sendGridClient = new SendGridClient(sendgrid.ApiKey1);

            var myMessage = new SendGridMessage();

            myMessage.AddTo("Info@luxedreameventhire.co.nz");
            myMessage.AddTo(cartContactModel.Contact.Email);
            myMessage.From = new EmailAddress("Info@luxedreameventhire.co.nz", "LuxeDreamEventHire");
            myMessage.SetTemplateId("d-8b50f89729a24c0590fcee9ef8bee1fe");

            var contactDetail = cartContactModel.Contact;
            var cartDetail = cartContactModel;
            //return Ok(cartDetail.CartProd;
            //var cartProds = "";
            //foreach (var cart in cartDetail.CartProd)
            //{
            //    cartProds = cartProds + cart.Quantity + " of " + " " + cart.Title + "\r\n";
            //}

            myMessage.SetTemplateData(new
            {
                FirstName = contactDetail.FirstName,
                LastName = contactDetail.LastName,
                PlannedTime = cartDetail.PlannedTime.ToString("D"),
                Email = contactDetail.Email,
                PhoneNum = contactDetail.PhoneNum,
                //cartProds = cartProds,
                cartProds = cartDetail.CartProd,
                Message = contactDetail.Message
            });
            sendGridClient.SendEmailAsync(myMessage);


        }

    }
}
