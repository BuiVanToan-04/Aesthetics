using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DTO.NetCore.DataObject.LogginModel;
using Aesthetics.DTO.NetCore.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.ResponesComment
{
	public class ResponesComment
	{
		public int CommentID { get; set; }
		public string? ProductName { get; set; }
		public string? ServiceName { get; set; }
		public string? UserName { get; set; }
		public string? Comment_Content { get; set; }
		public DateTime? CreationDate { get; set; }
	}

	public class ResponseGetList_SearchComment : ResponseData
	{
		public List<ResponesComment>? Data { get; set; }
	}
    public class ResponesCommentData : ResponseData
    {
		public List<Comment_Loggin>? comment_Loggins { get; set; }
	}
}
