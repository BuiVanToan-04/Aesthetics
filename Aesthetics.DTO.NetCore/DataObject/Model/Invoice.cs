using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject.Model
{
    public class Invoice
    {
        [Key]
        public int InvoiceID { get; set; }
        public int UserID { get; set; }
        public int VoucherID { get; set; }
        public string Code { get; set; }
        public double DiscountValue { get; set; }
        public double TotalAmount { get; set; }
        public DateTime DateCreated { get; set; }
        public int Status { get; set; }
        public int DeleteStatus { get; set; }
        public string Type { get; set; }
        public Users Users { get; set; }
        public ICollection<InvoiceDetail> InvoiceDetails { get; set; }
        public ICollection<Vouchers> Vouchers { get; set; }

    }
}
