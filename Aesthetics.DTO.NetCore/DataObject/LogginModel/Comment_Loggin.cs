using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject.LogginModel
{
    public class Comment_Loggin
    {
		public int? CommentID { get; set; }
		public int? ProductID { get; set; }
		public int? ServiceID { get; set; }
		public int? UserID { get; set; }
		public string? Comment_Content { get; set; }
		public DateTime? CreationDate { get; set; }
	}
}
