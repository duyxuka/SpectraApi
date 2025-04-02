using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    public class UserClaim
    {
        public string Type { get; set; }
        public string Value { get; set; }

        public UserClaim(string type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}
