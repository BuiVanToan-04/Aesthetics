using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject.LogginModel
{
	public class Carts_Loggin
	{
		public int CartID { get; set; }
		public int UserID { get; set; }
		public DateTime CreationDate { get; set; }
	}
}
