﻿using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DataAccess.NetCore.DBContext;
using Aesthetics.DataAccess.NetCore.Repositories.Interfaces;
using Aesthetics.DTO.NetCore.DataObject;
using Aesthetics.DTO.NetCore.RequestData;
using Aesthetics.DTO.NetCore.Response;
using BE_102024.DataAces.NetCore.CheckConditions;
using BE_102024.DataAces.NetCore.Dapper;
using Dapper;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Wordprocessing;
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
		private IClinicRepository _clinicRepository;
		public BookingsRepository(DB_Context context, IServiceProvider serviceProvider,
			IConfiguration configuration, IServicessRepository servicessRepository,
			IClinicRepository clinicRepository) : base(serviceProvider)
		{
			_context = context;
			_configuration = configuration;
			_servicessRepository = servicessRepository;
			_clinicRepository = clinicRepository;
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
						ProductsOfServicesID = servicess.ProductsOfServicesID,
						DeleteStatus = 1
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
						DeleteStatus = 1
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

			//1.Bắt đầu một transaction để đảm bảo tính toàn vẹn dữ liệu
			using var transaction = await _context.Database.BeginTransactionAsync();

			try
			{
				//2..Lấy Booking từ database và lấy các bản ghi ở Booking_Assignment & Booking_Assignment
				// có liên quan đến BookingID
				var booking = await _context.Booking
					.Include(b => b.Booking_Assignment)
					.Include(b => b.Booking_Servicesses)
					.AsSplitQuery()
					.FirstOrDefaultAsync(b => b.BookingID == update_.BookingID);

				if (booking == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"BookingID: {update_.BookingID} không tồn tại!";
					return returnData;
				}

				if (!string.IsNullOrEmpty(update_.UserName))
				{
					if (!Validation.CheckString(update_.UserName) || !Validation.CheckXSSInput(update_.UserName))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "UserName không hợp lệ!";
						return returnData;
					}
					booking.UserName = update_.UserName;
				}

				if (!string.IsNullOrEmpty(update_.Email))
				{
					if (!Validation.CheckString(update_.Email) || !Validation.CheckXSSInput(update_.Email))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Email không hợp lệ!";
						return returnData;
					}
					booking.Email = update_.Email;
				}

				if (!string.IsNullOrEmpty(update_.Phone))
				{
					if (!Validation.CheckXSSInput(update_.Phone) || !Validation.CheckNumber(update_.Phone))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Phone không hợp lệ!";
						return returnData;
					}
					booking.Phone = update_.Phone;
				}

				if (update_.ScheduledDate < DateTime.Now)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "ScheduledDate không hợp lệ!";
					return returnData;
				}

				//3.Update booking
				_context.Booking.Update(booking);
				await _context.SaveChangesAsync();

				//4. Nếu ServiceIDs khác null & ServiceIDs.Count > 0
				//=> Update Booking_Servicess & Booking_Assignment
				if (update_.ServiceIDs != null && update_.ServiceIDs.Count > 0)
				{
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
					foreach (var servicessID in update_.ServiceIDs)
					{
						//4.1 Update Booking_Assignment
						var servicess = await GetServicessByServicessID(servicessID);
						if (servicess == null)
						{
							returnData.ResponseCode = -1;
							returnData.ResposeMessage = $"ServicessID: {servicessID} không tồn tại!";
							return returnData;
						}

						//Kiểm tra xem Booking_Assignment đã có NumberOrder cho ProductsOfServicesID này chưa
						var checkNumber_Assignment = booking.Booking_Assignment
							.FirstOrDefault(s => s.ProductsOfServicesID == servicess.ProductsOfServicesID);

						if (checkNumber_Assignment != null)
						{
							//Nếu đã có NumberOrder, sử dụng lại
							numberOrder = checkNumber_Assignment.NumberOrder ?? 0;
						}
						else if (numberOrderMap.ContainsKey(servicess.ProductsOfServicesID))
						{
							//Nếu đã có trong dictionary, sử dụng lại
							numberOrder = numberOrderMap[servicess.ProductsOfServicesID];
						}
						else
						{
							//Nếu không có ở đâu cả, tạo NumberOrder mới
							var (generatedNumberOrder, mesage) = await GenerateNumberOrder(update_.ScheduledDate, servicess.ProductsOfServicesID);
							if (mesage != null)
							{
								returnData.ResponseCode = -1;
								returnData.ResposeMessage = mesage;
								return returnData;
							}
							numberOrder = generatedNumberOrder ?? 0;
							numberOrderMap[servicess.ProductsOfServicesID] = numberOrder;
						}

						//Lấy tất cả Booking_Assignment có liên quan đến bookingID
						var book_Assi = booking.Booking_Assignment
							.Where(s => s.BookingID == booking.BookingID).ToList();
						if (book_Assi.Any())
						{
							foreach (var book in book_Assi)
							{
								book.BookingID = booking.BookingID;
								book.UserName = update_.UserName;
								book.AssignedDate = update_.ScheduledDate;
							}

							//Lấy tất cả Booking_Assignment có liên quan đến ProductsOfServicesID
							var booking_Assignment = booking.Booking_Assignment
								.Where(s => s.ProductsOfServicesID == servicess.ProductsOfServicesID).ToList();
							if (booking_Assignment.Any())
							{
								//Nếu có booking_Assignment thì update
								foreach (var item in booking_Assignment)
								{
									item.ClinicID = await GetClinicIDByProductsOfServicesID(servicess.ProductsOfServicesID);
									item.ProductsOfServicesID = servicess.ProductsOfServicesID;
									item.ServiceName = servicess.ServiceName;
									item.NumberOrder = numberOrder;
								}
							}
							else
							{
								// Thêm mới nếu không có booking_Assignment nào
								var newAssignment = new Booking_Assignment
								{
									BookingID = booking.BookingID,
									ClinicID = await GetClinicIDByProductsOfServicesID(servicess.ProductsOfServicesID),
									ProductsOfServicesID = servicess.ProductsOfServicesID,
									UserName = update_.UserName,
									ServiceName = servicess.ServiceName,
									NumberOrder = numberOrder,
									AssignedDate = update_.ScheduledDate,
									Status = 0,
									DeleteStatus = 1
								};
								await _context.Booking_Assignment.AddAsync(newAssignment);
							}
						}



						//4.2 Update Booking_Servicess
						//Lấy tất cả Booking_Servicess có liên quan đên booking
						var booking_Servicess = booking.Booking_Servicesses
							.Where(s => s.BookingID == booking.BookingID
							&& s.ProductsOfServicesID == servicess.ProductsOfServicesID).ToList();
						if (booking_Servicess.Any())
						{
							//Nếu có booking_Servicess thì update
							foreach (var booking_Ser in booking_Servicess)
							{
								booking_Ser.BookingID = booking.BookingID;
								booking_Ser.ServiceID = servicess.ServiceID;
								booking_Ser.ProductsOfServicesID = servicess.ProductsOfServicesID;
							}
						}
						else
						{
							var newbookingServicess = new Booking_Servicess
							{
								BookingID = booking.BookingID,
								ServiceID = servicess.ServiceID,
								ProductsOfServicesID = servicess.ProductsOfServicesID,
								DeleteStatus = 1
							};
							await _context.Booking_Servicess.AddAsync(newbookingServicess);
						}

					}
				}

				//Commit transaction nếu thành công
				await transaction.CommitAsync();
				await _context.SaveChangesAsync();

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

		public async Task<ResponseData> Delete_Booking(Delete_Booking delete_)
		{
			var returnData = new ResponseData();
			//1.Bắt đầu một transaction để đảm bảo tính toàn vẹn dữ liệu
			using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				//Lấy bản ghi ở booking và những bản ghi có liên quan đến delete_.BookingID
				//ở Booking_Assignment & Booking_Servicesses
				var booking = await _context.Booking
					.Include(x => x.Booking_Assignment)
					.Include(x => x.Booking_Servicesses)
					.AsSplitQuery()
					.FirstOrDefaultAsync(x => x.BookingID == delete_.BookingID);

				if (booking != null)
				{
					booking.DeleteStatus = 0;
					await _context.SaveChangesAsync();

					//Xóa những bản ghi ở Booking_Servicesses có liên quan đến delete_.BookingID
					var booking_Servicess = booking.Booking_Servicesses
						.Where(s => s.BookingID == booking.BookingID).ToList();

					//Nếu booking_Servicess tồn tại 
					if (booking_Servicess.Any())
					{
						foreach (var item in booking_Servicess)
						{
							item.DeleteStatus = 0;
						}
					}

					//Xóa những bản ghi ở Booking_Assignment có liên quan đến delete_.BookingID
					var booking_Assignment = booking.Booking_Assignment
						.Where(x => x.BookingID == delete_.BookingID).ToList();
					if (booking_Assignment.Any())
					{
						foreach (var itemm in booking_Assignment)
						{
							itemm.DeleteStatus = 0;
						}
					}

					//Commit transaction nếu thành công
					await transaction.CommitAsync();
					await _context.SaveChangesAsync();

					returnData.ResponseCode = 1;
					returnData.ResposeMessage = $"Xóa thành công BookingID: {delete_.BookingID}!";
					return returnData;
				}
				else
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"BookingID: {delete_.BookingID} không tồn tại!";
					return returnData;
				}
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				returnData.ResponseCode = -99;
				returnData.ResposeMessage = ex.Message;
				return returnData;
			}
		}

		public async Task<ResponseBookingData> GetList_SearchBooking(GetList_SearchBooking getList_)
		{
			var responseData = new ResponseBookingData();
			var listData = new List<ResponseBooking>();
			try
			{
				if (getList_.BookingID != null)
				{
					if (getList_.BookingID <= 0 || await GetBooKingByBookingID(getList_.BookingID) == null)
					{
						responseData.ResponseCode = -1;
						responseData.ResposeMessage = $"BookingID: {getList_.BookingID} không hợp lệ || không tồn tại!";
						return responseData;
					}
				}
				if (getList_.UserName != null)
				{
					if (!Validation.CheckString(getList_.UserName) || !Validation.CheckXSSInput(getList_.UserName))
					{
						responseData.ResponseCode = -1;
						responseData.ResposeMessage = "UserName không hợp lệ || UserName chứa kí tự không hợp lệ";
						return responseData;
					}
				}
				if (getList_.StartDate != null && getList_.EndDate != null)
				{
					if (getList_.StartDate > getList_.EndDate)
					{
						responseData.ResponseCode = -1;
						responseData.ResposeMessage = "StartDate không thể lớn hơn EndDate";
						return responseData;
					}
				}
				var parameters = new DynamicParameters();
				parameters.Add("@BookingID", getList_.BookingID ?? null);
				parameters.Add("@UserName", getList_.UserName ?? null);
				parameters.Add("@StartDate", getList_.StartDate ?? null);
				parameters.Add("@EndDate", getList_.EndDate ?? null);
				var result = await DbConnection.QueryAsync<ResponseBooking>("GetList_SearchBooking", parameters);
				if(result != null && result.Any())
				{
					responseData.ResponseCode = 1;
					responseData.ResposeMessage = "Lấy thành công danh sách!";
					responseData.Data = result.ToList();
					return responseData;
				}
				else
				{
					responseData.ResponseCode = 0;
					responseData.ResposeMessage = "Danh sách rỗng";
					return responseData;
				}
			}
			catch (Exception ex)
			{
				responseData.ResponseCode = -99;
				responseData.ResposeMessage = ex.Message;
				return responseData;
			}
		}

		public async Task<ResponseBooking_AssignmentData> GetList_SearchBooking_Assignment(GetList_SearchBooking_Assignment getList_)
		{
			var returnData = new ResponseBooking_AssignmentData();
			var listData = new List<ResponseBooking_Assignment>();
			try
			{
				if (getList_.BookingID != null) 
				{
					if(getList_.BookingID <= 0 || await GetBooKingByBookingID(getList_.BookingID) == null)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = $"BookingID: {getList_.BookingID} không hợp lệ || không tồn tại!";
						return returnData;
					}
				}
				if (getList_.AssignmentID != null)
				{
					if (getList_.AssignmentID <= 0 || await GetBooking_AssignmentByID(getList_.AssignmentID) == null)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = $"AssignmentID: {getList_.AssignmentID} không hợp lệ || không tồn tại!";
						return returnData;
					}
				} 
				if (getList_.ClinicID != null)
				{
					if(getList_.ClinicID <= 0 || await _clinicRepository.GetClinicByID(getList_.ClinicID) == null)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = $"ClinicID: {getList_.ClinicID} không hợp lệ || không tồn tại!";
						return returnData;
					}
				}
				if (getList_.UserName != null)
				{
					if (!Validation.CheckString(getList_.UserName) || !Validation.CheckXSSInput(getList_.UserName))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu UserName không hợp lệ hoặc chứa kí tự không hợp lệ!";
						return returnData;
					}
				}
				if (getList_.ServiceName != null)
				{
					if (!Validation.CheckString(getList_.ServiceName) || !Validation.CheckXSSInput(getList_.ServiceName))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu ServiceName không hợp lệ hoặc chứa kí tự không hợp lệ!";
						return returnData;
					}
					if (await GetServicessByServicessName(getList_.ServiceName) == null)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu ServiceName tồn tại!";
						return returnData;
					}
				}
				var parameters = new DynamicParameters();
				parameters.Add("@AssignmentID", getList_.AssignmentID ?? null);
				parameters.Add("@BookingID", getList_.BookingID ?? null);
				parameters.Add("@ClinicID", getList_.ClinicID ?? null);
				parameters.Add("@UserName", getList_.UserName ?? null);
				parameters.Add("@ServiceName", getList_.ServiceName ?? null);
				parameters.Add("@AssignedDate", getList_.AssignedDate ?? null);
				var result = await DbConnection.QueryAsync<ResponseBooking_Assignment>("GetList_SearchBooking_Assignment", parameters);
				if (result != null && result.Any())
				{
					returnData.ResponseCode = 1;
					returnData.ResposeMessage = "Lấy thành công danh sách!";
					returnData.Data = result.ToList();
					return returnData;
				}
				else
				{
					returnData.ResponseCode = 0;
					returnData.ResposeMessage = "Danh sách rỗng";
					return returnData;
				}
			}
			catch (Exception ex) 
			{
				returnData.ResponseCode = -99;
				returnData.ResposeMessage = ex.Message;
				return returnData;
			}
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

			//4. Qua 19h tối thì reset _currentNumberOrder về 1
			if (timeVietNam.Hour > 19)
			{
				_currentNumberOrder = 1;
			}

			// 4. Nếu tồn tại booking trong ngày thì tăng NumberOrder
			if (latestBooking != null)
			{
				_currentNumberOrder = (latestBooking.NumberOrder ?? 0) + 1;
			}

			// 5. Kiểm tra xem số thứ tự có vượt quá 100 không
			if (_currentNumberOrder > 100)
			{
				return (null, $"Ngày: {assignedDate.ToString("dd/MM/yyyy")} đã đủ lượt Booking. Vui lòng chọn ngày khác!");
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

		public async Task<Booking_Assignment> GetBooking_AssignmentByID(int? AssignmentID)
		{
			return await _context.Booking_Assignment.Where(s => s.DeleteStatus == 1 
				&& s.AssignmentID == AssignmentID).FirstOrDefaultAsync();
		}

		public async Task<Servicess> GetServicessByServicessName(string? servicessName)
		{
			return await _context.Servicess.Where(s => s.ServiceName == servicessName
				&& s.DeleteStatus == 1).FirstOrDefaultAsync();
		}
	}
}
