﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using jupiterCore.jupiterContext;
using Jupiter.ActionFilter;
using Jupiter.Controllers;
using Jupiter.Models;
using Microsoft.AspNetCore.Authorization;

namespace jupiterCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoriesController : BasicController
    {
        private readonly jupiterContext.jupiterContext _context;
        private readonly IMapper _mapper;

        public ProductCategoriesController(jupiterContext.jupiterContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

//        [HttpGet]
//        public async Task<List<ProductCategory>> GetProductCategories()
//        {
//            return await _context.ProductCategory.ToListAsync();
//        }

        // GET: api/ProductCategories/GetProductCategoriesByType
        [HttpGet]
        [Route("{action}/{id}")]
        public async Task<ProductType> GetProductCategoriesByType(int id)
        {
            return await _context.ProductType.Where(x => x.ProdTypeId == id)
                .Include(x => x.ProductCategory).FirstOrDefaultAsync();
        }

        // GET: api/ProductCategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductCategory>> GetProductCategory(int id)
        {
            var prodCategory = await _context.Product
                .Where(x => x.CategoryId == id && x.IsActivate == 1)
                .Include(x => x.ProductMedia)
                .Include(x=>x.Category)
                .Select(x => x).ToListAsync();
            return Ok(prodCategory);
        }

        // PUT: api/ProductCategories/5
        //[CheckModelFilter]
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutProductCategory(int id, List<ProductCategoryModel> productCategoryModelList)
        {
            var result = new Result<string>();
            Type prodCateType = typeof(ProductCategory);

            foreach (var cate in productCategoryModelList)
            {
                if (cate.CategoryId == 0)
                {
                    ProductCategory productCategory = new ProductCategory();
                    _mapper.Map(cate, productCategory);
                    await _context.ProductCategory.AddAsync(productCategory);
                }
                else
                {
                    var existProdCategory = await _context.ProductCategory.FirstOrDefaultAsync(x => x.CategoryId == cate.CategoryId);
                    UpdateTable(cate,prodCateType,existProdCategory);
                }
            }

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

        // POST: api/ProductCategories
        [CheckModelFilter]
        [HttpPost]
        public async Task<ActionResult<ProductCategory>> PostProductCategory(ProductCategoryModel productCategoryModel)
        {
            var result = new Result<ProductCategory>();
            ProductCategory productCategory = new ProductCategory();
            _mapper.Map(productCategoryModel, productCategory);
            try
            {
                result.Data = productCategory;
                await _context.ProductCategory.AddAsync(productCategory);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                result.IsFound = false;
            }
            return Ok(result);
        }

        // DELETE: api/ProductCategories/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProductCategory>> DeleteProductCategory(int id)
        {
            var productCategory = await _context.ProductCategory.FindAsync(id);
            if (productCategory == null)
            {
                return NotFound();
            }

            _context.ProductCategory.Remove(productCategory);
            await _context.SaveChangesAsync();

            return productCategory;
        }
    }
}
