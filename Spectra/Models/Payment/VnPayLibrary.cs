
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Spectra.Models.Payment
{
    public class VnPayLibrary 
    {
        private readonly AppDBContext _context;
        public VnPayLibrary(AppDBContext context)
        {
            _context = context;
        }
     
        
        public const string VERSION = "2.1.0";
        private readonly SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
        private readonly SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());

        public PaymentResponseModel GetFullResponseData(IQueryCollection collection, string hashSecret)
        {
            PaymentResponseModel payments = new PaymentResponseModel();
            string returnContent = string.Empty;

            if (collection.Count > 0)
            {
                var vnPay = new VnPayLibrary(_context);

                foreach (var (key, value) in collection)
                {
                    if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                    {
                        AddResponseData(key, value);
                    }
                }

                var orderId = Convert.ToInt64(GetResponseData("vnp_TxnRef"));
                var vnPayTranId = Convert.ToInt64(GetResponseData("vnp_TransactionNo"));
                var vnpResponseCode = GetResponseData("vnp_ResponseCode");
                var vnpSecureHash =
                    collection.FirstOrDefault(k => k.Key == "vnp_SecureHash").Value; //hash của dữ liệu trả về
                var orderInfo = GetResponseData("vnp_OrderInfo");
                //var orderdataId = vnPay.GetResponseData("vnp_OrderType");
                var vnp_Amount = Convert.ToInt64(GetResponseData("vnp_Amount")) / 100;
                var vnp_TransactionStatus = GetResponseData("vnp_TransactionStatus");
                var checkSignature =
                    ValidateSignature(vnpSecureHash, hashSecret); //check Signature

                if (!checkSignature)
                    return new PaymentResponseModel()
                    {
                        Success = "2"
                    };
                if (checkSignature)
                {
                    payments.Success = "0";
                    payments.PaymentMethod = "VnPay";
                    payments.OrderDescription = orderInfo;
                    payments.OrderId = orderId.ToString();
                    payments.PaymentId = vnPayTranId.ToString();
                    payments.TransactionId = vnPayTranId.ToString();
                    payments.Token = vnpSecureHash;
                    payments.Amount = vnp_Amount;
                    payments.VnPayResponseCode = vnpResponseCode;
                    payments.VnPayTransitionStatus = vnp_TransactionStatus;


                    if (payments != null)
                    {
                        if (payments.Amount == vnp_Amount)
                        {
                            if (payments.Success == "0")
                            {
                                //Thêm code Thực hiện cập nhật vào Database 
                                //Update Database

                                returnContent = "{\"RspCode\":\"00\",\"Message\":\"Confirm Success\"}";
                            }
                            else
                            {
                                returnContent = "{\"RspCode\":\"02\",\"Message\":\"Order already confirmed\"}";
                            }
                        }
                        else
                        {
                            returnContent = "{\"RspCode\":\"04\",\"Message\":\"invalid amount\"}";
                        }
                    }
                    else
                    {
                        returnContent = "{\"RspCode\":\"01\",\"Message\":\"Order not found\"}";
                    }
                }
                else
                {
                    returnContent = "{\"RspCode\":\"97\",\"Message\":\"Invalid signature\"}";
                }
            }
            else
            {
                returnContent = "{\"RspCode\":\"99\",\"Message\":\"Input data required\"}";
            }


            return payments;
        }
        
        public async Task<PaymentIPN> ResponsepayAsync(IQueryCollection collection, string hashSecret)
        {
            var returnContent = new PaymentIPN();
            
            
            try {              
               // var vnPay = new VnPayLibrary();

                foreach (var (key, value) in collection)
                {
                    if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                    {
                        AddResponseData(key, value);
                    }
                }

                var orderId = Convert.ToString(GetResponseData("vnp_TxnRef"));
                var vnPayTranId = Convert.ToInt64(GetResponseData("vnp_TransactionNo"));
                var vnpResponseCode = GetResponseData("vnp_ResponseCode");
                var vnpSecureHash =
                    collection.FirstOrDefault(k => k.Key == "vnp_SecureHash").Value; //hash của dữ liệu trả về
                var orderInfo = GetResponseData("vnp_OrderInfo");
                //var orderdataId = vnPay.GetResponseData("vnp_OrderType");
                long vnp_Amount = Convert.ToInt64(GetResponseData("vnp_Amount")) / 100;
                var vnp_TransactionStatus = GetResponseData("vnp_TransactionStatus");
                var tmnCode = GetResponseData("vnp_TmnCode");
                var checkSignature = ValidateSignature(vnpSecureHash, hashSecret); //check Signature

                if (checkSignature)
                {
                    var order = _context.Order.FirstOrDefault(x => x.Code == orderId);

                    if (order != null)
                    {
                        if (order.TotalAmount == vnp_Amount)
                        {
                            if (order.Status == 0)
                            {
                                if (vnpResponseCode == "00" && vnp_TransactionStatus == "00")
                                {
                                    order.Status = 0; // Assuming 1 is the status for successful payment
                                    await _context.SaveChangesAsync();
                                    returnContent.Set("00", "Confirm Success");
                                }
                                else
                                {
                                    order.Status = 7; // Assuming 7 is the status for failed payment
                                    await _context.SaveChangesAsync();
                                    returnContent.Set("00", "Confirm Success");
                                }
                            }
                            else
                            {
                                returnContent.Set("02", "Order already confirmed");
                            }
                        }
                        else
                        {
                            returnContent.Set("04", "Invalid amount");
                        }
                    }
                    else
                    {
                        returnContent.Set("01", "Order not found");
                    }
                }
                else
                {
                    returnContent.Set("97", "Invalid signature");
                }
            }
            catch (Exception e)
            {
                returnContent.Set("99", "Input data required" + e.Message);
            }
            
            return returnContent;
        }
        public string GetIpAddress(HttpContext context)
        {
            var ipAddress = string.Empty;
            try
            {
                var remoteIpAddress = context.Connection.RemoteIpAddress;

                if (remoteIpAddress != null)
                {
                    if (remoteIpAddress.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        remoteIpAddress = Dns.GetHostEntry(remoteIpAddress).AddressList
                            .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
                    }

                    if (remoteIpAddress != null) ipAddress = remoteIpAddress.ToString();

                    return ipAddress;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "127.0.0.1";
        }
        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _responseData.Add(key, value);
            }
        }

        public string GetResponseData(string key)
        {
            return _responseData.TryGetValue(key, out var retValue) ? retValue : string.Empty;
        }

        public string CreateRequestUrl(string baseUrl, string vnpHashSecret)
        {
            var data = new StringBuilder();

            foreach (var (key, value) in _requestData.Where(kv => !string.IsNullOrEmpty(kv.Value)))
            {
                data.Append(WebUtility.UrlEncode(key) + "=" + WebUtility.UrlEncode(value) + "&");
            }

            var querystring = data.ToString();

            baseUrl += "?" + querystring;
            var signData = querystring;
            if (signData.Length > 0)
            {
                signData = signData.Remove(data.Length - 1, 1);
            }

            var vnpSecureHash = HmacSha512(vnpHashSecret, signData);
            baseUrl += "vnp_SecureHash=" + vnpSecureHash;

            return baseUrl;
        }

        public bool ValidateSignature(string inputHash, string secretKey)
        {
            var rspRaw = GetResponseData();
            var myChecksum = HmacSha512(secretKey, rspRaw);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

        private string HmacSha512(string key, string inputData)
        {
            var hash = new StringBuilder();
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                var hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }

        private string GetResponseData()
        {
            var data = new StringBuilder();
            if (_responseData.ContainsKey("vnp_SecureHashType"))
            {
                _responseData.Remove("vnp_SecureHashType");
            }

            if (_responseData.ContainsKey("vnp_SecureHash"))
            {
                _responseData.Remove("vnp_SecureHash");
            }

            foreach (var (key, value) in _responseData.Where(kv => !string.IsNullOrEmpty(kv.Value)))
            {
                data.Append(WebUtility.UrlEncode(key) + "=" + WebUtility.UrlEncode(value) + "&");
            }

            //remove last '&'
            if (data.Length > 0)
            {
                data.Remove(data.Length - 1, 1);
            }

            return data.ToString();
        }
    }

    public class VnPayCompare : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            var vnpCompare = CompareInfo.GetCompareInfo("en-US");
            return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
        }
    }
}
