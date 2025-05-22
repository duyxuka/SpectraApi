using Microsoft.EntityFrameworkCore;
using Spectra.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Services
{
    public class ServiceProductVariant : IServiceProductVariant
    {
        private readonly AppDBContext _context;
        public ServiceProductVariant(AppDBContext context)
        {
            _context = context;
        }
        public async Task UpdateDatabase(ProductVariant productVariant)
        {
            ProductVariant p = (from x in _context.ProductVariants
                         where x.Id == productVariant.Id
                         select x).First();
            p.Price = productVariant.Price;
            p.SalePrice = productVariant.SalePrice;
            p.CreatedDate = productVariant.CreatedDate;
            p.ModifiedDate = productVariant.ModifiedDate;
            _context.Entry(p).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            Console.WriteLine($"Update Database: at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        }
        public async Task UpdateDatabaseJobIdAsync(ProductVariant productVariant, string jobId)
        {
            ProductVariant p = (from x in _context.ProductVariants
                                where x.Id == productVariant.Id
                      select x).First();
            p.JobId = jobId;
            _context.Entry(p).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            Console.WriteLine($"Update Database: at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        }
        public async Task UpdateDatabaseAgain(ProductVariant productVariant)
        {
            ProductVariant p = (from x in _context.ProductVariants
                                where x.Id == productVariant.Id
                      select x).First();
            p.JobId = "0";
            p.SalePrice = 0;
            p.CreatedDate = new DateTime(0001, 01, 01, 00, 00, 00);
            p.ModifiedDate = new DateTime(0001, 01, 01, 00, 00, 00);
            _context.Entry(p).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            Console.WriteLine($"Update Database: at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        }
    }
}
