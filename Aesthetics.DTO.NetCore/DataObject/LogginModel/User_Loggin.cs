using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject.LogginModel
{
    public class User_Loggin
    {
		public int? UserID { get; set; }
		public string? UserName { get; set; }
		public string? PassWord { get; set; }
		public string? Email { get; set; }
		public DateTime? DateBirth { get; set; }
		public string? Sex { get; set; }
		public DateTime? Creation { get; set; }
		public string? Phone { get; set; }
		public string? Addres { get; set; }
		public string? IDCard { get; set; }
		public string? TypePerson { get; set; }
		public int? AccumulatedPoints { get; set; }
		public string? ReferralCode { get; set; }
		public string? RefeshToken { get; set; }
		public int? DeleteStatus { get; set; }
		public double? Moneyy { get; set; }
		public DateTime? TokenExprired { get; set; }
		public int? RatingPoints { get; set; }
		public string? RankMember { get; set; }
	}
}
