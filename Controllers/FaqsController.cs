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
using Microsoft.AspNetCore.Authorization;

namespace jupiterCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaqsController : BasicController
    {
        private readonly jupiterContext.jupiterContext _context;
        private readonly IMapper _mapper;

        public FaqsController(jupiterContext.jupiterContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> PutFaq(int id, FaqModel faqModel)
        {
            var result = new Result<Faq>();
            Type faqType = typeof(Faq);
            var updateFaq = await _context.Faq.Where(x=>x.Id == id).FirstOrDefaultAsync();
            if (updateFaq == null)
            {
                return NotFound(DataNotFound(result));
            }
            UpdateTable(faqModel,faqType,updateFaq);
            result.Data = updateFaq;
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

        // POST: api/Faqs
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Faq>> PostFaq(FaqModel faqModel)
        {
            var result = new Result<Faq>();
            Faq faq = new Faq();
            _mapper.Map(faqModel, faq);
            try
            {
                result.Data = faq;
                await _context.Faq.AddAsync(faq);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                result.IsFound = false;
            }
            return Ok(result);
        }

        // DELETE: api/Faqs/5
        [HttpDelete("{id}")]
        [Authorize]
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
