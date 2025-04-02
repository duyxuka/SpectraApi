using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Spectra.Models;
using Spectra.Models.Payment;

namespace Spectra.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDBContext _context;

        public VnPayService(IConfiguration configuration, AppDBContext _context)
        {
            _configuration = configuration;
            this._context = _context;
        }
        public string CreatePaymentUrl(Order model, HttpContext context)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnPayLibrary(_context);
            var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];

            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", (model.TotalAmount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"Mã order: {model.Code}-{model.Name}-{model.Email}-{model.Phone}-{model.TotalAmount}");
            pay.AddRequestData("vnp_OrderType", "Khác");
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", model.Code);


            var paymentUrl =
                pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

            return paymentUrl;
        }

        //public string CreatePaymentUrl(Demo model, HttpContext context)
        //{
        //    var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
        //    var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
        //    var tick = DateTime.Now.Ticks.ToString();
        //    var pay = new VnPayLibrary();
        //    var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];

        //    pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
        //    pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
        //    pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
        //    pay.AddRequestData("vnp_Amount", ((int)model.Amount * 100).ToString());
        //    pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
        //    pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
        //    pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
        //    pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
        //    pay.AddRequestData("vnp_OrderInfo", $"{model.Name} {model.OrderDescription} {model.Amount}");
        //    pay.AddRequestData("vnp_OrderType", model.OrderType);
        //    pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
        //    pay.AddRequestData("vnp_TxnRef", tick);

        //    var paymentUrl =
        //        pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

        //    return paymentUrl;
        //}
        [Produces("application/json")]
        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var pay = new VnPayLibrary(_context);
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);

            return response;
        }
        //[Produces("application/json")]
        //public PaymentIPN Responsepay(IQueryCollection collections)
        //{
        //    var pay = new VnPayLibrary(_context);
        //    var response = pay.ResponsepayAsync(collections, _configuration["Vnpay:HashSecret"]);

        //    return response;
        //}
        public PaymentIPN Responsepay(IQueryCollection collections)
        {
            var pay = new VnPayLibrary(_context);
            var response = pay.ResponsepayAsync(collections, _configuration["Vnpay:HashSecret"]).Result;

            return response;
        }
    }
}
