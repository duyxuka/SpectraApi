using Spectra.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Services
{
    public class ServiceVoucher : IServiceVoucher
    {
        private readonly AppDBContext _context;
        public ServiceVoucher(AppDBContext context)
        {
            _context = context;
        }
        public async Task UpdateDatabase(Voucher voucher)
        {
            Voucher p = (from x in _context.Vouchers
                          where x.Id == voucher.Id
                          select x).First();
            p.StartDate = voucher.StartDate;
            p.EndDate = voucher.EndDate;
            p.Status = true;
            p.ScheduleStatus = true;
            _context.Entry(p).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            Console.WriteLine($"Update Database: at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        }
        public async Task UpdateDatabaseJobIdAsync(Voucher voucher, string jobId)
        {
            Voucher p = (from x in _context.Vouchers
                         where x.Id == voucher.Id
                         select x).First();
            p.JobId = jobId;
            _context.Entry(p).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            Console.WriteLine($"Update Database: at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        }
        public async Task UpdateDatabaseAgain(Voucher voucher)
        {
            Voucher p = (from x in _context.Vouchers
                         where x.Id == voucher.Id
                         select x).First();
            p.Status = false;
            p.ScheduleStatus = false;
            p.JobId = "0";
            p.StartDate = new DateTime(0001,01,01,00,00,00);
            p.EndDate = new DateTime(0001,01,01, 00, 00, 00);
            _context.Entry(p).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            Console.WriteLine($"Update Database: at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        }
    }
}
