using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using jupiterCore.jupiterContext;
using jupiterCore.Models;
using Jupiter.ActionFilter;
using Jupiter.Controllers;
using Jupiter.Models;
using MailKit.Net.Smtp;
using MimeKit;

namespace jupiterCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactEmailsController : BasicController
    {
        private readonly jupiterContext.jupiterContext _context;
        private readonly IMapper _mapper;

        public ContactEmailsController(jupiterContext.jupiterContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/ContactEmails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactEmail>>> GetContactEmail()
        {
            return await _context.ContactEmail.ToListAsync();
        }

        // GET: api/ContactEmails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ContactEmail>> GetContactEmail(int id)
        {
            var contactEmail = await _context.ContactEmail.FindAsync(id);

            if (contactEmail == null)
            {
                return NotFound();
            }

            return contactEmail;
        }

        // PUT: api/ContactEmails/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContactEmail(int id, ContactEmail contactEmail)
        {
            if (id != contactEmail.Id)
            {
                return BadRequest();
            }

            _context.Entry(contactEmail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactEmailExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ContactEmails
        [CheckModelFilter]
        [HttpPost]
        public async Task<ActionResult<ContactEmail>> PostContactEmail(ContactEmailModel contactEmailModel)
        {
            var result = new Result<ContactEmail>();
            ContactEmail contactEmail = new ContactEmail();
            _mapper.Map(contactEmailModel, contactEmail);
            result.Data = contactEmail;
            await _context.ContactEmail.AddAsync(contactEmail);

            try
            {
                SendEmail(contactEmailModel);
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
        // DELETE: api/ContactEmails/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ContactEmail>> DeleteContactEmail(int id)
        {
            var contactEmail = await _context.ContactEmail.FindAsync(id);
            if (contactEmail == null)
            {
                return NotFound();
            }

            _context.ContactEmail.Remove(contactEmail);
            await _context.SaveChangesAsync();

            return contactEmail;
        }

        private bool ContactEmailExists(int id)
        {
            return _context.ContactEmail.Any(e => e.Id == id);
        }

        public void SendEmail(ContactEmailModel contactEmailModel)
        {
            var message = new MimeMessage();
            message.To.Add(new MailboxAddress("luxedreameventhire@gmail.com"));
            message.From.Add(new MailboxAddress("LuxeDreamEventHire","luxecontacts94@gmail.com"));
            message.Subject = "New Customer Email";
            var builder = new BodyBuilder();
            builder.HtmlBody = $@"<b>Name: </b>{contactEmailModel.Name}<br><b>Email: </b>{contactEmailModel.Email}<br>
<b>Phone Number: </b>{contactEmailModel.PhoneNumber}<br><b>Company: </b>{contactEmailModel.Company}<br>
                <b>DateOfEvent: </b>{contactEmailModel.DateOfEvent}<br><b>LocationOfEvent: </b>{contactEmailModel.LocationOfEvent}<br>
<b>How to find us: </b>{contactEmailModel.FindUs}<br><b>Type of event: </b>{contactEmailModel.TypeOfEvent}<br>
<b>Message: </b>{contactEmailModel.Message}";
            message.Body = builder.ToMessageBody();


            var messageToCustomer = new MimeMessage();
            messageToCustomer.To.Add(new MailboxAddress(contactEmailModel.Email));
            messageToCustomer.From.Add(new MailboxAddress("LuxeDreamEventHire","luxecontacts94@gmail.com"));
            messageToCustomer.Subject = "Thanks for your contact Email";
            var builderCustomer = new BodyBuilder();
            builderCustomer.HtmlBody = $@"We have received your email.<br>Your contact details are as followings.<br><b>Name: </b>{contactEmailModel.Name}<br><b>Email: </b>{contactEmailModel.Email}<br>
<b>Phone Number: </b>{contactEmailModel.PhoneNumber}<br><b>Company: </b>{contactEmailModel.Company}<br>
                <b>DateOfEvent: </b>{contactEmailModel.DateOfEvent}<br><b>LocationOfEvent: </b>{contactEmailModel.LocationOfEvent}<br>
<b>How to find us: </b>{contactEmailModel.FindUs}<br><b>Type of event: </b>{contactEmailModel.TypeOfEvent}<br>
<b>Message: </b>{contactEmailModel.Message}<br>
We will get in touch with you as soon as possible.<br><br>
                Many thanks<br>
                Emma, Luxe Dream Event Hire";
            messageToCustomer.Body = builderCustomer.ToMessageBody ();

            using (var emailClient = new SmtpClient())
            {
                emailClient.Connect("smtp.gmail.com", 587, false);
                emailClient.Authenticate("luxecontacts94@gmail.com","luxe1234");
                emailClient.Send(message);
                emailClient.Send(messageToCustomer);
                emailClient.Disconnect(true);
            }
        }
    }
}
