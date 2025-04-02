using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spectra.Models;

namespace Spectra.Services
{
    public interface IServiceItem
    {
        Task UpdateDatabase(Item item);
        Task UpdateDatabaseJobIdAsync(Item item, string jobId);
        Task UpdateDatabaseAgain(Item item);
    }
}
