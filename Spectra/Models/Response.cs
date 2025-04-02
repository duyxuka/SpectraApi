using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    public class Response
    {
        public Response(bool IsSuccess, string Message)
        {
            this.IsSuccess = IsSuccess;
            this.Message = Message;
        }

        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
