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
using MailChimp.Net.Interfaces;
using MailChimp.Net;
using MailChimp.Net.Models;

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

        [HttpPut("{id}")]
        //[Authorize]
        //[ValidateAntiForgeryToken]
        //[Route("ModifyInfo")]
        public async Task<ActionResult<UserContactInfo>> ModifyInfo(int id,[FromBody] UserContactInfoModel userContactInfoModel)
        {
            //var result = new Result<string>();
            var result = new Result<UserContactInfo>();

            var user = await _context.UserContactInfo.Where(x => x.UserId == id).FirstOrDefaultAsync();
            var updateSubscribe = await _context.User.FirstAsync(x => x.Id == id);

            if (user == null)
            {
                UserContactInfo userContactInfo = new UserContactInfo();
                _mapper.Map(userContactInfoModel, userContactInfo);

                await _context.UserContactInfo.AddAsync(userContactInfo);
            }
            else
            {
                user.FirstName = userContactInfoModel.FirstName;
                user.LastName = userContactInfoModel.LastName;
                user.PhoneNumber = userContactInfoModel.PhoneNumber;
                user.Company = userContactInfoModel.Company;
                user.Comments = userContactInfoModel.Comments;
                _context.UserContactInfo.Update(user);
            }
            updateSubscribe.IsSubscribe = userContactInfoModel.IsSubscribe;
            updateSubscribe.Discount = userContactInfoModel.Discount;
            _context.User.Update(updateSubscribe);

            try
            {
                await UserSubscribe(userContactInfoModel);
                _context.SaveChanges();
                result.Data = user;
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

        private async Task<IActionResult> UserSubscribe(UserContactInfoModel userContactInfoModel)
        {
            var result = new Result<string>();
            var mailchimp = _context.ApiKey.Find(2);

            IMailChimpManager mailChimpManager = new MailChimpManager(mailchimp.ApiKey1);
            var listId = "c8326de226";
            var user = await _context.User.FirstAsync(x => x.Id == userContactInfoModel.UserId);
            var members = await mailChimpManager.Members.GetAllAsync(listId).ConfigureAwait(false);
            var member = members.First(x => x.EmailAddress == user.Email);

            // Use the Status property if updating an existing member
            member.MergeFields.Clear();
            member.MergeFields.Add("FNAME", userContactInfoModel.FirstName);
            member.MergeFields.Add("LNAME", userContactInfoModel.LastName);
            if (userContactInfoModel.IsSubscribe == 0)
            {
                member.Status = Status.Unsubscribed;
            }
            else if (userContactInfoModel.IsSubscribe == 1)
            {
                member.Status = Status.Subscribed;
            }
            
            await mailChimpManager.Members.AddOrUpdateAsync(listId, member);
            return Ok(result);
        }

    }
}
