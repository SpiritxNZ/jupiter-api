using System;
using AutoMapper;
using Jupiter.Controllers;
using Microsoft.AspNetCore.Mvc;
using jupiterCore.Models;
using Jupiter.Models;
using jupiterCore.jupiterContext;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace jupiterCore.Controllers
{

    [Route("api/[Controller]")]
    [ApiController]
    public class UserController : BasicController
    {
        private readonly jupiterContext.jupiterContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserController(jupiterContext.jupiterContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        public class JsonResult
        {
            public int userId { get; set; }
            public string email { get; set; }
            public string token { get; set; }
        }

        //[HttpPost]
        //[Route("ForgotPassword")]
        //public IActionResult ForgotPassword(string email)
        //{
        //    var result = new Result<Object>();
        //    var user = _context.User.FirstOrDefault(x => x.Email == email);
        //    if (user == null)
        //    {
        //        throw new Exception("Email not exist");
        //    }
            
        //    var tokenString = GenerateJwt(email);

        //    result.Data = new JsonResult { Email = user.Email, Token = tokenString };
        //    return Ok(result);
        //}


        public class ForgotPasswordModel
        {
            public string Email { get; set; }
        }
        [HttpPost]
        [Route("ForgotPassword")]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword ([FromBody]ForgotPasswordModel email)
        {
            var result = new Result<Object>();
            var user = _context.User.Select(x=>new { x.Email, x.Id}).FirstOrDefault(x => x.Email == email.Email);

            if (user == null)
            {
                result.IsSuccess = false;
                result.IsFound = false;
                result.Data = "This user does not exist.";
                return Ok(result);
            }
            var tokenString = GenerateJwt(user.Email);
            //var callbackUrl = Url.Action("EmailResetPassword", "User", new { userId = user.Id, code = tokenString }, protocol: HttpContext.Request.Scheme);
            var callbackUrl = "http://luxedreameventhire.co.nz/" + "userId="+user.Id + "code="+tokenString;
            var sendgrid = _context.ApiKey.Find(1);
            var sendGridClient = new SendGridClient(sendgrid.ApiKey1);

            var myMessage = new SendGridMessage();

            myMessage.AddTo(user.Email);
            myMessage.From = new EmailAddress("Info@luxedreameventhire.co.nz", "LuxeDreamEventHire");
            myMessage.Subject = "ResetPassword";
            myMessage.HtmlContent = $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>";
            await sendGridClient.SendEmailAsync(myMessage);

            result.Data = new JsonResult { userId = user.Id, email = user.Email, token = tokenString };
            return Ok(result);
        }

        [HttpPut]
        [Authorize]
        //[ValidateAntiForgeryToken]
        [Route("EmailResetPassword")]
        public IActionResult EmailResetPassword(ResetPasswordModel resetPasswordModel)
        {
            var result = new Result<string>();
            Type userType = typeof(User);
            var user = _context.User.FirstOrDefault(x => x.Id == resetPasswordModel.userId);
            if (user != null)
            {
                user.Password = resetPasswordModel.Password;
            }
            try
            {
                _context.Update(user);
                _context.SaveChanges();
            }
            catch(Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = ex.ToString();
                return BadRequest(result);
            }
            
            result.IsSuccess = true;
            return Ok(result);
        }


        [HttpPost]
        [Route("register")]
        public IActionResult Register([FromBody] UserModel userModel)
        {
            var result = new Result<User>();

            var user = _context.User.FirstOrDefault(x => x.Email == userModel.Email);
            if (user != null)
            {
                return Ok("This email has been registed.");
            }
            User newUser = new User();
            _mapper.Map(userModel, newUser);
            result.Data = newUser;
            _context.User.AddAsync(newUser);
            _context.SaveChangesAsync();

            return Ok(result);
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            var user = _context.User.FirstOrDefault(x => x.Email == loginModel.Email);
            if (user == null)
            {
                throw new Exception( "Email not exist");
            }
            //bad password
            if (user.Password != loginModel.Password)
            {
                throw new Exception("Password is wrong");
            }
            string result = ValidateUser(loginModel);
            if(result == "Success")
            {
                var tokenString = GenerateJwt(user.Email);
                return Ok(new { token = tokenString });
            }
            else
            {
                return Unauthorized(result);
            }
            
        }



        private string GenerateJwt(string email)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email,email)
            };
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var expiry = DateTime.Now.AddMinutes(120);
            var securityKey = new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials
                (securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(issuer: issuer,
                audience: audience,
                claims:claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            var stringToken = tokenHandler.WriteToken(token);
            return stringToken;
        }

        private string ValidateUser(LoginModel loginModel)
        {
            var user = _context.User.FirstOrDefault(x => x.Email == loginModel.Email);
            //cannot find user
            if (user == null)
            {
                return "Email not exist";
            }
            //bad password
            if (user.Password != loginModel.Password)
            {
                return "Password is wrong";
            }

            return "Success";
        }

    }
}
