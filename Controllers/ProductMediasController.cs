using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using jupiterCore.jupiterContext;
using Jupiter.ActionFilter;
using Jupiter.Controllers;
using Jupiter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Type = System.Type;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace jupiterCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductMediasController : BasicController
    {
        private readonly jupiterContext.jupiterContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        public ProductMediasController(jupiterContext.jupiterContext context, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        // GET: api/ProductMedias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductMedia>>> GetProductMedia()
        {
            return await _context.ProductMedia.ToListAsync();
        }

        // GET: api/ProductMedias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<List<ProductMedia>>> GetProductMedia(int id)
        {
            var productMedia = await _context.ProductMedia.Where(x => x.ProdId == id).Select(x => x).ToListAsync();

            if (productMedia == null)
            {
                return NotFound();
            }

            return productMedia;


        }

        // PUT: api/ProductMedias/5
        [CheckModelFilter]
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutProductMedia(int id, ProductMediaModel productMediaMediaModel)
        {
            var result = new Result<string>();
            Type mediaType = typeof(ProductMedia);
            var updateMedia = await _context.ProductMedia.Where(x=>x.Id == id).FirstOrDefaultAsync();
            if (updateMedia == null)
            {
                return NotFound(DataNotFound(result));
            }
            UpdateTable(productMediaMediaModel,mediaType,updateMedia);
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

        // POST: api/ProductMedias
        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm] ProductMediaModel productMediaModel)
        {
            var requestForm = Request.Form;
            var file = requestForm.Files[0];
            var result = new Result<string>();
            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var newFileName = $@"{Int32.Parse(productMediaModel.ProdId)}-{fileName}";
            try
            {
                //    //add image name to db
                ProductMedia productMedia = new ProductMedia { ProdId = Int32.Parse(productMediaModel.ProdId), Url = $@"Images/ProductImages/{newFileName}" };
                await _context.ProductMedia.AddAsync(productMedia);
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
                    await storageClient.UploadObjectAsync(bucketName, $@"wwwroot/Images/ProductImages/{newFileName}", "image/jpeg", memoryStream);
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

        //[Authorize]
        //public async Task<IActionResult> UploadFile([FromForm] ProductMediaModel productMediaModel)
        //{
        //    var requestForm = Request.Form;
        //    var file = requestForm.Files[0];
        //    var result = new Result<string>();
        //    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
        //    var newFileName = $@"{Int32.Parse(productMediaModel.ProdId)}-{fileName}";
        //    try
        //    {
        //        // add image
        //        bool isStoreSuccess = await StoreImage("ProductImages", newFileName, file);
        //        if (!isStoreSuccess)
        //        {
        //            throw new Exception("Store image locally failed.");
        //        }

        //        //add image name to db
        //        ProductMedia productMedia = new ProductMedia { ProdId = Int32.Parse(productMediaModel.ProdId), Url = $@"Images/ProductImages/{newFileName}" };
        //        await _context.ProductMedia.AddAsync(productMedia);
        //        await _context.SaveChangesAsync();

        //        result.Data = $@"{fileName} successfully uploaded";
        //    }
        //    catch (Exception e)
        //    {
        //        result.ErrorMessage = e.Message;
        //        return BadRequest(result);
        //    }
        //    return Ok(result);

        //}




        // DELETE: api/ProductMedias/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<ProductMedia>> DeleteProductMedia(int id)
        {
            var result = new Result<string>();
            var media = await _context.ProductMedia.FindAsync(id);
            if (media == null)
            {
                return NotFound(DataNotFound(result));
            }

            try
            {
                //remove img from folder
                DeleteImage(media.Url);
            }
            catch (Exception e)
            {
                return BadRequest("unable to delete image");
            }
            _context.ProductMedia.Remove(media);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                result.IsSuccess = false;
            }
            return Ok(result);
        }
    }
}
