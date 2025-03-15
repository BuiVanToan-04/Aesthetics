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
		//3.Function thêm Comment
		Task<ResponesComment> Insert_Comment(CommentRequest Comment);

		//4.Function cập nhật Comment
		Task<ResponesComment> Update_Comment(Update_Comment Comment);

		//5.Function xóa Comment
		Task<ResponesComment> Delete_Comment(Delete_Comment Comment);

		//6.Get list Comment & Search Comment by CommentName or CommentID
		Task<ResponesComment> GetList_SearchComment(GetList_SearchComment _searchComment);
	}
}
