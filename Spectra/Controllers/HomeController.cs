using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Spectra.Models;
using Spectra.Services;

namespace Spectra.Controllers
{
    public class HomeController : Controller
    {
        private readonly IVnPayService _vnPayService;

        public HomeController(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}

        //public IActionResult CreatePaymentUrl(Demo model)
        //{
        //    var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

        //    return Redirect(url);
        //}
        //[Produces("application/json")]
        //public IActionResult PaymentCallback()
        //{
        //    var response = _vnPayService.Responsepay(Request.Query);

        //    return Json(response);
        //}

        public IActionResult Index()
        {
            return new RedirectResult("~/swagger/index.html");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
