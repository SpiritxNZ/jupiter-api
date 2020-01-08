using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml;
using Jupiter.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PaymentExpress.PxPay;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PxPayUserId"></param>
        /// <param name="PxPayKey"></param>
        public PxPayController(IConfiguration configuration)
        {
            _configuration = configuration;
            _PxPayUserId = _configuration.GetSection("WindCave:PxPayUserId").Value;
            _PxPayKey = _configuration.GetSection("WindCave:PxPayKey").Value;
        }


        [HttpPost]
        public string Button1_Click()
        {
            string PxPayUserId = _configuration.GetSection("WindCave:PxPayUserId").Value;//.AppSettings["PxPayUserId"];
            string PxPayKey = _configuration.GetSection("WindCave:PxPayKey").Value;
            //string PxPayKey = ConfigurationManager.AppSettings["PxPayKey"];

            PxPay WS = new PxPay(PxPayUserId, PxPayKey);

            RequestInput input = new RequestInput();

            input.AmountInput = "123";
            input.CurrencyInput = "NZD";
            input.MerchantReference = "My Reference";
            input.TxnType = "Purchase";
            input.UrlFail = "https://demo.windcave.com/SandboxSuccess.aspx";
            input.UrlSuccess = "https://demo.windcave.com/SandboxSuccess.aspx";
            //input.UrlFail = Request.Url.GetLeftPart(UriPartial.Path);
            //input.UrlSuccess = Request.Url.GetLeftPart(UriPartial.Path);

            // TODO: GUID representing unique identifier for the transaction within the shopping cart (normally would be an order ID or similar)
            Guid orderId = Guid.NewGuid();
            input.TxnId = orderId.ToString().Substring(0, 16);
            //input.TxnId = "123456123123123";

            RequestOutput output = WS.GenerateRequest(input);

            if (output.valid == "1")
            {
                // Redirect user to payment page

                //Response.Redirect(output.Url);
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

       
    }
}
