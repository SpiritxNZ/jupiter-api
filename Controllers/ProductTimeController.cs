using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using jupiterCore.jupiterContext;
using Jupiter.Models;
using jupiterCore.Models;
using Jupiter.Controllers;

namespace jupiterCore.Controllers
{
    [Route("api/[controller]")]
    public class ProductTimeController : BasicController
    {

        private readonly jupiterContext.jupiterContext _context;
        private readonly IMapper _mapper;

        public ProductTimeController(jupiterContext.jupiterContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public class JsonResult
        {
            public int? ProdDetailId { get; set; }
            public int? ProdId { get; set; }
        }

        // GET: api/values
        //[HttpGet]
        //[Route("[action]/{ProdId}")]
        //public ActionResult<IEnumerable<ProductTimetable>> GetProductTime(int ProdId)
        //{
        //    var result = new Result<IEnumerable<ProductTimetable>>();

        //    var productTime = _context.ProductTimetable.Where(x => x.ProdId == ProdId).ToList();
        //    result.Data = productTime;
        //    return Ok(result);
        //}

        //get product rent time
        [HttpGet("{id}")]
        //[Route("[action]/{ProdDetailId}")]
        public List<ProductTimetable> GetProductTime(int id,int isDetailId)
        {
            var productTime = new List<ProductTimetable>();
            if (isDetailId == 1)
            {
                productTime = _context.ProductTimetable.Where(x => x.ProdDetailId == id && x.IsActive==1).ToList();
            }
            else if (isDetailId == 0)
            {
                productTime = _context.ProductTimetable.Where(x => x.ProdId == id && x.IsActive==1).ToList();
            }


            return productTime;
        }



        //generate the date between now and the end of this month
        private List<DateTime> GenerateDate(DateTime beginDate)
        {
            var endMonth = DateTime.Parse(beginDate.AddDays(90).ToShortDateString());
            List<DateTime> dateList = new List<DateTime>();
            for (DateTime dt = beginDate; dt <= endMonth; dt = dt.AddDays(1))
            {
                dateList.Add(dt);
            }
            return dateList;

        }

        //get the total stock of the product
        private int GetProdStock(int proDetailId,int isDetailId)
        {
            if (isDetailId == 1)
            {
                var productQuantity1 = _context.ProductDetail.Find(proDetailId);
                return (int)productQuantity1.TotalStock;
            }

            var productQuantity = _context.Product.First(x=>x.ProdId==proDetailId);
            return (int)productQuantity.TotalStock;

            
        }

        //calculate the available stock of the produdct in the time period
        private int[] CalculateQuantity (int proDetailId, DateTime beginDate, int isDetailId)
        {
            List<DateTime> dateList = GenerateDate(toNZTimezone(beginDate));
            int totalStock = GetProdStock(proDetailId,isDetailId);

            var productTime = GetProductTime(proDetailId,isDetailId);
            int prodTimeLength = productTime.Count();
            int[] prodQuantity = new int[dateList.Count];
            for (int i = 0; i < dateList.Count(); i++)
            {
                for (int j = 0; j< prodTimeLength; j++)
                {
                    if (dateList[i] >= productTime[j].BeginDate && dateList[i] <= productTime[j].EndDate)
                    {
                        prodQuantity[i] += (int)productTime[j].Quantity;
                    }
                }
                prodQuantity[i] = totalStock - prodQuantity[i];
            }
            
            return prodQuantity;
        }

        [HttpPost]
        [Route("[action]")]
        public List<DateTime> CalculateTime ([FromBody] IEnumerable<CheckProdStockModel> checkProdStockModel)
        {
            int count = checkProdStockModel.Count();
            int[][] storeAvaliableStock = new int[count][];
            int[] rentNum = new int[count];
            List<int> avaliable = new List<int>();

            int i = 0;
            foreach(var prod in checkProdStockModel)
            {
                if(prod.prodid == null)
                {
                    storeAvaliableStock[i] = CalculateQuantity((int)prod.proddetailid, prod.beginDate,1);
                }
                else if (prod.prodid != null)
                {
                    storeAvaliableStock[i] = CalculateQuantity((int)prod.prodid, prod.beginDate,0);
                }
                
                rentNum[i] = prod.quantity;
                i++;
            }

            for(int index = 0; index < storeAvaliableStock[0].Count(); index++)
            {
                int satisfiedNum = 0;
                for(int j = 0; j <count; j++)
                {
                    if(storeAvaliableStock[j][index] < rentNum[j])
                    {
                        break;
                    }
                    satisfiedNum++;
                }
                if(satisfiedNum == count)
                {
                    avaliable.Add(index);
                }
                
            }
            List<DateTime> dateList = new List<DateTime>();
            for (int j = 0;j<avaliable.Count;j++)
            {
                dateList.Add(checkProdStockModel.ToArray()[0].beginDate.AddDays(avaliable[j]));
            }

            List<DateTime> allDateList = GenerateDate(checkProdStockModel.ToArray()[0].beginDate);
            List<DateTime> unavaliable = new List<DateTime>(allDateList.Except(dateList));
            return unavaliable;
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult CheckIfAvaliable([FromBody] IEnumerable<ProductTimetableModel> productTimetableModel)
        {
            //var result = new Result<Object>();
            var result = new Result<string>();
            //var checkif = new List<JsonResult>();
            var checkProdStockModel = new List<CheckProdStockModel>();
            productTimetableModel.ToList().ForEach(s =>
            {
                checkProdStockModel.Add(new CheckProdStockModel
                {
                    proddetailid = s.ProdDetailId,
                    prodid = s.ProdId,
                    beginDate = (DateTime)s.BeginDate,
                    quantity = (int)s.Quantity,

                });
            });
            List<DateTime> unavaliables = CalculateTime(checkProdStockModel);
            DateTime begindate = (DateTime)productTimetableModel.ToArray()[0].BeginDate;
            DateTime enddate = (DateTime)productTimetableModel.ToArray()[0].EndDate;

            foreach (var unavaliable in unavaliables)
            {
                if (unavaliable >= begindate && unavaliable <= enddate)
                {
                    result.IsSuccess = false ;
                    break;

                }
            }

            //foreach (var productTimetable in productTimetableModel)
            //{
            //    foreach (var unavaliable in unavaliables)
            //    {
            //        if (unavaliable >= productTimetable.BeginDate && unavaliable <= productTimetable.EndDate)
            //        {
            //            if (productTimetable.ProdId != null)
            //            {
            //                checkif.Add(new JsonResult
            //                {
            //                    ProdId = productTimetable.ProdId,
            //                });
            //                break;
            //            }
            //            checkif.Add(new JsonResult
            //            {
            //                ProdDetailId = productTimetable.ProdDetailId,
            //            });
            //            break;

            //        }
            //    }
            //}

            return Ok(result);
        }

    }
}
