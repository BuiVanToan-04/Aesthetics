using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject.LogginModel
{
    public class Vouchers_Loggin
    {
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
	}
}
