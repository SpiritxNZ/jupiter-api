using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Jupiter.Controllers;
using Jupiter.Models;
using jupiterCore.jupiterContext;
using jupiterCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;

namespace jupiterCore.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class HomepageCarouselMediaController : BasicController
    {
        private readonly jupiterContext.jupiterContext _context;
        private readonly IMapper _mapper;

        public HomepageCarouselMediaController(jupiterContext.jupiterContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<List<HomepageCarouselMedia>> GetHomepageCarouselMedias()
        {
            return await _context.HomepageCarouselMedia.ToListAsync();
        }

        [HttpPost]
        public async Task<IActionResult> PostHomepageCarouselMedia([FromForm] HomepageCarouselMediaModel homepageCarouselMediaModel)
        {
            var requestForm = Request.Form;
            var file = requestForm.Files[0];
            var result = new Result<string>();
            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            // fileName = RemoveWhitespace(fileName);
            try
            {
                // add image
                bool isStoreSuccess = await StoreImage("HomepageCarouselImages", fileName, file);
                if (!isStoreSuccess)
                {
                    throw new Exception("Store image locally failed.");
                }

                //add image name to db
                HomepageCarouselMedia homepageCarouselMedia = new HomepageCarouselMedia
                {
                    ImageUrl = $@"Images/HomepageCarouselImages/{fileName}"
                };
                await _context.HomepageCarouselMedia.AddAsync(homepageCarouselMedia);
                await _context.SaveChangesAsync();

                result.Data = $@"{fileName} successfully uploaded";
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                return BadRequest(result);
            }
            return Ok(result);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHomepageCarouselMedia (int id)
        {
            var result = new Result<string>();
            var data = await _context.HomepageCarouselMedia.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (data == null)
            {
                result.Data = "Image not found";
                return NotFound(result);
            }

            try
            {
                DeleteImage(data.ImageUrl);
                _context.HomepageCarouselMedia.Remove(data);
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

    }
}