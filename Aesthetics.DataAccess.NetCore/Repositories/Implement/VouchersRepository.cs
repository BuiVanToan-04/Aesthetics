using Aesthetics.DataAccess.NetCore.DBContext;
using Aesthetics.DataAccess.NetCore.Repositories.Interfaces;
using Aesthetics.DTO.NetCore.DataObject.LogginModel;
using Aesthetics.DTO.NetCore.DataObject.Model;
using Aesthetics.DTO.NetCore.RequestData;
using Aesthetics.DTO.NetCore.ResponseVouchers;
using BE_102024.DataAces.NetCore.CheckConditions;
using BE_102024.DataAces.NetCore.Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DataAccess.NetCore.Repositories.Implement
{
    public class VouchersRepository : BaseApplicationService, IVouchersRepository
	{
        private DB_Context _context;
        private IConfiguration _configuration;
		public VouchersRepository(DB_Context context, IConfiguration configuration,
			IServiceProvider serviceProvider) : base(serviceProvider) 
		{
			 _context = context;
			_configuration = configuration;
		}

		public async Task<ResponseVouchers_Loggin> Insert_Vouchers(VouchersRequest insert_)
		{
			var returnData = new ResponseVouchers_Loggin();
			var vouchers_Loggins = new List<Vouchers_Loggin>();
			try
			{
				if (!Validation.CheckString(insert_.Description) || !Validation.CheckXSSInput(insert_.Description)) 
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Description không hợp lệ!";
					return returnData;
				}
				if (insert_.DiscountValue <= 0 )
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "DiscountValue không hợp lệ!";
					return returnData;
				}
				if (insert_.StartDate < DateTime.Today || insert_.EndDate < DateTime.Today) 
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "StartDate || EndDate không hợp lệ!";
					return returnData;
				}
				if (insert_.StartDate > insert_.EndDate)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Hạn sử dụng Vouchers không hợp lệ!";
					return returnData;
				}
				if (insert_.MinimumOrderValue <= 0)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "MinimumOrderValue không hợp lệ!";
					return returnData;
				}
				if (!Validation.CheckString(insert_.RankMember) || !Validation.CheckXSSInput(insert_.RankMember))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "RankMember không hợp lệ!";
					return returnData;
				}
				if (!Validation.CheckString(insert_.VoucherImage))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "VoucherImage không hợp lệ!";
					return returnData;
				}
				var vouchersImagePath = await BaseProcessingFunction64(insert_.VoucherImage);
				var code = await GenCodeUnique();
				var newVouchers = new Vouchers
				{
					Code = code,
					Description = insert_.Description,
					VoucherImage = vouchersImagePath,
					DiscountValue = insert_.DiscountValue,
					StartDate = insert_.StartDate,
					EndDate = insert_.EndDate,
					MinimumOrderValue = insert_.MinimumOrderValue,
					RankMember = insert_.RankMember,
					IsActive = 1
				};
				_context.Vouchers.Add(newVouchers);
				await _context.SaveChangesAsync();
				vouchers_Loggins.Add(new Vouchers_Loggin
				{
					VoucherID = newVouchers.VoucherID,
					Code = newVouchers.Code,
					Description = newVouchers.Description,
					VoucherImage = newVouchers.VoucherImage,
					DiscountValue= newVouchers.DiscountValue,
					StartDate = newVouchers.StartDate,
					EndDate = newVouchers.EndDate,
					MinimumOrderValue = newVouchers.MinimumOrderValue,
					RankMember = newVouchers.RankMember,
					IsActive = newVouchers.IsActive,
				});
				returnData.ResponseCode = 1;
				returnData.ResposeMessage = "Insert Vouchers thành công!";
				returnData.vouchers_Loggins = vouchers_Loggins;
				return returnData;
			}
			catch (Exception ex)
			{
				throw new Exception($"Error in Insert_Vouchers Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}

		public Task<ResponseVouchers_Loggin> Update_Vouchers(Update_Vouchers update_)
		{
			throw new NotImplementedException();
		}

		public Task<ResponseVouchers_Loggin> Delete_Vouchers(Delete_Vouchers delete_)
		{
			throw new NotImplementedException();
		}

		public Task<ResponseVouchers_Loggin> GetList_SearchVouchers(GetList_SearchVouchers getList_)
		{
			throw new NotImplementedException();
		}

		public async Task<string> BaseProcessingFunction64(string? ServicessImage)
		{
			try
			{
				var path = "FilesImages/Servicess";

				if (!System.IO.Directory.Exists(path))
				{
					System.IO.Directory.CreateDirectory(path);
				}
				string imageName = Guid.NewGuid().ToString() + ".png";
				var imgPath = Path.Combine(path, imageName);

				if (ServicessImage.Contains("data:image"))
				{
					ServicessImage = ServicessImage.Substring(ServicessImage.LastIndexOf(',') + 1);
				}

				byte[] imageBytes = Convert.FromBase64String(ServicessImage);
				MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
				ms.Write(imageBytes, 0, imageBytes.Length);
				System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);

				image.Save(imgPath, System.Drawing.Imaging.ImageFormat.Png);

				return imageName;
			}
			catch (Exception ex)
			{
				throw new Exception("Lỗi khi lưu file: " + ex.Message);
			}
		}

		public async Task<string> GenCodeUnique()
		{
			string code;
			bool exists;
			Random random = new Random();
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

			do
			{
				code = new string(chars.OrderBy(x => random.Next()).Take(5).ToArray());
				exists = await _context.Vouchers.AnyAsync(s => s.Code == code);
			} while (exists);
			return code;
		}
	}
}
