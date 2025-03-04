using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject.Model
{
	public class Clinic_Staff
	{
		[Key]
		public int ClinicStaffID { get; set; }
		public int? ClinicID { get; set; }
		public int? UserID { get; set; }
		public int? DeleteStatus { get; set; }
		public Users Users { get; set; }
		public Clinic Clinic { get; set; } 
	}
}
