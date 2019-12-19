using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using jupiterCore.jupiterContext;
using Jupiter.Models;
using jupiterCore.Models;

namespace jupiterCore.Controllers
{
    [Route("api/[controller]")]
    public class ProductTimeController : Controller
    {

        private readonly jupiterContext.jupiterContext _context;
        private readonly IMapper _mapper;

        public ProductTimeController(jupiterContext.jupiterContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
        [HttpGet]
        [Route("[action]/{ProdDetailId}")]
        public List<ProductTimetable> GetProductTime(int proDetailId)
        {
            var productTime = _context.ProductTimetable.Where(x => x.ProdDetailId == proDetailId).ToList();
            return productTime;
        }



        //generate the date between now and the end of this month
        private List<DateTime> GenerateDate(DateTime beginDate)
        {
            //DateTime startDay = DateTime.Parse(DateTime.Now.ToShortDateString());
            //var endMonth = DateTime.Parse(beginDate.AddDays(1 - beginDate.Day).AddMonths(4).AddDays(-1).ToShortDateString());
            var endMonth = DateTime.Parse(beginDate.AddDays(90).ToShortDateString());
            List<DateTime> dateList = new List<DateTime>();
            for (DateTime dt = beginDate; dt <= endMonth; dt = dt.AddDays(1))
            {
                dateList.Add(dt);
            }
            return dateList;

        }

        //get the total stock of the product
        private int GetProdStock(int proDetailId)
        {
            var productQuantity = _context.ProductDetail.Find(proDetailId);
            return (int)productQuantity.TotalStock;
        }

        //calculate the available stock of the produdct in the time period
        private int[] CalculateQuantity (int proDetailId, DateTime beginDate)
        {
            List<DateTime> dateList = GenerateDate(beginDate);
            int totalStock = GetProdStock(proDetailId);

            var productTime = GetProductTime(proDetailId);
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
                storeAvaliableStock[i] = CalculateQuantity(prod.proddetailid, prod.beginDate);
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
                //dateList.Add(DateTime.Now.AddDays(avaliable[j]));
                dateList.Add(checkProdStockModel.ToArray()[0].beginDate.AddDays(avaliable[j]));
            }

            List<DateTime> allDateList = GenerateDate(checkProdStockModel.ToArray()[0].beginDate);
            List<DateTime> unavaliable = new List<DateTime>(allDateList.Except(dateList));
            return unavaliable;
        }







    }
}
