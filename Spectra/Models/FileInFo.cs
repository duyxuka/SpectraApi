using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    public class FileInFo
    {
        public string Name { get; set; }
        public IFormFile Url { get; set; }
    }
}
