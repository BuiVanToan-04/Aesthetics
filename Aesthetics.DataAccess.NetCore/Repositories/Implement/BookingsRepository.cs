using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DataAccess.NetCore.DBContext;
using Aesthetics.DataAccess.NetCore.Repositories.Interfaces;
using Aesthetics.DTO.NetCore.DataObject;
using Aesthetics.DTO.NetCore.RequestData;
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
	public class BookingsRepository : BaseApplicationService, IBookingsRepository
	{
		private DB_Context _context;
		private IConfiguration _configuration;
		private IServicessRepository _servicessRepository;
		public BookingsRepository(DB_Context context, IServiceProvider serviceProvider,
			IConfiguration configuration, IServicessRepository servicessRepository) : base(serviceProvider)
		{
			_context = context;
			_configuration = configuration;
			_servicessRepository = servicessRepository;
		}

		public async Task<Booking> GetBookingByID(int? BookingID)
		{
			return await _context.Booking.Where(s => s.BookingID == BookingID
				&& s.DeleteStatus == 1).FirstOrDefaultAsync();
		}

		public async Task<int?> GenerateNumberOrder()
		{
			//var timeVietNam = DateTime.UtcNow.AddHours(7).Date;
			//var latestBooking = await _context.Booking
			//	.Where(b => b.BookingCreation >= timeVietNam)
			//	.OrderByDescending(a => a.NumberOrder)
			//	.FirstOrDefaultAsync();
			//return latestBooking == null ? 1 : latestBooking.NumberOrder + 1;

			//1. Lấy giờ việt nam
			var timeVietNam = DateTime.UtcNow.AddHours(7).Date;
			//2. Lưu thời gian cuối cùng mà NumberOrder được reset
			var _lastResetTime = DateTime.MinValue;
			//3. NumberOrder hiện tại
			int _currentNumberOrder = 1;
			//4. Kiểm tra xem có cần reset NumberOrder không?
			if (_lastResetTime.Date != timeVietNam.Date || timeVietNam.Hour > 19)
			{
				_currentNumberOrder = 1;
				_lastResetTime = timeVietNam; //cập nhật lại thời gian cuối cùng cho NumberOrder
			}

			//5.Lấy số thứ tự lớn nhất trong ngày hiện tại
			var latestBooking = await _context.Booking
					.Where(s => s.BookingCreation.Date == timeVietNam.Date)
					.OrderByDescending(a => a.NumberOrder)
					.FirstOrDefaultAsync();
			//6. Nếu tồn tại booking trong ngày thì tăng NumberOrder
			if(latestBooking != null)
			{
				_currentNumberOrder = latestBooking.NumberOrder + 1;
			}
			return _currentNumberOrder;
		}

		public async Task<ResponseData> Insert_Booking(BookingRequest insert_)
		{
			var returnData = new ResponseData();
			try
			{
				if(insert_.ServiceID <=0)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu đầu vào ServiceID không hợp lệ!";
					return returnData;
				}
				if (await _servicessRepository.GetServicessByServicesID(insert_.ServiceID) == null) 
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Service không tồn tại!";
					return returnData;
				}
				if(!Validation.CheckString(insert_.UserName))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu đầu vào UserName không hợp lệ!";
					return returnData;
				}
				if (!Validation.CheckXSSInput(insert_.UserName)) 
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu UserName chứa kí tự không hợp lệ!";
					return returnData;
				}
				if (insert_.Email == null & insert_.Phone == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Vui lòng nhập Phone || Email để liên hệ!";
					return returnData;
				}
				if (insert_.Email != null) 
				{
					if (!Validation.CheckString(insert_.Email))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu đầu vào Email không hợp lệ!";
						return returnData;
					}
					if (!Validation.CheckXSSInput(insert_.Email))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu Email chứa kí tự không hợp lệ!";
						return returnData;
					}
				}
				if (insert_.Phone != null)
				{
					if (!Validation.CheckXSSInput(insert_.Phone))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu Phone chứa kí tự không hợp lệ!";
						return returnData;
					}
					if (!Validation.CheckNumber(insert_.Phone))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Phone không hợp lệ. Phone gồm các số(10-11số)!";
						return returnData;
					}
				}
				if (insert_.ScheduledDate < DateTime.Now) 
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu ScheduledDate không hợp lệ!";
					return returnData;
				}

				var BookingCreation = DateTime.Now;
				var NumberOrder = await GenerateNumberOrder();

				var parameters = new DynamicParameters();
				parameters.Add("@ServiceID", insert_.ServiceID);
				parameters.Add("@UserName", insert_.UserName);
				parameters.Add("@Email", insert_.Email ?? null);
				parameters.Add("@Phone", insert_.Phone ?? null);
				parameters.Add("@BookingCreation", BookingCreation);
				parameters.Add("@NumberOrder", NumberOrder);
				parameters.Add("@ScheduledDate", insert_.ScheduledDate);
				await DbConnection.ExecuteAsync("Insert_Booking", parameters);
				returnData.ResponseCode = 1;
				returnData.ResposeMessage = "Insert Booking thành công!";
				return returnData;
			}
			catch (Exception ex) 
			{
				returnData.ResponseCode = -99;
				returnData.ResposeMessage = ex.StackTrace;
				return returnData;
			}
		}

		public async Task<ResponseData> Update_Booking(Update_Booking update_)
		{
			var returnData = new ResponseData();
			try
			{
				if (update_.BookingID <= 0)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu đầu vào BookingID không hợp lệ!";
					return returnData;
				}
				if(update_.ServiceID != null)
				{
					if (update_.ServiceID <= 0)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu đầu vào ServiceID không hợp lệ!";
						return returnData;
					}
					if (await _servicessRepository.GetServicessByServicesID(update_.ServiceID) == null)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Service không tồn tại!";
						return returnData;
					}
				}
				if (update_.UserName != null)
				{
					if (!Validation.CheckString(update_.UserName))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu đầu vào UserName không hợp lệ!";
						return returnData;
					}
					if (!Validation.CheckXSSInput(update_.UserName))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu UserName chứa kí tự không hợp lệ!";
						return returnData;
					}
				}
				if (update_.Email != null)
				{
					if (!Validation.CheckString(update_.Email))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu đầu vào Email không hợp lệ!";
						return returnData;
					}
					if (!Validation.CheckXSSInput(update_.Email))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu Email chứa kí tự không hợp lệ!";
						return returnData;
					}
				}
				if (update_.Phone != null)
				{
					if (!Validation.CheckXSSInput(update_.Phone))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu Phone chứa kí tự không hợp lệ!";
						return returnData;
					}
					if (!Validation.CheckNumber(update_.Phone))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Phone không hợp lệ. Phone gồm các số(10-11số)!";
						return returnData;
					}
				}
				if (update_.ScheduledDate != null)
				{
					if (update_.ScheduledDate < DateTime.Now)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu ScheduledDate không hợp lệ!";
						return returnData;
					}
				}
				var parameters = new DynamicParameters();
				parameters.Add("@ServiceID", update_.ServiceID);
				parameters.Add("@UserName", update_.UserName);
				parameters.Add("@Email", update_.Email ?? null);
				parameters.Add("@Phone", update_.Phone ?? null);
				parameters.Add("@ScheduledDate", update_.ScheduledDate);
				await DbConnection.ExecuteAsync("Insert_Booking", parameters);
				returnData.ResponseCode = 1;
				returnData.ResposeMessage = "Update Booking thành công!";
				return returnData;
			}
			catch (Exception ex)
			{
				returnData.ResponseCode = -99;
				returnData.ResposeMessage = ex.StackTrace;
				return returnData;
			}
		}

		public Task<ResponseData> Delete_Booking(Delete_Booking delete_)
		{
			throw new NotImplementedException();
		}

		public Task<ResponseData> GetList_SearchBooking(GetList_SearchBooking getList_)
		{
			throw new NotImplementedException();
		}

		
	}
}
