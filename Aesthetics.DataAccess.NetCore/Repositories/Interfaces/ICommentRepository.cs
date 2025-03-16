using Aesthetics.DTO.NetCore.DataObject.Model;
using Aesthetics.DTO.NetCore.RequestData;
using Aesthetics.DTO.NetCore.ResponesComment;
using Aesthetics.DTO.NetCore.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DataAccess.NetCore.Repositories.Interfaces
{
    public interface ICommentRepository
    {
		//1.Function thêm Comment
		Task<ResponesCommentData> Insert_Comment(CommnetRequest _comment);

		//2.Function cập nhật Comment
		Task<ResponesCommentData> Update_Comment(Update_Comment _comment);

		//3.Function xóa Comment
		Task<ResponesCommentData> Delete_Comment(Delete_Comment _comment);

		//4.Get list Comment & Search Comment by CommentName or CommentID
		Task<ResponseGetList_SearchComment> GetList_SearchComment(GetList_SearchCommnet _searchComment);

		//5 Get comment by commentID
		Task<Comments> GetCommentByCommentID (int? CommentID);
	}
}
