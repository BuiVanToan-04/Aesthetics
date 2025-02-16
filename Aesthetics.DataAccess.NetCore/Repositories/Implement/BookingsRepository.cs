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

		public async Task<string?> GetServicessNameByID(int? ServicessID)
		{
			return await _context.Servicess.Where(s => s.ServiceID == ServicessID)
				.Select(v => v.ServiceName).FirstOrDefaultAsync();
		}

		public async Task<int?> GetProductsOfServicesIDByServicesID(int? ServicessID)
		{
			return await _context.Servicess
								 .Where(s => s.ServiceID == ServicessID)
								 .Select(s => s.TypeProductsOfServices.ProductsOfServicesID)
								 .FirstOrDefaultAsync();
		}

		public async Task<Booking> GetBookingByID(int? BookingID)
		{
			return await _context.Booking.Where(s => s.BookingID == BookingID
				&& s.DeleteStatus == 1).FirstOrDefaultAsync();
		}

		public async Task<(int? NumberOrder, string? Message)> GenerateNumberOrder(DateTime scheduledDate, int? TypeServicessID)
		{
			// 1. Lấy giờ Việt Nam
			var timeVietNam = DateTime.UtcNow.AddHours(7);

			// 2. Số thứ tự hiện tại
			int _currentNumberOrder = 1;

			// 3. Lấy NumberOrder lớn nhất trong ScheduledDate
			var latestBooking = await _context.Booking
				.Where(s => s.ScheduledDate.Date == scheduledDate.Date && s.TypeServicessID == TypeServicessID)
				.OrderByDescending(v => v.NumberOrder)
				.FirstOrDefaultAsync();

			// 4. Nếu tồn tại booking trong ngày thì tăng NumberOrder
			if (latestBooking != null)
			{
				_currentNumberOrder = (latestBooking.NumberOrder ?? 0) + 1;
			}

			// 5. Kiểm tra xem số thứ tự có vượt quá 100 không
			if (_currentNumberOrder > 100)
			{
				return (null, $"Ngày: {scheduledDate.Date} đã đủ lượt Booking. Vui lòng chọn ngày khác!");
			}

			return (_currentNumberOrder, null);
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
				//1.Lấy ProductsOfServicesID để so sánh ở bước 2
				var ProductsOfServicesID = await GetProductsOfServicesIDByServicesID(insert_.ServiceID);

				//2. Gen numberOrder qua ScheduledDate & ProductsOfServicesID
				var (numberOrder,mesage) = await GenerateNumberOrder(insert_.ScheduledDate, ProductsOfServicesID);
				if (mesage != null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = mesage;
					return returnData;
				}

				//3.Lấy ServiceName
				var ServiceName = await GetServicessNameByID(insert_.ServiceID);

				var parameters = new DynamicParameters();
				parameters.Add("@UserName",insert_.UserName);
				parameters.Add("@ServiceID", insert_.ServiceID);
				parameters.Add("@ServiceName", ServiceName);
				parameters.Add("@TypeServicessID", ProductsOfServicesID);
				parameters.Add("@Email", insert_.Email ?? null);
				parameters.Add("@Phone", insert_.Phone ?? null);
				parameters.Add("@BookingCreation", BookingCreation);
				parameters.Add("@ScheduledDate", insert_.ScheduledDate);
				parameters.Add("@NumberOrder", numberOrder);

				await DbConnection.ExecuteAsync("Insert_Booking", parameters);
				returnData.ResponseCode = 1;
				returnData.ResposeMessage = "Insert Booking thành công!";
				return returnData;
			}
			catch (Exception ex) 
			{
				returnData.ResponseCode = -99;
				returnData.ResposeMessage = ex.Message;
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
