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
using XAct.Services;

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

		public async Task<ResponseData> Insert_Booking(BookingRequest insert_)
		{
			var returnData = new ResponseData();
			using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				if (insert_.ServiceID <= 0)
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
				if (!Validation.CheckString(insert_.UserName) || !Validation.CheckXSSInput(insert_.UserName))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu UserName không hợp lệ hoặc chứa kí tự không hợp lệ!";
					return returnData;
				}
				if (insert_.Email == null && insert_.Phone == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Vui lòng nhập Phone || Email để liên hệ!";
					return returnData;
				}
				if (insert_.Email != null && (!Validation.CheckString(insert_.Email) 
					|| !Validation.CheckXSSInput(insert_.Email)))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu Email không hợp lệ hoặc chứa kí tự không hợp lệ!";
					return returnData;
				}
				if (insert_.Phone != null && (!Validation.CheckXSSInput(insert_.Phone) 
					|| !Validation.CheckNumber(insert_.Phone)))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Phone không hợp lệ. Phone gồm các số(10-11số)!";
					return returnData;
				}
				if (insert_.ScheduledDate < DateTime.Now)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu ScheduledDate không hợp lệ!";
					return returnData;
				}

				var BookingCreation = DateTime.Now;
				//1. Lấy ProductsOfServicesID để so sánh ở bước 2
				var ProductsOfServicesID = await GetProductsOfServicesIDByServicesID(insert_.ServiceID);

				//2. Gen numberOrder qua ScheduledDate & ProductsOfServicesID
				var (numberOrder, mesage) = await GenerateNumberOrder(insert_.ScheduledDate, ProductsOfServicesID);
				if (mesage != null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = mesage;
					return returnData;
				}

				//3. Lấy ServiceName
				var serviceName = await GetServicessNameByID(insert_.ServiceID);

				//4. Thêm vào db
				var newBooking = new Booking
				{
					UserName = insert_.UserName,
					ServiceID = insert_.ServiceID,
					ServiceName = serviceName,
					TypeServicessID = ProductsOfServicesID,
					Email = insert_.Email,
					Phone = insert_.Phone,
					BookingCreation = BookingCreation,
					NumberOrder = numberOrder,
					ScheduledDate = insert_.ScheduledDate,
					DeleteStatus = 1
				};
				await _context.Booking.AddAsync(newBooking);
				await _context.SaveChangesAsync();

				//Lấy ClinicID
				var clinicID = await GetClinicByProductsOfServicesID(ProductsOfServicesID) ?? 0;

				//Thêm Booking_Assignment sau khi thêm Booking thành công
				var newBooking_Ass = new Booking_Assignment
				{
					BookingID = newBooking.BookingID,
					ClinicID = clinicID,
					UserName = insert_.UserName,
					ServiceName = serviceName,
					NumberOrder = numberOrder,
					AssignedDate = insert_.ScheduledDate,
					Status = 0
				};
				await _context.Booking_Assignment.AddAsync(newBooking_Ass);
				await _context.SaveChangesAsync();

				await transaction.CommitAsync(); //Commit transaction nếu thành công

				returnData.ResponseCode = 1;
				returnData.ResposeMessage = "Insert Booking & Booking_Assignment thành công!";
				return returnData;
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync(); 
				returnData.ResponseCode = -99;
				returnData.ResposeMessage = ex.Message;
				return returnData;
			}
		}


		public async Task<ResponseData> Update_Booking(Update_Booking update_)
		{
			var returnData = new ResponseData(); 
			using var transaction = await _context.Database.BeginTransactionAsync(); 
			try
			{
				// Tìm booking theo ID
				var booking = await _context.Booking.FindAsync(update_.BookingID);

				// Tìm booking assignment theo BookingID
				var booking_Ass = await GetBooking_AssignmentByBookingID(update_.BookingID);

				// Kiểm tra nếu không tìm thấy booking
				if (booking == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Booking không tồn tại!";
					return returnData;
				}

				// Kiểm tra nếu không tìm thấy booking assignment
				if (booking_Ass == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Booking_Assignment không tồn tại!";
					return returnData;
				}

				// Cập nhật BookingID trong Booking_Assignment
				booking_Ass.BookingID = update_.BookingID; 

				if (update_.ServiceID != null)
				{
					if (update_.ServiceID <= 0 || await _servicessRepository.GetServicessByServicesID(update_.ServiceID) == null)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu đầu vào ServiceID không hợp lệ hoặc Service không tồn tại!";
						return returnData;
					}
				}

				// Cập nhật ServiceID cho booking
				booking.ServiceID = update_.ServiceID; 

				if (!string.IsNullOrEmpty(update_.UserName) && Validation.CheckString(update_.UserName) && Validation.CheckXSSInput(update_.UserName))
				{
					booking.UserName = update_.UserName;
					booking_Ass.UserName = update_.UserName;
				}

				if (!string.IsNullOrEmpty(update_.Email) && Validation.CheckString(update_.Email) && Validation.CheckXSSInput(update_.Email))
				{
					booking.Email = update_.Email;
				}

				if (!string.IsNullOrEmpty(update_.Phone) && Validation.CheckXSSInput(update_.Phone) && Validation.CheckNumber(update_.Phone))
				{
					booking.Phone = update_.Phone;
				}

				if (update_.ScheduledDate <= DateTime.Now)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu ScheduledDate không hợp lệ. Vui lòng nhập lại ScheduledDate!";
					return returnData;
				}
				booking.ScheduledDate = update_.ScheduledDate;
				booking_Ass.AssignedDate = update_.ScheduledDate;

				var ServiceID = booking.ServiceID;
				var ScheduledDate = booking.ScheduledDate;

				//Lấy ProductsOfServicesID qua BooKingID
				var ProductsOfServicesID = await GetProductsOfServicesIDByBooKingID(update_.BookingID);
				//Lấy ProductsOfServicesID qua ServiceID
				var newProductsOfServicesID = await GetProductsOfServicesIDByServicesID(update_.ServiceID);
				// ==> Mục đích kiểm tra xem ProductsOfServicesID của Booking ban đầu và ProductsOfServicesID của
				// update_.ServiceID mới truyền vào có trùng nhau hay không ở (*)

				// Nếu ServiceID thay đổi thì cập nhật ServiceName
				if (update_.ServiceID != ServiceID)
				{
					var serviceName = await GetServicessNameByID(update_.ServiceID);
					booking.ServiceName = serviceName;
					booking_Ass.ServiceName = serviceName;
				}

				//(*) Kiểm tra nếu ngày đặt lịch hoặc loại dịch vụ thay đổi
				if (update_.ScheduledDate != ScheduledDate || newProductsOfServicesID != ProductsOfServicesID)
				{
					// Tạo số thứ tự mới cho booking
					var result = await GenerateNumberOrder(update_.ScheduledDate, newProductsOfServicesID);

					// Kiểm tra nếu vượt quá số lượng cho phép
					if (result.Item2 != null)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = result.Item2;
						return returnData;
					}

					// Cập nhật số thứ tự mới
					booking.NumberOrder = result.Item1;
					booking_Ass.NumberOrder = result.Item1;
					booking.TypeServicessID = newProductsOfServicesID;

					// Lấy ClinicID và cập nhật cho booking_Ass
					var clinicID = await GetClinicByProductsOfServicesID(newProductsOfServicesID) ?? 0;
					booking_Ass.ClinicID = clinicID;
				}

				// Cập nhật booking và booking assignment trong database
				_context.Booking.Update(booking);
				_context.Booking_Assignment.Update(booking_Ass);
				await _context.SaveChangesAsync();
				await transaction.CommitAsync();

				returnData.ResponseCode = 1;
				returnData.ResposeMessage = "Update thành công Booking & Booking_Assignment!";
				return returnData;
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				returnData.ResponseCode = -99;
				returnData.ResposeMessage = ex.Message;
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

		public async Task<int?> GetClinicByProductsOfServicesID(int? ProductsOfServicesID)
		{
			return await _context.Clinic.Where(s => s.ProductsOfServicesID == ProductsOfServicesID
						&& s.ClinicStatus == 1)
						.Select(a => a.ClinicID)
						.FirstOrDefaultAsync();
		}

		public async Task<string?> GetServicessNameByID(int? ServicessID)
		{
			return await _context.Servicess.Where(s => s.ServiceID == ServicessID 
					&& s.DeleteStatus ==1)
					.Select(v => v.ServiceName).FirstOrDefaultAsync();
		}

		public async Task<int?> GetProductsOfServicesIDByServicesID(int? ServicessID)
		{
			return await _context.Servicess
								 .Where(s => s.ServiceID == ServicessID && s.DeleteStatus ==1)
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

			//6. Qua 19h tối thì reset _currentNumberOrder về 1
			if (timeVietNam.Hour > 19)
			{
				_currentNumberOrder = 1;
			}
			return (_currentNumberOrder, null);
		}

		public async Task<int?> GetProductsOfServicesIDByBooKingID(int? BookingID)
		{
			return await _context.Booking.Where(s => s.BookingID == BookingID && s.DeleteStatus ==1)
					.Select(s => s.TypeServicessID)
					.FirstOrDefaultAsync();
		}

		public async Task<int?> GetServicesIDByBookingID(int? BookingID)
		{
			return await _context.Booking.Where(s => s.BookingID == BookingID
							&& s.DeleteStatus == 1)
					.Select(v => v.ServiceID)
					.FirstOrDefaultAsync();
		}

		public async Task<DateTime?> GetScheduledDateByBookingID(int? BookingID)
		{
			return await _context.Booking.Where(s => s.BookingID == BookingID 
						&& s.DeleteStatus == 1)
				.Select(v => v.ScheduledDate)
				.FirstOrDefaultAsync();
		}

		public async Task<Booking_Assignment> GetBooking_AssignmentByBookingID(int? BookingID)
		{
			return await _context.Booking_Assignment.Where(s => s.BookingID == BookingID).FirstOrDefaultAsync();
		}
	}
}
