using Microsoft.EntityFrameworkCore;
using Spectra.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Services
{
    public class ServiceItem: IServiceItem
    {
        private readonly AppDBContext _context;
        public ServiceItem(AppDBContext context)
        {
            _context = context;
        }
        public async Task UpdateDatabase(Item item)
        {
            Item p = (from x in _context.Items
                         where x.Id == item.Id
                         select x).First();
            p.Price = item.Price;
            p.GiftId = item.GiftId;
            _context.Entry(p).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            Console.WriteLine($"Update Database: at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        }
        public async Task UpdateDatabaseJobIdAsync(Item item, string jobId)
        {
            Item p = (from x in _context.Items
                      where x.Id == item.Id
                      select x).First();
            p.JobId = jobId;
            _context.Entry(p).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            Console.WriteLine($"Update Database: at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        }
        public async Task UpdateDatabaseAgain(Item item)
        {
            Item p = (from x in _context.Items
                      where x.Id == item.Id
                      select x).First();
            p.Price = item.PriceAgain;
            p.JobId = "0";
            p.PriceAgain = 0;
            p.GiftId = 1;
            _context.Entry(p).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            Console.WriteLine($"Update Database: at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        }
    }
}
