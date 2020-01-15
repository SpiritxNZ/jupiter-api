using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using jupiterCore.jupiterContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace jupiterCore.Services
{
    public class TimedHostedService : IHostedService, IDisposable
    {

        private readonly jupiterContext.jupiterContext _context;
        private Timer _timer;

        public TimedHostedService(jupiterContext.jupiterContext context)
        {
            _context = context;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            //TimeSpan scheduleTime = new TimeSpan(23, 59, 59);
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
           TimeSpan.FromMinutes(2));

            return Task.CompletedTask;
        }


        private async void DoWork(object state)
        {
            var carts = await _context.Cart.Where(x => x.IsExpired == 0 && x.IsPay == 0).ToListAsync();
            foreach(var cart in carts)
            {
                DateTime dateTime = (DateTime)cart.CreateOn;
                DateTime expireDate = dateTime.AddMinutes(15);
                DateTime now = toNZTimezone(DateTime.UtcNow);
                if (DateTime.Compare(expireDate, now)<0)
                {
                    cart.IsExpired = 1;
                    _context.Cart.Update(cart);
                    var prodtimes = await _context.ProductTimetable.Where(x => x.CartId == cart.CartId).ToListAsync();
                    if (prodtimes == null) break;
                    foreach(var prodtime in prodtimes)
                    {
                        prodtime.IsActive = 0;
                        _context.ProductTimetable.Update(prodtime);
                    }
                    
                }
            }
            _context.SaveChanges();
        }




        public Task StopAsync(CancellationToken cancellationToken)
        {

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }


        public void Dispose()
        {
            _timer?.Dispose();
        }


        private DateTime toNZTimezone(DateTime utc)
        {
            DateTime nzTime = new DateTime();
            try
            {
                TimeZoneInfo nztZone = TimeZoneInfo.FindSystemTimeZoneById("New Zealand Standard Time");
                nzTime = TimeZoneInfo.ConvertTimeFromUtc(utc, nztZone);
                return nzTime;
            }
            catch (TimeZoneNotFoundException)
            {
                TimeZoneInfo nztZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific/Auckland");
                nzTime = TimeZoneInfo.ConvertTimeFromUtc(utc, nztZone);
                return nzTime;
            }
            catch (InvalidTimeZoneException)
            {
                TimeZoneInfo nztZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific/Auckland");
                nzTime = TimeZoneInfo.ConvertTimeFromUtc(utc, nztZone);
                return nzTime;
            }
        }
    }
}
