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
    //TODO: TEST ALL THE CONTROLLERS
    [Route("api/[controller]")]
    [ApiController]
    public class EventTypesController : BasicController
    {
        private readonly jupiterContext.jupiterContext _context;
        private readonly IMapper _mapper;

        public EventTypesController(jupiterContext.jupiterContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/EventTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventType>>> GetEventType()
        {
            var EventTypeValue = await _context.EventType.Include(x => x.Project)
                    .Include(x => x.Testimonial).ToListAsync();
            return EventTypeValue;
        }

        // GET: api/EventTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EventType>> GetEventType(int id)
        {
            var eventType = await _context.EventType.Include(x => x.Project)
                .Include(x => x.Testimonial).FirstOrDefaultAsync(s => s.TypeId == id);

            if (eventType == null)
            {
                return NotFound();
            }

            return eventType;
        }

        // PUT: api/EventTypes/5
        [CheckModelFilter]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEventType(int id, EventTypeModel eventTypeModel)
        {
            var result = new Result<string>();
            Type eType = typeof(EventType);
            var updateEventType = await _context.EventType.Where(x => x.TypeId == id).FirstOrDefaultAsync();
            if (updateEventType == null)
            {
                return NotFound(DataNotFound(result));
            }
            UpdateTable(eventTypeModel, eType,updateEventType);
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

        // POST: api/EventTypes
        [CheckModelFilter]
        [HttpPost]
        public async Task<ActionResult<EventType>> PostEventType(EventTypeModel eventTypeModel)
        {
            var result = new Result<EventType>();
            EventType eventType = new EventType();
            _mapper.Map(eventTypeModel, eventType);
            try
            {
                result.Data = eventType;
                await _context.EventType.AddAsync(eventType);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                result.IsFound = false;
            }
            return Ok(result);
        }

        // DELETE: api/EventTypes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<EventType>> DeleteEventType(int id)
        {
            var result = new Result<string>();
            var eventType = await _context.EventType.FindAsync(id);
            if (eventType == null)
            {
                return NotFound(DataNotFound(result));
            }

            _context.EventType.Remove(eventType);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                result.IsSuccess = false;
            }
            return Ok(result);
        }

        //private bool EventTypeExists(int id)
        //{
        //    return _context.EventType.Any(e => e.TypeId == id);
        //}
    }
}
