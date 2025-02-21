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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XAct;
using XAct.Messages;
using XAct.Services;
using XAct.Users;

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

			// Bắt đầu một transaction để đảm bảo tính toàn vẹn dữ liệu
			using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				// Kiểm tra nếu danh sách dịch vụ rỗng hoặc null thì trả về lỗi
				if (insert_.ServiceIDs == null || insert_.ServiceIDs.Count == 0)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Vui lòng chọn ít nhất một Servicess!";
					return returnData;
				}
				// Kiểm tra UserName có hợp lệ hay không (không chứa ký tự đặc biệt, không bị XSS)
				if (!Validation.CheckString(insert_.UserName) || !Validation.CheckXSSInput(insert_.UserName))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu UserName không hợp lệ hoặc chứa kí tự không hợp lệ!";
					return returnData;
				}
				// Kiểm tra phải tồn tại 1 Email hoặc Phone
				if (insert_.Email == null && insert_.Phone == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Vui lòng nhập Phone || Email để liên hệ!";
					return returnData;
				}
				// Kiểm tra Email có hợp lệ hay không (không chứa ký tự đặc biệt, không bị XSS)
				if (insert_.Email != null && (!Validation.CheckString(insert_.Email)
					|| !Validation.CheckXSSInput(insert_.Email)))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu Email không hợp lệ hoặc chứa kí tự không hợp lệ!";
					return returnData;
				}
				// Kiểm tra Phone có hợp lệ hay không (không chứa ký tự đặc biệt, không bị XSS, đủ số)
				if (insert_.Phone != null && (!Validation.CheckXSSInput(insert_.Phone)
					|| !Validation.CheckNumber(insert_.Phone)))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Phone không hợp lệ. Phone gồm các số(10-11số)!";
					return returnData;
				}
				// Kiểm tra ngày đặt lịch không được nhỏ hơn ngày hiện tại
				if (insert_.ScheduledDate < DateTime.Now)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu ScheduledDate không hợp lệ!";
					return returnData;
				}

				//1.Lấy thời gian hiệ tại
				var bookingCreation = DateTime.Now;

				var newBooking = new Booking()
				{
					UserName = insert_.UserName,
					Email = insert_.Email ?? null,
					Phone = insert_.Phone ?? null,
					BookingCreation = bookingCreation,
					DeleteStatus = 1
				};
				await _context.Booking.AddAsync(newBooking);
				await _context.SaveChangesAsync();

				// Dictionary lưu NumberOrder theo từng ProductsOfServicesID
				var numberOrderMap = new Dictionary<int, int>();
				int numberOrder;
				//Dictionary numberOrderMap<int, int> lưu NumberOrder cho mỗi ProductsOfServicesID.
				//Key: ProductsOfServicesID
				//Value: NumberOrder
				//Kiểm tra nếu ProductsOfServicesID đã có NumberOrder
				//Nếu có → sử dụng lại.
				//Nếu chưa có → gọi GenerateNumberOrder() để tạo mới.

				//Duyệt qua list ServiceIDs đầu vào để xử lý các servicessID
				foreach (var servicessID in insert_.ServiceIDs)
				{
					//Kiểm tra servicessID có tồn tại hay kh?
					var servicess = await GetServicessByServicessID(servicessID);
					if (servicess == null)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = $"ServicessID: {servicessID} không tồn tại!";
						return returnData;
					}

					// Kiểm tra xem ProductsOfServicesID đã có NumberOrder chưa
					if (numberOrderMap.ContainsKey(servicess.ProductsOfServicesID))
					{
						// Nếu đã có, sử dụng lại NumberOrder
						numberOrder = numberOrderMap[servicess.ProductsOfServicesID];
					}
					else
					{
						// Nếu chưa có, tạo mới NumberOrder
						var (generatedNumberOrder, mesage) = await GenerateNumberOrder(insert_.ScheduledDate, servicess.ProductsOfServicesID);
						if (mesage != null)
						{
							returnData.ResponseCode = -1;
							returnData.ResposeMessage = mesage;
							return returnData;
						}

						numberOrder = generatedNumberOrder ?? 0;
						numberOrderMap[servicess.ProductsOfServicesID] = numberOrder;
					}

					var newBooking_Servicess = new Booking_Servicess
					{
						BookingID = newBooking.BookingID,
						ServiceID = servicess.ServiceID,
					};
					await _context.Booking_Servicess.AddAsync(newBooking_Servicess);
					await _context.SaveChangesAsync();

					var newBooking_Assignment = new Booking_Assignment
					{
						BookingID = newBooking.BookingID,
						ClinicID = await GetClinicIDByProductsOfServicesID(servicess.ProductsOfServicesID),
						ProductsOfServicesID = servicess.ProductsOfServicesID,
						UserName = insert_.UserName,
						ServiceName = servicess.ServiceName,
						NumberOrder = numberOrder,
						AssignedDate = insert_.ScheduledDate,
					};
					await _context.Booking_Assignment.AddAsync(newBooking_Assignment);
				}
				await _context.SaveChangesAsync();
				returnData.ResponseCode = 1;
				returnData.ResposeMessage = "Insert Booking thành công!";

				//Commit transaction nếu thành công
				await transaction.CommitAsync();
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

			// Bắt đầu một transaction để đảm bảo tính toàn vẹn dữ liệu
			using var transaction = await _context.Database.BeginTransactionAsync();

			try
			{
				// Lấy Booking từ database và lấy các bản ghi ở Booking_Assignment & Booking_Assignment
				// có liên quan đến BookingID
				var booking = await _context.Booking
					.Include(b => b.Booking_Assignment)
					.Include(b => b.Booking_Servicesses)
					.FirstOrDefaultAsync(b => b.BookingID == update_.BookingID);

				
				await _context.SaveChangesAsync();
				//Commit transaction nếu thành công
				await transaction.CommitAsync();

				returnData.ResponseCode = 1;
				returnData.ResposeMessage = "Cập nhật Booking thành công!";
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

		public async Task<(int? NumberOrder, string? Message)> GenerateNumberOrder(DateTime assignedDate, int? ProductsOfServicesID)
		{
			// 1. Lấy giờ Việt Nam
			var timeVietNam = DateTime.UtcNow.AddHours(7);

			// 2. Số thứ tự hiện tại
			int _currentNumberOrder = 1;

			// 3. Lấy NumberOrder lớn nhất trong ScheduledDate
			var latestBooking = await _context.Booking_Assignment
				.Where(s => s.AssignedDate.Date == assignedDate.Date && s.ProductsOfServicesID == ProductsOfServicesID)
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
				return (null, $"Ngày: {assignedDate.Date} đã đủ lượt Booking. Vui lòng chọn ngày khác!");
			}

			//6. Qua 19h tối thì reset _currentNumberOrder về 1
			if (timeVietNam.Hour > 19)
			{
				_currentNumberOrder = 1;
			}
			return (_currentNumberOrder, null);
		}

		public async Task<Servicess> GetServicessByServicessID(int servicessID)
		{
			return await _context.Servicess.Where(s => s.ServiceID == servicessID
					&& s.DeleteStatus == 1).FirstOrDefaultAsync();
		}

		public async Task<int> GetClinicIDByProductsOfServicesID(int? ProductsOfServicesID)
		{
			return await _context.Clinic.Where(s => s.ClinicStatus == 1
					&& s.ProductsOfServicesID == ProductsOfServicesID)
				.Select(s => s.ClinicID)
				.FirstOrDefaultAsync();
		}

		public async Task<Booking> GetBooKingByBookingID(int? BookingID)
		{
			return await _context.Booking.Where(s => s.BookingID == BookingID
				&& s.DeleteStatus == 1).FirstOrDefaultAsync();
		}
	}
}
