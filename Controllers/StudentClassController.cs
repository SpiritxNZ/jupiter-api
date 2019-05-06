using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using jupiterCore.jupiterContext;
using jupiterCore.Models;
using Jupiter.ActionFilter;
using Jupiter.Controllers;
using Jupiter.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace jupiterCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentClassController : BasicController
    {
        private readonly jupiterContext.jupiterContext _context;

        public StudentClassController(jupiterContext.jupiterContext context)
        {
            _context = context;
        }
        // POST: api/StudentClass
        [HttpPost]
        [CheckModelFilter]
        public async Task<ActionResult<ClassModel>> PostClass (IEnumerable<ClassModel> classModel)
        {
            var result = new Result<IEnumerable<ClassModel>>();
            result.Data = classModel;
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var productValue = await _context.Product
                .Include(x=>x.ProductMedia)
                .Include(x=>x.Category)
                .Include(x=>x.ProdType)
                .Include(x=>x.ProductDetail)
                .FirstOrDefaultAsync(s=>s.ProdId == id);
            return productValue;
        }
    }
}
