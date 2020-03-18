using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Jupiter.Controllers;
using Jupiter.Models;
using jupiterCore.jupiterContext;
using jupiterCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace jupiterCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectTypeImageController : BasicController
    {
        private readonly jupiterContext.jupiterContext _context;
        public ProjectTypeImageController(jupiterContext.jupiterContext context)
        {
            _context = context;
        }
        // Get all images of all event types
        // GET: api/ProjectTypeImage
        [HttpGet]
        public async Task<ActionResult<List<EventType>>> Get()
        {
            return await _context.EventType.ToListAsync();
        }

        // GET: api/ProjectTypeImage/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            Result<EventType> result = new Result<EventType>();
            var eventTypeData =  await _context.EventType.Where(x => x.TypeId == id).FirstOrDefaultAsync();
            if (eventTypeData == null)
            {
                result.ErrorMessage = "Event type Id not found";
                return NotFound(result);
            }

            result.Data = eventTypeData;
            return Ok(result);
        }

        // PUT: api/ProjectTypeImage/5
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Put([FromForm] EventTypeImageModel eventTypeImageModel)
        {
            var file = Request.Form.Files[0];
            var result = new Result<string>();
            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var newFileName = $@"{Int32.Parse(eventTypeImageModel.Id)}-{fileName}";

            try
            {
                var selectedEventType = await _context.EventType.Where(x => x.TypeId == Int32.Parse(eventTypeImageModel.Id))
                    .FirstOrDefaultAsync();
                if (selectedEventType == null)
                {
                    throw new Exception("Event Id not found");
                }
                // remove the old image if there is one
                if (selectedEventType.EventTypeImage != null)
                {
                    DeleteImage(selectedEventType.EventTypeImage);
                }

                //update image name on db
                selectedEventType.EventTypeImage = $@"Images/EventTypeImages/{newFileName}";
                await _context.SaveChangesAsync();

                // add new image
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
                    await eventTypeImageModel.FormFile.CopyToAsync(memoryStream);
                    await storageClient.UploadObjectAsync(bucketName, $@"wwwroot/Images/EventTypeImages/{newFileName}", "image/jpeg", memoryStream);
                }

                result.Data = $@"{fileName} successfully uploaded";
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                return BadRequest(result);
            }
            return Ok(result);

        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
