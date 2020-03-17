using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Jupiter.Controllers;
using Jupiter.Models;
using jupiterCore.jupiterContext;
using jupiterCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace jupiterCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : BasicController
    {
        private readonly jupiterContext.jupiterContext _context;
        public VideoController(jupiterContext.jupiterContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Video>>> GetVideo()
        {
            return await _context.Videos.ToListAsync();
        }

        // GET: api/Videos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Video>> GetVideo(int id)
        {
            var video = await _context.Videos.FindAsync(id);

            if (video == null)
            {
                return NotFound();
            }

            return video;
        }

        // PUT: api/Videos/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutVideo(int id, VideoModel VideoModel)
        {
            var result = new Result<Video>();
            Type VideoType = typeof(Video);
            var updateVideo = await _context.Videos.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (updateVideo == null)
            {
                return NotFound(DataNotFound(result));
            }
            updateVideo.Type = VideoModel.Type;
            updateVideo.Description = VideoModel.Description;
            updateVideo.Url = VideoModel.Url;
            result.Data = updateVideo;
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

        // POST: api/Videos
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Video>> PostVideo(VideoModel VideoModel)
        {
            var result = new Result<Video>();
            Video newVideo = new Video {
                Description = VideoModel.Description,
                Url=VideoModel.Url,
                Type= VideoModel.Type
            };

            try
            {
                result.Data = newVideo;
                await _context.Videos.AddAsync(newVideo);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                result.IsFound = false;
            }
            return Ok(result);
        }

        // DELETE: api/Videos/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<Video>> DeleteVideo(int id)
        {
            var video = await _context.Videos.FindAsync(id);
            if (video == null)
            {
                return NotFound();
            }

            _context.Videos.Remove(video);
            await _context.SaveChangesAsync();

            return video;
        }
    }
}
