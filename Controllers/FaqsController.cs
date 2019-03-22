using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using jupiterCore.jupiterContext;
using Jupiter.Controllers;

namespace jupiterCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaqsController : BasicController
    {
        private readonly jupiterContext.jupiterContext _context;

        public FaqsController(jupiterContext.jupiterContext context)
        {
            _context = context;
        }

        // GET: api/Faqs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Faq>>> GetFaq()
        {
            return await _context.Faq.ToListAsync();
        }

        // GET: api/Faqs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Faq>> GetFaq(int id)
        {
            var faq = await _context.Faq.FindAsync(id);

            if (faq == null)
            {
                return NotFound();
            }

            return faq;
        }

        // PUT: api/Faqs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFaq(int id, Faq faq)
        {
            if (id != faq.Id)
            {
                return BadRequest();
            }

            _context.Entry(faq).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FaqExists(id))
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

        // POST: api/Faqs
        [HttpPost]
        public async Task<ActionResult<Faq>> PostFaq(Faq faq)
        {
            _context.Faq.Add(faq);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FaqExists(faq.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFaq", new { id = faq.Id }, faq);
        }

        // DELETE: api/Faqs/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Faq>> DeleteFaq(int id)
        {
            var faq = await _context.Faq.FindAsync(id);
            if (faq == null)
            {
                return NotFound();
            }

            _context.Faq.Remove(faq);
            await _context.SaveChangesAsync();

            return faq;
        }

        private bool FaqExists(int id)
        {
            return _context.Faq.Any(e => e.Id == id);
        }
    }
}
