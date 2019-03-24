using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jupiter.Controllers;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace jupiterCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendEmailController : BasicController
    {
        // GET: api/SendEmail
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/SendEmail/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/SendEmail
        [HttpPost]
        public void Post()
        {
            var message = new MimeMessage();
            message.To.Add(new MailboxAddress("Destination@email.com"));
            message.From.Add(new MailboxAddress("Source@email.com"));
            message.Subject = "Testing MailKit";
            message.Body = new TextPart("plain")
            {
                Text = "The message from .Net Core"
            };

            using (var emailClient = new SmtpClient())
            {
                emailClient.Connect("smtp.126.com", 25, false);
                emailClient.Authenticate("Destination@email.com","password");
                emailClient.Send(message);
                emailClient.Disconnect(true);
            }
        }

        // PUT: api/SendEmail/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
