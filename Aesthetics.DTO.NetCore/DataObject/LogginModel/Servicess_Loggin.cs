using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject.LogginModel
{
    public class Servicess_Loggin
    {
		public int ServiceID { get; set; }
		public int ProductsOfServicesID { get; set; }
		public string ServiceName { get; set; }
		public string Description { get; set; }
		public string ServiceImage { get; set; }
		public double PriceService { get; set; }
		public int DeleteStatus { get; set; }
	}
}
