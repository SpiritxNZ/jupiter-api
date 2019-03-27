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
                SendEmail();
                //await _context.SaveChangesAsync();
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

        public void SendEmail()
        {
            var message = new MimeMessage();
            message.To.Add(new MailboxAddress("lgx9587@email.com"));
            message.From.Add(new MailboxAddress("lgx2500@126.com"));
            message.Subject = "Testing MailKit";
            message.Body = new TextPart("plain")
            {
                Text = "Testing Email."
            };

            using (var emailClient = new SmtpClient())
            {
                emailClient.Connect("smtp.126.com", 25, false);
                emailClient.Authenticate("lgx2500@126.com","password");
                emailClient.Send(message);
                emailClient.Disconnect(true);
            }
        }
    }
}
