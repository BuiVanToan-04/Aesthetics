using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject.Model
{
	public class Carts
	{
		[Key]
		public int CartID { get; set; }
		public int UserID { get; set; }
		public DateTime? CreationDate { get; set; }
		public ICollection<CartProduct> CartProducts { get; set; }
		public Users Users { get; set; }
	}
}
