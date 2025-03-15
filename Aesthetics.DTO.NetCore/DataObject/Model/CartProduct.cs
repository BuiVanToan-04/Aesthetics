using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject.Model
{
	public class CartProduct
	{
		[Key]
		public int? CartProductID { get; set; }
		public int? CartID { get; set; }
		public int? ProductID { get; set; }
		public int? Quantity { get; set; }
		public DateTime? CreateDay { get; set; }
		public Carts Carts { get; set; }
		public Products Products { get; set; }
	}
}
