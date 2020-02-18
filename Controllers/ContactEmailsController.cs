using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
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
using MimeKit;
using MimeKit.Utils;
using SendGrid;
using SendGrid.Helpers.Mail;
using Attachment = SendGrid.Helpers.Mail.Attachment;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using MailChimp.Net.Interfaces;
using MailChimp.Net;
using MailChimp.Net.Models;

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

        public class CustomMessageResult
        {
            public string email { get; set; }
            public string phoneNumber { get; set; }
            public string findUs { get; set; }
            public string coupon { get; set; }
        }

        [CheckModelFilter]
        [HttpPost("[action]")]
        public async Task<ActionResult<CustomMessageResult>> CustomerMessage(ContactUsModel contactUsModel)
        {
            var result = new Result<CustomMessageResult>();
            ContactEmail contactEmail = new ContactEmail();
            _mapper.Map(contactUsModel, contactEmail);
            await _context.ContactEmail.AddAsync(contactEmail);

            try
            {
                Guid coupon = Guid.NewGuid();
                Popup newPopUp = new Popup
                {
                    Coupon = coupon.ToString().Substring(0, 8),
                    IsValid = 1
                };
                await _context.Popups.AddAsync(newPopUp);
                SendEmailCustomer(contactUsModel,newPopUp.Coupon);
                await _context.SaveChangesAsync();
                CustomMessageResult messageResult = new CustomMessageResult
                {
                    email = contactEmail.Email,
                    phoneNumber = contactEmail.PhoneNumber,
                    findUs = contactEmail.FindUs,
                    coupon = newPopUp.Coupon
                };
                await UserSubscribe(contactEmail.Email);
                
                result.Data = messageResult;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                result.IsFound = false;
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<ContactEmail>> GetAllCustomerMessage()
        {
            return await _context.ContactEmail.Where(x => x.Name == null).ToListAsync();
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

        private void SendEmail(ContactEmailModel contactEmailModel)
        {
            var sendgrid = _context.ApiKey.Find(1);
            var sendGridClient = new SendGridClient(sendgrid.ApiKey1);

            var myMessage = new SendGridMessage();

            myMessage.AddTo("Info@luxedreameventhire.co.nz");
            myMessage.From = new EmailAddress("Info@luxedreameventhire.co.nz", "LuxeDreamEventHire");
            myMessage.SetTemplateId("d-fa12e602e09041339338a5869708e195");
            myMessage.SetTemplateData(new
            {
                Name = contactEmailModel.Name,
                Email = contactEmailModel.Email,
                PhoneNumber = contactEmailModel.PhoneNumber,
                Company = contactEmailModel.Company,
                DateOfEvent = contactEmailModel.DateOfEvent.ToString("D"),
                LocationOfEvent = contactEmailModel.LocationOfEvent,
                FindUs = contactEmailModel.FindUs,
                TypeOfEvent = contactEmailModel.TypeOfEvent,
                Message = contactEmailModel.Message
            });


            var messageToCustomer = new SendGridMessage();


            messageToCustomer.AddTo(contactEmailModel.Email);
            messageToCustomer.From = new EmailAddress("Info@luxedreameventhire.co.nz", "LuxeDreamEventHire");
            messageToCustomer.SetTemplateId("d-2978c7005cdb4dfd8bd47ab5e8257094");
            messageToCustomer.SetTemplateData(new
            {
                Name = contactEmailModel.Name,
                Email = contactEmailModel.Email,
                PhoneNumber = contactEmailModel.PhoneNumber,
                Company = contactEmailModel.Company,
                DateOfEvent = contactEmailModel.DateOfEvent.ToString("D"),
                LocationOfEvent = contactEmailModel.LocationOfEvent,
                FindUs = contactEmailModel.FindUs,
                TypeOfEvent = contactEmailModel.TypeOfEvent,
                Message = contactEmailModel.Message
            });


            sendGridClient.SendEmailAsync(myMessage);
            sendGridClient.SendEmailAsync(messageToCustomer);

        }


        private void SendEmailCustomer (ContactUsModel contactUsModel,string coupon)
        {
            var sendgrid = _context.ApiKey.Find(1);
            var sendGridClient = new SendGridClient(sendgrid.ApiKey1);

            var myMessage = new SendGridMessage();

            myMessage.AddTo(contactUsModel.Email);
            myMessage.From = new EmailAddress("Info@luxedreameventhire.co.nz", "LuxeDreamEventHire");
            myMessage.SetTemplateId("d-1df8a6bcabad4f4fa62d98a4941cd08c");
            myMessage.SetTemplateData(new
            {
                Email = contactUsModel.Email,
                PhoneNumber = contactUsModel.PhoneNumber,
                FindUs = contactUsModel.FindUs,
                Coupon = coupon,

            });
            sendGridClient.SendEmailAsync(myMessage);
        }

        private async Task<IActionResult> UserSubscribe(string email)
        {
            var result = new Result<string>();
            var mailchimp = _context.ApiKey.Find(2);
            
            IMailChimpManager mailChimpManager = new MailChimpManager(mailchimp.ApiKey1);
            var listId = "c8326de226";
            var members = await mailChimpManager.Members.GetAllAsync(listId).ConfigureAwait(false);
            var member = members.FirstOrDefault(x => x.EmailAddress == email);
            if (null == member)
            {
                var newMember = new Member { EmailAddress = email, StatusIfNew = Status.Subscribed };
                await mailChimpManager.Members.AddOrUpdateAsync(listId, newMember);
            }

            return Ok(result);
        }
    }


}
