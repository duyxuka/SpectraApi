using Spectra.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Services
{
    public interface IServiceManagercs
    {
        Task UpdateDatabase(Product product);
        Task UpdateDatabaseJobIdAsync(Product product, string jobId);
        Task UpdateDatabaseAgain(Product product);
    }
}
