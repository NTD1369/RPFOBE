using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.OMSModels
{
    public class PayooDataModel
    {
        /// <summary>
        /// Mã đơn hàng, tồn tại duy nhất trong hệ thống đối tác. Tối đa 32 kí tự bao gồm chữ và số
        /// </summary>
        public string OrderCode { get; set; }
        /// <summary>
        /// Số tiền đơn hàng
        /// </summary>
        public double OrderAmount { get; set; }
        /// <summary>
        /// Ngày hết hạn thanh toán. Format: yyyyMMddHHmmss(24h)
        /// </summary>
        public string OrderExpiredDate { get; set; }
        /// <summary>
        /// Link nhận notify thanh toán từ Payoo nếu thanh toán thành công
        /// </summary>
        public string OrderLinkNotify { get; set; }
        /// <summary>
        /// Mã đại lý.  PAYOO sẽ cung cấp AccountName tương ứng
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// Mã nhân viên. Mã này chính là mã nhân viên đăng nhập vào app mPOS
        /// </summary>
        public string StaffCode { get; set; }
        /// <summary>
        /// Tên khách hàng, tối đa 32 kí tự
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// Địa chỉ khách hàng
        /// </summary>
        public string CustomerAddress { get; set; }
        /// <summary>
        /// Số điện thoại khách hàng
        /// </summary>
        public string CustomerPhone { get; set; }
        /// <summary>
        /// Email khách hàng, nên có
        /// </summary>
        public string CustomerEmail { get; set; }
        /// <summary>
        /// Ghi chú đơn hàng, tối đa 32 kí tự
        /// </summary>
        public string OrderNote { get; set; }
        /// <summary>
        /// Chi tiết đơn hàng, tối đa 2000 kí tự
        /// </summary>
        public string OrderDetail { get; set; }
        /// <summary>
        /// Mã thiết bị tạo đơn hàng
        /// </summary>
        public string TerminalID { get; set; }
        /// <summary>
        /// Mã cửa hàng tạo đơn hàng
        /// </summary>
        public string CreateShopCode { get; set; }
        /// <summary>
        /// Ngày tạo đơn hàng. Format: yyyyMMddHHmmss(24h)
        /// </summary>
        public string CreateDate { get; set; }
        /// <summary>
        /// Trạng thái đơn hàng
        /// </summary>
        public PayooOrderStatus? Status { get; set; }
    }

    public class PayooResponseModel
    {
        public int ReturnCode { get; set; }
        public string Description { get; set; }
        public object ResponseData { get; set; }
    }

    public enum PayooOrderStatus
    {
        CHUA_THANH_TOAN = 0,
        DA_THANH_TOAN = 1,
        HUY_THANH_TOAN = 2,
        HUY_DON_HANG = 3,
        NGHI_VAN = 4,
    }
}
