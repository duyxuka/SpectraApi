using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Spectra.Models;
using Spectra.Models.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Services
{
    public class VnpayPaymentStatusJob
    {
        private readonly AppDBContext _context;
        private readonly IConfiguration _configuration;

        public VnpayPaymentStatusJob(AppDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Job này được Hangfire gọi tự động, kiểm tra đơn hàng quá hạn và cập nhật trạng thái.
        /// </summary>
        public async Task CheckPendingPaymentsAsync()
        {
            var vnPay = new VnPayLibrary(_context);
            var now = DateTime.Now;
            var timeoutMinutes = 15; // thời gian cho phép thanh toán (VD: 15 phút)

            // Lấy danh sách đơn hàng quá hạn nhưng chưa thanh toán
            var pendingOrders = await _context.Order
                .Where(o => o.PaymentMethod == "VNPAY" && o.Status == 0 && o.CreatedDate <= now.AddMinutes(-timeoutMinutes))
                .ToListAsync();

            if (!pendingOrders.Any())
                return;

            foreach (var order in pendingOrders)
            {
                try
                {
                    // Gọi Query API để kiểm tra giao dịch
                    var result = await vnPay.QueryTransactionAsync(
                        txnRef: order.Code,
                        transactionDate: order.CreatedDate.ToString("yyyyMMddHHmmss"),
                        vnp_TmnCode: _configuration["Vnpay:TmnCode"],
                        vnp_HashSecret: _configuration["Vnpay:HashSecret"]
                    );

                    // Nếu không có phản hồi, đánh dấu hủy
                    if (result == null)
                    {
                        order.Status = 7; // Hủy
                    }
                    else if (result.vnp_TxnRef != order.Code)
                    {
                        // Không cập nhật nếu mã không khớp
                        continue;
                    }
                    else if (result.vnp_TransactionStatus == "00" || result.vnp_ResponseCode == "00")
                    {
                        order.Status = 1; // Thành công
                    }
                    else
                    {
                        order.Status = 7; // Thất bại hoặc chưa thanh toán
                    }

                }
                catch (Exception ex)
                {
                    order.Status = 7;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
