using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Jupiter.Controllers;
using Jupiter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PaymentExpress.PxPay;
using jupiterCore.jupiterContext;
using jupiterCore.Models;
using System.Linq;

namespace jupiterCore.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class PxPayController : BasicController
    {
        //public IConfiguration Configuration { get; }
        //private string _WebServiceUrl = ConfigurationManager.AppSettings["WindCave:PaymentExpress.PxPay"];   //.AppSettings["PaymentExpress.PxPay"];
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


        [HttpPost]
        [Route("[action]")]
        public string RequestPaymentUrl(int cartId)
        {
            string PxPayUserId = _configuration.GetSection("WindCave:PxPayUserId").Value;//.AppSettings["PxPayUserId"];
            string PxPayKey = _configuration.GetSection("WindCave:PxPayKey").Value;
            //string PxPayKey = ConfigurationManager.AppSettings["PxPayKey"];

            PxPay WS = new PxPay(PxPayUserId, PxPayKey);

            RequestInput input = new RequestInput();
            var card = _context.Cart.Find(cartId);


            input.AmountInput = card.Price.ToString();
            input.CurrencyInput = "NZD";
            input.MerchantReference = "My Reference";
            input.TxnType = "Purchase";

            //input.UrlFail = "https://demo.windcave.com/SandboxSuccess.aspx";
            //input.UrlSuccess = "https://demo.windcave.com/SandboxSuccess.aspx";
            //input.UrlFail = Request.Url.GetLeftPart(UriPartial.Path);
            //input.UrlSuccess = Request.Url.GetLeftPart(UriPartial.Path);


            //input.UrlFail = "http://www.gradspace.org:80/paymentresult";
            //input.UrlSuccess = "http://www.gradspace.org:80/paymentresult";


            input.UrlFail = "http://45.76.123.59:80/paymentresult";
            input.UrlSuccess = "http://45.76.123.59:80/paymentresult";

            // TODO: GUID representing unique identifier for the transaction within the shopping cart (normally would be an order ID or similar)
            Guid orderId = Guid.NewGuid();
            input.TxnId = orderId.ToString().Substring(0, 16);
            //input.TxnId = "123456123123123";
            Payment payment = new Payment();
            payment.TxnId = input.TxnId;
            payment.CardId = cartId;

            RequestOutput output = WS.GenerateRequest(input);

            if (output.valid == "1")
            {
                // Redirect user to payment page

                //Response.Redirect(output.Url);
                payment.url = output.Url;
                _context.Payment.AddAsync(payment);
                _context.SaveChangesAsync();
                return output.Url;
            }

            return output.Url;
            //PxPay WS = new PxPay(PxPayUserId, PxPayKey);

            //RequestInput input = new RequestInput();

            //input.AmountInput = "123";
            //input.CurrencyInput = "123";
            //input.MerchantReference = "123";
            //input.TxnType = "123";
            //input.UrlFail = Request.Url.GetLeftPart(UriPartial.Path);
            //input.UrlSuccess = Request.Url.GetLeftPart(UriPartial.Path);

            //// TODO: GUID representing unique identifier for the transaction within the shopping cart (normally would be an order ID or similar)
            //Guid orderId = Guid.NewGuid();
            //input.TxnId = orderId.ToString().Substring(0, 16);

            //RequestOutput output = WS.GenerateRequest(input);

            //if (output.valid == "1")
            //{
            //    // Redirect user to payment page

            //    Response.Redirect(output.Url);
            //}
        }


        public class Url
        {
            public string url { get; set; }
        }

        [HttpGet]
        [Route("[action]")]
        public ResponseOutput ResponseOutput(string url)
        {
            string PxPayUserId = _configuration.GetSection("WindCave:PxPayUserId").Value;//.AppSettings["PxPayUserId"];
            string PxPayKey = _configuration.GetSection("WindCave:PxPayKey").Value;
            //string PxPayKey = ConfigurationManager.AppSettings["PxPayKey"];

            PxPay WS = new PxPay(PxPayUserId, PxPayKey);
            
            ResponseOutput response = WS.ProcessResponse(url);
            var payment = _context.Payment.Where(x => x.TxnId == response.TxnId).First();
            var cart = _context.Cart.Where(x => x.CartId == payment.CardId).First();
            var producttimes = _context.ProductTimetable.Where(x => x.CartId == payment.CardId).ToList();


            
            payment.Success= int.Parse(response.Success);
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
                foreach(var producttime in producttimes)
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

        

        //[HttpGet]
        //public string checkpay()
        //{
        //    //HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create("https://sec.paymentexpress.com/pxmi3/EF4054F622D6C4C1B78D12B0DD6083868DEF04F43C26F5542A2797505507C04BF622922849AEF0087");

        //    // Create a request for the URL. 		
        //    WebRequest request = WebRequest.Create("http://www.gradspace.org:5005/paymentresult?result=00001200025586540cd5f2bd37039e56&userid=LuxeDreamEventHire_Dev");
        //    // If required by the server, set the credentials.
        //    request.Credentials = CredentialCache.DefaultCredentials;
        //    request.Method = "GET";
        //    // Get the response.
        //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //    // Display the status.
        //    Console.WriteLine(response.StatusDescription);
        //    // Get the stream containing content returned by the server.
        //    Stream dataStream = response.GetResponseStream();
        //    // Open the stream using a StreamReader for easy access.
        //    StreamReader reader = new StreamReader(dataStream);
        //    // Read the content.
        //    string responseFromServer = reader.ReadToEnd();
        //    // Display the content.

        //    // Cleanup the streams and the response.
        //    reader.Close();
        //    dataStream.Close();
        //    response.Close();
        //    return responseFromServer;
        //}
    }
}
