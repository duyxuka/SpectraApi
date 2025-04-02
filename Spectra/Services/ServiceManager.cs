using Spectra.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Services
{
    public class ServiceManager : IServiceManagercs
    {
        private readonly AppDBContext _context;
        public ServiceManager(AppDBContext context)
        {
            _context = context;
        }
        public async Task UpdateDatabase(Product product)
        {
            Product p = (from x in _context.Products
                          where x.Id == product.Id
                          select x).First();
            p.SalePrice = product.SalePrice;
            p.GiftId = product.GiftId;
            p.Start = product.Start;
            p.Ends = product.Ends;
            _context.Entry(p).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            Console.WriteLine($"Update Database: at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        }
        public async Task UpdateDatabaseJobIdAsync(Product product, string jobId)
        {
            Product p = (from x in _context.Products
                         where x.Id == product.Id
                         select x).First();
            p.JobId = jobId;
            _context.Entry(p).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            Console.WriteLine($"Update Database: at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        }
        public async Task UpdateDatabaseAgain(Product product)
        {
            Product p = (from x in _context.Products
                         where x.Id == product.Id
                         select x).First();
            p.ScheduleStatus = false;
            p.JobId = "0";
            p.SalePrice = 0;
            p.GiftId = 1;
            p.Start = new DateTime(0001,01,01,00,00,00);
            p.Ends = new DateTime(0001,01,01, 00, 00, 00);
            _context.Entry(p).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            Console.WriteLine($"Update Database: at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        }
    }
}
