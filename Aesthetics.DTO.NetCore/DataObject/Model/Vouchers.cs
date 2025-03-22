using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject.Model
{
    public class Vouchers
    {
        [Key]
        public int VoucherID { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
		public string? VoucherImage { get; set; }
		public double? DiscountValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? MinimumOrderValue { get; set; }
        public string? RankMember { get; set; }
        public int? IsActive { get; set; }
        public ICollection<Invoice> Invoice { get; set; }
		public ICollection<InvoiceDetail> InvoiceDetails { get; set; }
		public ICollection<Wallets> Wallets { get; set; }
    }
}
