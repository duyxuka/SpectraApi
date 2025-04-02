using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    public class AuthenticatedResponse
    {
        public string Token { get; set; }
        public object User { get; set; }
    }
}
