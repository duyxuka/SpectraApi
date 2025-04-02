using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spectra.Models;
using Spectra.Services;

namespace Spectra.Controllers
{
    //[EnableCors("AddCorsIPN")]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;

        public PaymentController(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }
        [HttpPost]
        [Route("Payment")]
        [Produces("application/json")]
        public IActionResult PostOrderPayment([FromBody] Order order)
        {
            var url = _vnPayService.CreatePaymentUrl(order, HttpContext);

            return Ok(url);
        }

        [HttpGet]
        [Route("PaymentCallback")]
        [Produces("application/json")]
        public IActionResult PaymentCallback()
        {
            var response = _vnPayService.Responsepay(Request.Query);

            return Ok(response);
        }
    }
}