using Aesthetics.DataAccess.NetCore.DBContext;
using Aesthetics.DataAccess.NetCore.Repositories.Interface;
using Aesthetics.DataAccess.NetCore.Repositories.Interfaces;
using Aesthetics.DTO.NetCore.DataObject.LogginModel;
using Aesthetics.DTO.NetCore.DataObject.Model;
using Aesthetics.DTO.NetCore.RequestData;
using Aesthetics.DTO.NetCore.ResponesComment;
using BE_102024.DataAces.NetCore.CheckConditions;
using BE_102024.DataAces.NetCore.Dapper;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DataAccess.NetCore.Repositories.Implement
{
	public class CommentRepository : BaseApplicationService, ICommentRepository
	{
		private DB_Context _context;
		private IConfiguration _configuration;
		private IServicessRepository _servicessRepository;
		private IProductsRepository _productsRepository;
		private IUserRepository _userRepository;
		public CommentRepository(DB_Context context, IConfiguration configuration,
			IServiceProvider serviceProvider, IProductsRepository productsRepository,
			IServicessRepository servicessRepository, IUserRepository userRepository) : base(serviceProvider)
		{
			_context = context;
			_configuration = configuration;
			_productsRepository = productsRepository;
			_servicessRepository = servicessRepository;
			_userRepository = userRepository;
		}

		public async Task<ResponesCommentData> Insert_Comment(CommnetRequest _comment)
		{
			var responseData = new ResponesCommentData();
			var comment_Loggin = new List<Comment_Loggin>();
			try
			{
				var newComment = new Comments();
				if (_comment.ProductID != null)
				{
					if (await _productsRepository.GetProductsByProductID(_comment.ProductID) == null)
					{
						responseData.ResponseCode = -1;
						responseData.ResposeMessage = "Product không tồn tại!";
						return responseData;
					}
					newComment.ProductID = _comment.ProductID;
				}
				if (_comment.ServiceID != null)
				{
					if (await _servicessRepository.GetServicessByServicesID(_comment.ServiceID) == null)
					{
						responseData.ResponseCode = -1;
						responseData.ResposeMessage = "Sevicess không tồn tại!";
						return responseData;
					}
					newComment.ServiceID = _comment.ServiceID;
				}
				if (_comment.ServiceID != null && _comment.ProductID != null)
				{
					responseData.ResponseCode = -1;
					responseData.ResposeMessage = "Chỉ comment được 1 nội dung!";
					return responseData;
				}
				if (_comment.ServiceID == null && _comment.ProductID == null)
				{
					responseData.ResponseCode = -1;
					responseData.ResposeMessage = "Vui lòng chọn 1 nội dung để comment!";
					return responseData;
				}
				if (await _userRepository.GetUserByUserID(_comment.UserID) == null)
				{
					responseData.ResponseCode = -1;
					responseData.ResposeMessage = "User không tồn tại!";
					return responseData;
				}
				newComment.UserID = _comment.UserID;
				if (_comment.Comment_Content == null)
				{
					responseData.ResponseCode = -1;
					responseData.ResposeMessage = "Nhập nội dùng comment ...!";
					return responseData;
				}
				if (!Validation.CheckString(_comment.Comment_Content) || !Validation.CheckXSSInput(_comment.Comment_Content))
				{
					responseData.ResponseCode = -1;
					responseData.ResposeMessage = "Comment không hợp lệ || chứa kí tự không hợp lệ!";
					return responseData;
				}
				newComment.Comment_Content = _comment.Comment_Content;
				newComment.CreationDate = DateTime.Now;
				await _context.Comments.AddAsync(newComment);
				await _context.SaveChangesAsync();
				comment_Loggin.Add(new Comment_Loggin
				{
					CommentID = newComment.CommentID,
					ProductID = newComment.ProductID,
					ServiceID = newComment.ServiceID,
					UserID = newComment.UserID,
					Comment_Content = newComment.Comment_Content,
					CreationDate = newComment.CreationDate,
				});
				responseData.ResponseCode = 1;
				responseData.comment_Loggins = comment_Loggin;
				responseData.ResposeMessage = "Insert Comment thành công!";
				return responseData;
			}
			catch (Exception ex)
			{
				throw new Exception($"Error in Insert_Comment Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}

		public async Task<ResponesCommentData> Update_Comment(Update_Comment _comment)
		{
			var responseData = new ResponesCommentData();
			var comment_Loggin = new List<Comment_Loggin>();
			try
			{
				// Lấy comment hiện có từ DB
				var existingComment = await GetCommentByCommentID(_comment.CommentID);
				if (existingComment == null)
				{
					responseData.ResponseCode = -1;
					responseData.ResposeMessage = "CommentID không hợp lệ || không tồn tại!";
					return responseData;
				}

				// Kiểm tra nội dung bình luận hợp lệ
				if (string.IsNullOrWhiteSpace(_comment.Comment_Content))
				{
					responseData.ResponseCode = -1;
					responseData.ResposeMessage = "Nhập nội dung comment ...!";
					return responseData;
				}
				if (!Validation.CheckString(_comment.Comment_Content) || !Validation.CheckXSSInput(_comment.Comment_Content))
				{
					responseData.ResponseCode = -1;
					responseData.ResposeMessage = "Comment không hợp lệ || chứa kí tự không hợp lệ!";
					return responseData;
				}

				existingComment.Comment_Content = _comment.Comment_Content;
				existingComment.CreationDate = DateTime.Now;

				await _context.SaveChangesAsync();

				comment_Loggin.Add(new Comment_Loggin
				{
					CommentID = existingComment.CommentID,
					Comment_Content = existingComment.Comment_Content,
					CreationDate = existingComment.CreationDate,
				});

				responseData.ResponseCode = 1;
				responseData.comment_Loggins = comment_Loggin;
				responseData.ResposeMessage = "Update Comment thành công!";
				return responseData;
			}
			catch (Exception ex)
			{
				throw new Exception($"Error in Update_Comment Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}


		public async Task<ResponesCommentData> Delete_Comment(Delete_Comment _comment)
		{
			var responseData = new ResponesCommentData();
			var comment_Loggin = new List<Comment_Loggin>();
			try
			{
				if (_comment.CommentID <= 0)
				{
					responseData.ResponseCode = -1;
					responseData.ResposeMessage = "CommentID không hợp lệ!";
					return responseData;
				}
				var resultComment = await _context.Comments.FindAsync(_comment.CommentID);
				if (resultComment != null)
				{
					_context.Comments.Remove(resultComment);
					await _context.SaveChangesAsync();
					comment_Loggin.Add(new Comment_Loggin
					{
						CommentID = resultComment.CommentID,
						ProductID = resultComment.ProductID,
						ServiceID = resultComment.ServiceID,
						UserID = resultComment.UserID,
						Comment_Content = resultComment.Comment_Content,
						CreationDate = resultComment.CreationDate,
					});
					responseData.ResponseCode = 1;
					responseData.comment_Loggins = comment_Loggin;
					responseData.ResposeMessage = "Delete Comment thành công!";
					return responseData;
				}
				responseData.ResponseCode = 0;
				responseData.ResposeMessage = $"Không tìm thấy commentID: {_comment.CommentID}!";
				return responseData;
			}
			catch (Exception ex)
			{
				throw new Exception($"Error in Delete_Comment Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}

		public async Task<ResponseGetList_SearchComment> GetList_SearchComment(GetList_SearchCommnet _searchComment)
		{
			var returnData = new ResponseGetList_SearchComment();
			try
			{
				if (_searchComment.CommentID != null)
				{
					if (await GetCommentByCommentID(_searchComment.CommentID) == null)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Không tìm thấy CommentID!";
						return returnData;
					}
				}
				if (_searchComment.ProductID != null)
				{
					if (await _productsRepository.GetProductsByProductID(_searchComment.ProductID) == null)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Không tìm thấy ProductID!";
						return returnData;
					}
				}
				if (_searchComment.ServiceID != null)
				{
					if (await _servicessRepository.GetServicessByServicesID(_searchComment.ServiceID) == null)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Không tìm thấy ServiceID!";
						return returnData;
					}
				}
				var parameters = new DynamicParameters();
				parameters.Add("@CommentID", _searchComment.CommentID ?? null);
				parameters.Add("@ProductID", _searchComment.ProductID ?? null);
				parameters.Add("@ServiceID", _searchComment.ServiceID ?? null);
				var result = await DbConnection.QueryAsync<ResponesComment>("GetList_SearchComment", parameters);
				if (result != null && result.Any())
				{
					returnData.ResponseCode = 1;
					returnData.ResposeMessage = "Get list Comments thành công!";
					returnData.Data = result.ToList();
					return returnData;
				}
				returnData.ResponseCode = 0;
				returnData.ResposeMessage = "Không tìm thấy Comments nào.";
				return returnData;
			}
			catch (Exception ex)
			{
				throw new Exception($"Error in GetList_SearchComment Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}

		public async Task<Comments> GetCommentByCommentID(int? CommentID)
		{
			return await _context.Comments.Where(s => s.CommentID == CommentID).FirstOrDefaultAsync();
		}
	}
}
