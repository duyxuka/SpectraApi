using Microsoft.AspNetCore.Http;
using Spectra.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(Order model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);
        PaymentIPN Responsepay(IQueryCollection collections);
    }
}
