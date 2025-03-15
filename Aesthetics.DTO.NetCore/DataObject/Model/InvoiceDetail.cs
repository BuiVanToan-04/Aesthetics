using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject.Model
{
    public class InvoiceDetail
    {
        [Key]
        public int InvoiceDetailID { get; set; }
        public int InvoiceID { get; set; }
		//public int UserID { get; set; }
		public int CustomerID { get; set; }
        public string? CustomerName { get; set; }
		public int EmployeeID { get; set; }
		public string? EmployeeName { get; set; }
        public int? ProductID { get; set; }
        public string? ProductName { get; set; }
		public int? ServiceID { get; set; }
		public string? ServiceName { get; set; }
		public int VoucherID { get; set; }
		public string? Code { get; set; }
		public double? DiscountValue { get; set; }
		public double? PriceService { get; set; }
		public double? PriceProduct { get; set; }
		public int? TotalQuantityService { get; set; }
		public int? TotalQuantityProduct { get; set; }
		public double? TotalMoney { get; set; }
        public int? DeleteStatus { get; set; }
        public string? Type { get; set; }
        public Invoice Invoice { get; set; }
        public Users Users { get; set; }
        public Products Products { get; set; }
        public Servicess Servicess { get; set; }
        public Vouchers Vouchers { get; set; }
    }
}
