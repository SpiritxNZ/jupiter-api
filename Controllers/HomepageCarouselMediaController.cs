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
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

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
            //var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            string fileNameForStorage = Guid.NewGuid().ToString();
            try
            {
                //add image name to db
                HomepageCarouselMedia homepageCarouselMedia = new HomepageCarouselMedia
                {
                    ImageUrl = $@"Images/HomepageCarouselImages/{fileNameForStorage}"
                };
                await _context.HomepageCarouselMedia.AddAsync(homepageCarouselMedia);
                await _context.SaveChangesAsync();

                var bucketName = "luxe_media";
                GoogleCredential credential = null;
                using (var jsonStream = new FileStream("secrect.json", FileMode.Open,
                    FileAccess.Read, FileShare.Read))
                {
                    credential = GoogleCredential.FromStream(jsonStream);
                }
                var storageClient = StorageClient.Create(credential);

                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    await storageClient.UploadObjectAsync(bucketName, $@"wwwroot/Images/ProductImages/{fileNameForStorage}", "image/jpeg", memoryStream);
                }

                result.Data = $@"{fileNameForStorage} successfully uploaded";
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