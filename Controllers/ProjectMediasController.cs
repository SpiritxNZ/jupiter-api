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

namespace jupiterCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectMediasController : BasicController
    {
        private readonly jupiterContext.jupiterContext _context;
        private readonly IMapper _mapper;

        public ProjectMediasController(jupiterContext.jupiterContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/ProjectMedias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectMedia>>> GetProjectMedia()
        {
            return await _context.ProjectMedia.Include(x=>x.Project).ToListAsync();
        }

        // GET: api/ProjectMedias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectMedia>> GetProjectMedia(int id)
        {
            var projectMedia = await _context.ProjectMedia.Include(x=>x.Project).FirstOrDefaultAsync(s=>s.ProjectId==id);

            if (projectMedia == null)
            {
                return NotFound();
            }

            return projectMedia;
        }

        // PUT: api/ProjectMedias/5
        [CheckModelFilter]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProjectMedia(int id, ProjectMediaModel projectMediaModel)
        {
            var result = new Result<string>();
            Type projectMediaType = typeof(ProjectMedia);
            var updateProjectMedia = await _context.ProductMedia.Where(x=>x.Id == id).FirstOrDefaultAsync();
            if (updateProjectMedia == null)
            {
                return NotFound(DataNotFound(result));
            }
            UpdateTable(projectMediaModel,projectMediaType,updateProjectMedia);
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

        // POST: api/ProjectMedias
        [CheckModelFilter]
        [HttpPost]
        public async Task<ActionResult<ProjectMedia>> PostProjectMedia(ProjectMediaModel projectMediaModel)
        {
            var result = new Result<ProjectMedia>();
            ProjectMedia projectMedia = new ProjectMedia();
            _mapper.Map(projectMediaModel, projectMedia);
            try
            {
                result.Data = projectMedia;
                await _context.ProjectMedia.AddAsync(projectMedia);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                result.IsFound = false;
            }
            return Ok(result);
        }

        // DELETE: api/ProjectMedias/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProjectMedia>> DeleteProjectMedia(int id)
        {
            var result = new Result<string>();
            var projectMedia = await _context.ProjectMedia.FindAsync(id);
            if (projectMedia == null)
            {
                return NotFound();
            }

            _context.ProjectMedia.Remove(projectMedia);

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
            return projectMedia;
        }
    }
}
