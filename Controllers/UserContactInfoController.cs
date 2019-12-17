using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Jupiter.Controllers;
using Jupiter.Models;
using jupiterCore.jupiterContext;
using jupiterCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace jupiterCore.Controllers
{

    [Route("api/[Controller]")]
    [ApiController]

    public class UserContactInfoController: BasicController
    {

        private readonly jupiterContext.jupiterContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserContactInfoController(jupiterContext.jupiterContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpPut]
        //[Authorize]
        //[ValidateAntiForgeryToken]
        [Route("MdifyInfo")]
        public async Task<ActionResult<UserContactInfo>> ModifyInfo([FromBody] UserContactInfoModel userContactInfoModel)
        {
            //var result = new Result<string>();
            var result = new Result<UserContactInfo>();
            //Type userType = typeof(User);
            var user = await _context.UserContactInfo.Where(x => x.UserId == userContactInfoModel.UserId).FirstOrDefaultAsync();
            if (user == null)
            {
                UserContactInfo userContactInfo = new UserContactInfo();
                _mapper.Map(userContactInfoModel, userContactInfo);
                await _context.UserContactInfo.AddAsync(userContactInfo);
                //await _context.SaveChangesAsync();
            }
            else
            {
                user.FirstName = userContactInfoModel.FirstName;
                user.LastName = userContactInfoModel.LastName;
                user.PhoneNumber = userContactInfoModel.PhoneNumber;
                //_mapper.Map(userContactInfoModel, user);
                _context.UserContactInfo.Update(user);
                //await _context.SaveChangesAsync();

            }
            try
            {
                //_context.Update(user);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = ex.ToString();
                return BadRequest(result);
            }

            result.IsSuccess = true;
            return Ok(result);
        }
    }
}
