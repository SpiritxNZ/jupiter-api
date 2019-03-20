using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using jupiterCore.jupiterContext;
using Jupiter.ActionFilter;
using Jupiter.Controllers;
using Jupiter.Models;

namespace jupiterCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : BasicController
    {
        private readonly jupiterContext.jupiterContext _context;
        private readonly IMapper _mapper;

        public ContactsController(jupiterContext.jupiterContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Contacts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetContact()
        {
            return await _context.Contact.ToListAsync();
        }

        // GET: api/Contacts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetContact(int id)
        {
            var contact = await _context.Contact.FindAsync(id);

            if (contact == null)
            {
                return NotFound();
            }

            return contact;
        }

        // PUT: api/Contacts/5
        [CheckModelFilter]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContact(int id, ContactModel contactModel)
        {
            var result = new Result<string>();
            Type conType = typeof(Contact);
            var updateContact = await _context.Contact.Where(x=>x.ContactId == id).FirstOrDefaultAsync();
            if (updateContact == null)
            {
                return NotFound(DataNotFound(result));
            }
            UpdateTable(contactModel,conType,updateContact);
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

        // POST: api/Contacts
        [CheckModelFilter]
        [HttpPost]
        public async Task<ActionResult<Contact>> PostContact(ContactModel contactModel)
        {
            var result = new Result<Contact>();
            Contact contact = new Contact();
            _mapper.Map(contactModel, contact);
            try
            {
                result.Data = contact;
                await _context.Contact.AddAsync(contact);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                result.IsFound = false;
            }
            return Ok(result);
        }

        // DELETE: api/Contacts/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<Contact>> DeleteContact(int id)
        //{
        //    var contact = await _context.Contact.FindAsync(id);
        //    if (contact == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Contact.Remove(contact);
        //    await _context.SaveChangesAsync();

        //    return contact;
        //}
    }
}
