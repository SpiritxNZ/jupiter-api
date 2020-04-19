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
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using MailChimp;
using MailChimp.Net.Models;
using MailChimp.Net.Interfaces;
using MailChimp.Net;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Claims;

namespace jupiterCore.Controllers
{

    [Route("api/[Controller]")]
    [ApiController]
    public class UserController : BasicController
    {
        private readonly jupiterContext.jupiterContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private string _pswKey;

        public UserController(jupiterContext.jupiterContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
            _pswKey = _configuration.GetSection("PasswordKey").Value;
        }

        public class JsonResult
        {
            public int userId { get; set; }
            public string email { get; set; }
            public string token { get; set; }
        }

        public class ForgotPasswordModel
        {
            public string Email { get; set; }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            var result = new Result<object>();

            var user = _context.User.Include(x => x.UserContactInfo).Select(s=>new { s.Id,s.Email,s.IsSubscribe,s.CreatedOn,s.Discount,UserInfo=s.UserContactInfo}).OrderByDescending(x=>x.CreatedOn).ToListAsync();
            result.Data = user;

            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserByid(int id)
        {
            Result<Object> result = new Result<Object>();

            var user = await _context.User.Include(x => x.UserContactInfo).Where(x => x.Id == id).Select(s => new { s.Id, s.Email, s.IsSubscribe,s.CreatedOn,s.Discount, UserInfo = s.UserContactInfo }).ToListAsync();
            result.Data = user;
            return Ok(result);
        }



        [HttpPut]
        [Route("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel changePasswordModel)
        {
            var result = new Result<string>();
            try
            {
                var user = _context.User.FirstOrDefault(s => s.Email == changePasswordModel.email);
                if (user == null)
                {
                    throw new Exception("email does not exist");
                }

                if (user.Password != HashPassword(changePasswordModel.oldPassword))
                {
                    result.IsSuccess = false;
                    result.ErrorMessage = "The old password is incorrect";
                    return StatusCode(401, result);
                }

                if (user.Password == HashPassword(changePasswordModel.newPassword))
                {
                    throw new Exception("The new password is same as the old password");
                }

                user.Password = HashPassword(changePasswordModel.newPassword);
                _context.Update(user);
                await _context.SaveChangesAsync();
                result.Data = "success";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = ex.Message;
                return BadRequest(result);
            }

            return Ok(result);
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
            var tokenString = GenerateJwt(user.Id);

//            var callbackUrl = Url.Action("EmailResetPassword", "User", new { userId = user.Id, code = tokenToClient }, protocol: HttpContext.Request.Scheme);
            var callbackUrl = "http://45.76.123.59:80/reset?" + "code=" + tokenString;
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
        [Route("ResetPassword")]
        public IActionResult EmailResetPassword(ResetPasswordModel resetPasswordModel)
        {
            var userId = int.Parse(User.Claims.First(s => s.Type == "id").Value);
            var result = new Result<string>();
            Type userType = typeof(User);
            var user = _context.User.FirstOrDefault(x => x.Id == userId);
            if (user != null)
            {
                user.Password = HashPassword(resetPasswordModel.Password);
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
        public async Task<IActionResult> Register([FromBody] UserModel userModel)
        {
            var result = new Result<Object>();

            var user = _context.User.FirstOrDefault(x => x.Email == userModel.Email);
            if (user != null)
            {
                result.IsSuccess = false;
                result.ErrorMessage = "This email has been registered.";
                return BadRequest(result);
            }
            User newUser = new User();
            userModel.CreatedOn = DateTime.Now;
            _mapper.Map(userModel, newUser);
            newUser.Password = HashPassword(userModel.Password);
            try
            {
                await UserSubscribe(userModel);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = userModel.Email + " looks fake or invalid";
                return BadRequest(result);
            }
            await _context.User.AddAsync(newUser);

            UserContactInfo userContactInfo = new UserContactInfo();
            userContactInfo.UserId = newUser.Id;
            await _context.UserContactInfo.AddAsync(userContactInfo);
            await _context.SaveChangesAsync();
            var tokenString = GenerateJwt(newUser.Id);
            result.Data = new JsonResult { userId = newUser.Id, email = newUser.Email, token = tokenString };
            SendRegisterEmail(userModel);
            //result.Data = newUser;
            return Ok(result);
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            var result1 = new Result<Object>();
            var user = _context.User.FirstOrDefault(x => x.Email == loginModel.Email);
            if (user == null)
            {
                throw new Exception( "Email not exist");
            }
            //bad password
            if (user.Password != HashPassword(loginModel.Password))
            {
                throw new Exception("Password is wrong");
            }
            string result = ValidateUser(loginModel);
            if(result == "Success")
            {
                var tokenString = GenerateJwt(user.Id);
                result1.Data = new JsonResult { userId = user.Id, email = user.Email, token = tokenString };
                return Ok(result1);
            }
            else
            {
                return Unauthorized(result);
            }
            
        }

        private async Task<IActionResult> UserSubscribe(UserModel userModel)
        {
            var result = new Result<string>();
            var mailchimp = _context.ApiKey.Find(2);

            IMailChimpManager mailChimpManager = new MailChimpManager(mailchimp.ApiKey1);
            var listId = "c8326de226";

            var members = await mailChimpManager.Members.GetAllAsync(listId).ConfigureAwait(false);
            var member = members.FirstOrDefault(x => x.EmailAddress == userModel.Email);
            if (null == member)
            {
                if (userModel.IsSubscribe == 0)
                {
                    var newMember = new Member { EmailAddress = userModel.Email, StatusIfNew = Status.Unsubscribed };
                    await mailChimpManager.Members.AddOrUpdateAsync(listId, newMember);
                }
                else if (userModel.IsSubscribe == 1)
                {
                    var newMember = new Member { EmailAddress = userModel.Email, StatusIfNew = Status.Subscribed };
                    await mailChimpManager.Members.AddOrUpdateAsync(listId, newMember);
                }
            }
            
            
            return Ok(result);
        }

        private string GenerateJwt(int id)
        {
            var claims = new[]
            {
                new Claim("id",id.ToString())
            };
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var expiry = DateTime.Now.AddMinutes(1);
            var securityKey = new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials
                (securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(issuer,
                audience,
                claims,
                expires: DateTime.Now.AddMinutes(1),
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
            if (user.Password != HashPassword(loginModel.Password))
            {
                return "Password is wrong";
            }

            return "Success";
        }


        private string HashPassword(string password)
        {
            byte[] salt = new byte[128 / 8];

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));
            return hashed;
        }

        private void SendRegisterEmail(UserModel userModel)
        {

            var sendgrid = _context.ApiKey.Find(1);
            var sendGridClient = new SendGridClient(sendgrid.ApiKey1);

            var myMessage = new SendGridMessage();

            myMessage.AddTo(userModel.Email);
            myMessage.From = new EmailAddress("Info@luxedreameventhire.co.nz", "LuxeDreamEventHire");
            myMessage.SetTemplateId("d-79bbe53d102749fcbec110be88a29b85");

            myMessage.SetTemplateData(new
            {
               Email = userModel.Email,
               Password = userModel.Password,

            });
            sendGridClient.SendEmailAsync(myMessage);


        }

    }
}
