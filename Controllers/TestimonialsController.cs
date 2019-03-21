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
    public class TestimonialsController : BasicController
    {
        private readonly jupiterContext.jupiterContext _context;
        private readonly IMapper _mapper;

        public TestimonialsController(jupiterContext.jupiterContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Testimonials
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Testimonial>>> GetTestimonial()
        {
            return await _context.Testimonial.Include(s=>s.Eventtype).ToListAsync();
        }

        // GET: api/Testimonials/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Testimonial>> GetTestimonial(int id)
        {
            var testimonial = await _context.Testimonial.Include(s=>s.Eventtype).FirstOrDefaultAsync(x=>x.TestimonialId==id);

            if (testimonial == null)
            {
                return NotFound();
            }

            return testimonial;
        }

        // PUT: api/Testimonials/5
        [CheckModelFilter]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTestimonial(int id, TestimonialModel testimonialModel)
        {
            var result = new Result<string>();
            Type testType = typeof(Testimonial);
            var updateTest = await _context.Testimonial.Where(x=>x.TestimonialId == id).FirstOrDefaultAsync();
            if (updateTest == null)
            {
                return NotFound(DataNotFound(result));
            }
            UpdateTable(testimonialModel,testType,updateTest);
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

        // POST: api/Testimonials
        [CheckModelFilter]
        [HttpPost]
        public async Task<ActionResult<Testimonial>> PostTestimonial(TestimonialModel testimonialModel)
        {
            var result = new Result<Testimonial>();
            Testimonial testimonial = new Testimonial();
            _mapper.Map(testimonialModel, testimonial);
            try
            {
                result.Data = testimonial;
                await _context.Testimonial.AddAsync(testimonial);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                result.IsFound = false;
            }
            return Ok(result);
        }

        // DELETE: api/Testimonials/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Testimonial>> DeleteTestimonial(int id)
        {
            var result = new Result<string>();
            var testimonial = await _context.Testimonial.FindAsync(id);
            if (testimonial == null)
            {
                return NotFound();
            }

            _context.Testimonial.Remove(testimonial);
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

            return testimonial;
        }
    }
}
