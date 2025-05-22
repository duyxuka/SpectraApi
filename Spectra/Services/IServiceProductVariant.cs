using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spectra.Models;

namespace Spectra.Services
{
    public interface IServiceProductVariant
    {
        Task UpdateDatabase(ProductVariant productVariant);
        Task UpdateDatabaseJobIdAsync(ProductVariant productVariant, string jobId);
        Task UpdateDatabaseAgain(ProductVariant productVariant);
    }
}
