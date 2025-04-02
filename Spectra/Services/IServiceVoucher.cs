using Spectra.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Services
{
    public interface IServiceVoucher
    {
        Task UpdateDatabase(Voucher voucher);
        Task UpdateDatabaseJobIdAsync(Voucher voucher, string jobId);
        Task UpdateDatabaseAgain(Voucher voucher);
    }
}
