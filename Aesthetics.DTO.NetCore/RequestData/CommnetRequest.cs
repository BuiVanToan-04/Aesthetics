using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.RequestData
{
    public class CommnetRequest
    {
		public int? ProductID { get; set; }
		public int? ServiceID { get; set; }
		public int UserID { get; set; }
		public string Comment_Content { get; set; }
	}

	public class Update_Comment
	{
		public int CommentID { get; set; }
		public int? ProductID { get; set; }
		public int? ServiceID { get; set; }
		public int UserID { get; set; }
		public string? Comment_Content { get; set; }
	}

	public class Delete_Comment
	{
		public int CommentID { get; set; }
	}

	public class GetList_SearchCommnet
	{
		public int? CommentID { get; set; }
		public int? ProductID { get; set; }
		public int? ServiceID { get; set; }
	}
}
