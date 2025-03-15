using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject.LogginModel
{
    public class CartProducts_Loggin 
    {
		public int? CartProductID { get; set; }
		public int? CartID { get; set; }
		public int? ProductID { get; set; }
		public int? Quantity { get; set; }
		public DateTime? CreateDay { get; set; }
	}
}
