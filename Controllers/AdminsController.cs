using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jupiterCore.Models;
using Jupiter.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace jupiterCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController : BasicController
    {
        private readonly IConfiguration _configuration;
        private readonly jupiterContext.jupiterContext _context;

        public AdminsController(IConfiguration configuration, jupiterContext.jupiterContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        // GET: api/Users
        [Authorize]
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value3", "value4" };
        }

        // GET: api/Users/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Users
        [HttpPost]
        public IActionResult  Login(AdminModel adminModel)
        {

            string result = ValidateUser(adminModel);

            if (result == "Success")
            {
                var tokenString = GenerateJwt();
                return Ok(new { token = tokenString });
            }
            else
            {
                return Unauthorized(result);
            }
        }

        [Authorize]
        [HttpPost("[action]")]
        public Boolean HaveAccess()
        {
            return true;
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        private string GenerateJwt()
        {
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var expiry = DateTime.Now.AddMinutes(120);
            var securityKey = new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials
                (securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(issuer: issuer, 
                audience:audience,
                expires: DateTime.Now.AddMinutes(120), 
                signingCredentials: credentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            var stringToken = tokenHandler.WriteToken(token);
            return stringToken;
        }

        private string ValidateUser(AdminModel adminModel)
        {
            var admin = _context.Admin.FirstOrDefault(x => x.Username == adminModel.UserName);
            //cannot find user
            if (admin == null)
            {
                return "Username not exist";
            }
            //bad password
            if (admin.Password != adminModel.Password)
            {
                return "Password is wrong";
            }

            return "Success";
        }

    }
}
