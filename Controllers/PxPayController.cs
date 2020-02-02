using System;
using Jupiter.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PaymentExpress.PxPay;
using jupiterCore.jupiterContext;
using System.Linq;


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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PxPayUserId"></param>
        /// <param name="PxPayKey"></param>
        public PxPayController(IConfiguration configuration, jupiterContext.jupiterContext context)
        {
            _context = context;
            _configuration = configuration;
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
            //input.UrlCallback = "http://45.76.123.59:80/paymentresult";

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
            var producttimes = _context.ProductTimetable.Where(x => x.CartId == payment.CardId).ToList();



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
                cart.IsPay = 1;
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

    }
}
