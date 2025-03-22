using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.RequestData
{
    public class VouchersRequest
    {
		public string? Description { get; set; }
		public double? DiscountValue { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public float? MinimumOrderValue { get; set; }
		public string? RankMember { get; set; }
		public string? VoucherImage { get; set; }
	}

	public class Update_Vouchers
	{
		public int VoucherID { get; set; }
		public string? Description { get; set; }
		public string? VoucherImage { get; set; }
		public double? DiscountValue { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public float? MinimumOrderValue { get; set; }
		public string? RankMember { get; set; }
	}

	public class Delete_Vouchers
	{
		public int VoucherID { get; set; }
	}

	public class GetList_SearchVouchers
	{
		public int? VoucherID { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public string? RankMember { get; set; }
	}
}
