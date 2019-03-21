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
    public class ProjectsController : BasicController
    {
        private readonly jupiterContext.jupiterContext _context;
        private readonly IMapper _mapper;

        public ProjectsController(jupiterContext.jupiterContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Projects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProject()
        {
            return await _context.Project.Include(s=>s.Eventtype).Include(s=>s.ProjectMedia).ToListAsync();
        }

        // GET: api/Projects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var project = await _context.Project.Include(s=>s.Eventtype).Include(s=>s.ProjectMedia).FirstOrDefaultAsync(x=>x.ProdjectId == id);

            if (project == null)
            {
                return NotFound();
            }

            return project;
        }

        // PUT: api/Projects/5
        [CheckModelFilter]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(int id, ProjectModel projectModel)
        {
            var result = new Result<string>();
            Type projectType = typeof(ProjectModel);
            var updateProject = await _context.Project.Where(x=>x.ProdjectId == id).FirstOrDefaultAsync();
            if (updateProject == null)
            {
                return NotFound(DataNotFound(result));
            }
            UpdateTable(projectModel,projectType,updateProject);
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

        // POST: api/Projects
        [CheckModelFilter]
        [HttpPost]
        public async Task<ActionResult<Project>> PostProject(ProjectModel projectModel)
        {
            var result = new Result<Project>();
            Project project = new Project();
            _mapper.Map(projectModel, project);
            try
            {
                result.Data = project;
                await _context.Project.AddAsync(project);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                result.IsFound = false;
            }
            return Ok(result);
        }

        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Project>> DeleteProject(int id)
        {
            var result = new Result<string>();
            var project = await _context.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            _context.Project.Remove(project);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                result.IsSuccess = false;
            }
            return project;
        }
    }
}
