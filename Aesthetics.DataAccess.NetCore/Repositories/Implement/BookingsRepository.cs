using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DataAccess.NetCore.DBContext;
using Aesthetics.DataAccess.NetCore.Repositories.Interface;
using Aesthetics.DataAccess.NetCore.Repositories.Interfaces;
using Aesthetics.DTO.NetCore.DataObject.LogginModel;
using Aesthetics.DTO.NetCore.DataObject.Model;
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
		private IUserRepository _userRepository;
		public BookingsRepository(DB_Context context, IServiceProvider serviceProvider,
			IConfiguration configuration, IServicessRepository servicessRepository,
			IClinicRepository clinicRepository, IUserRepository userRepository) : base(serviceProvider)
		{
			_context = context;
			_configuration = configuration;
			_servicessRepository = servicessRepository;
			_clinicRepository = clinicRepository;
			_userRepository = userRepository;
		}

		public async Task<ResponseBooking_Ser_Ass> Insert_Booking(BookingRequest insert_)
		{
			var returnData = new ResponseBooking_Ser_Ass();
			// Danh sách lưu log tất cả các Booking_Assignment & Booking_Servicess được tạo
			var bookingAss_List = new List<Booking_AssignmentLoggin>();
			var bookingSer_List = new List<Booking_ServicessLoggin>();
			var booking_Loggins = new List<Booking_Loggin>();
			var invalidServiceIDs = new List<int>();

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
				if (insert_.UserID <= 0 || await _userRepository.GetUserByUserID(insert_.UserID) == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "UserID không hợp lệ || không tồn tại!";
					return returnData;
				}
				// Kiểm tra ngày đặt lịch không được nhỏ hơn ngày hiện tại
				if (insert_.ScheduledDate.Date < DateTime.Today)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu ScheduledDate không hợp lệ!";
					return returnData;
				}

				//1.Lấy thời gian hiện tại
				var bookingCreation = DateTime.Now;

				var newBooking = new Booking()
				{
					UserID = insert_.UserID,
					BookingCreation = bookingCreation,
					DeleteStatus = 1
				};
				await _context.Booking.AddAsync(newBooking);
				await _context.SaveChangesAsync();
				booking_Loggins.Add(new Booking_Loggin
				{
					BookingID = newBooking.BookingID,
					UserID = newBooking.UserID,
					BookingCreation = newBooking.BookingCreation,
					DeleteStatus = newBooking.DeleteStatus
				});

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
						invalidServiceIDs.Add(servicessID);
						continue;
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
							returnData.ResposeMessage = mesage + $"{servicess.ServiceID}, " + "Vui lòng chọn ngày khác";
							return returnData;
						}

						numberOrder = generatedNumberOrder ?? 0;
						numberOrderMap[servicess.ProductsOfServicesID] = numberOrder;
					}

					//Thêm vào bookingServices
					var newBooking_Servicess = new BookingServicess
					{
						BookingID = newBooking.BookingID,
						ServiceID = servicess.ServiceID,
						ProductsOfServicesID = servicess.ProductsOfServicesID,
						AssignedDate = insert_.ScheduledDate
					};
					await _context.Booking_Servicess.AddAsync(newBooking_Servicess);
					await _context.SaveChangesAsync();

					//Lưu log
					bookingSer_List.Add(new Booking_ServicessLoggin
					{
						BookingServiceID = newBooking_Servicess.BookingServiceID,
						BookingID = newBooking_Servicess.BookingID,
						ServiceID = newBooking_Servicess.ServiceID,
						ProductsOfServicesID = newBooking_Servicess.ProductsOfServicesID,
						AssignedDate = newBooking_Servicess.AssignedDate
					});

					var userName = await GetUserNameByUserID(insert_.UserID);

					//Thêm vào booking Assignment
					var newBooking_Assignment = new BookingAssignment
					{
						BookingID = newBooking.BookingID,
						ClinicID = await GetClinicIDByProductsOfServicesID(servicess.ProductsOfServicesID),
						ProductsOfServicesID = servicess.ProductsOfServicesID,
						UserName = userName,
						ServiceName = servicess.ServiceName,
						NumberOrder = numberOrder,
						AssignedDate = insert_.ScheduledDate,
						Status = 0,
						DeleteStatus = 1
					};
					await _context.Booking_Assignment.AddAsync(newBooking_Assignment);
					await _context.SaveChangesAsync();

					//Lưu log
					bookingAss_List.Add(new Booking_AssignmentLoggin
					{
						AssignmentID = newBooking_Assignment.AssignmentID,
						BookingID = newBooking_Assignment.BookingID,
						ClinicID = newBooking_Assignment.ClinicID,
						ProductsOfServicesID = newBooking_Assignment.ProductsOfServicesID,
						UserName = newBooking_Assignment.UserName,
						ServiceName = newBooking_Assignment.ServiceName,
						NumberOrder = newBooking_Assignment.NumberOrder,
						AssignedDate = newBooking_Assignment.AssignedDate,
						Status = newBooking_Assignment.Status,
						DeleteStatus = newBooking_Assignment.DeleteStatus
					});
				}
				await _context.SaveChangesAsync();
				if (invalidServiceIDs.Any())
				{
					returnData.ResponseCode = 1;
					returnData.ResposeMessage = $"Insert Booking thành công, Ngoại trừ các ServicessID: {string.Join(", ", invalidServiceIDs)} không hợp lệ!";
				}
				else
				{
					returnData.ResponseCode = 1;
					returnData.ResposeMessage = $"Insert Booking thành công!";
				}
				returnData.booking_Loggins = booking_Loggins;
				returnData.Booking_AssData = bookingAss_List;
				returnData.Booking_SerData = bookingSer_List;

				//Commit transaction nếu thành công
				await transaction.CommitAsync();
				return returnData;
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				throw new Exception($"Error Insert_Booking Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}

		public async Task<ResponseBooking_Ser_Ass> Delete_Booking(Delete_Booking delete_)
		{
			var returnData = new ResponseBooking_Ser_Ass();
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

				// Danh sách lưu tất cả các Booking_Assignment & Booking_Servicess được tạo
				var bookingAss_List = new List<Booking_AssignmentLoggin>();
				var bookingSer_List = new List<Booking_ServicessLoggin>();

				if (booking != null)
				{
					booking.DeleteStatus = 0;
					await _context.SaveChangesAsync();

					//Xóa những bản ghi ở Booking_Servicesses có liên quan đến BookingID
					var booking_Servicess = booking.Booking_Servicesses
						.Where(s => s.BookingID == booking.BookingID && s.AssignedDate > DateTime.Today).ToList();

					//Nếu booking_Servicess tồn tại 
					if (booking_Servicess.Any())
					{
						foreach (var item in booking_Servicess)
						{
							_context.Booking_Servicess.Remove(item);
							bookingSer_List.Add(new Booking_ServicessLoggin
							{
								BookingServiceID = item.BookingServiceID,
								BookingID = item.BookingID,
								ServiceID = item.ServiceID,
								ProductsOfServicesID = item.ProductsOfServicesID,
							});
						}
					}

					//Xóa những bản ghi ở Booking_Assignment có liên quan đến delete_.BookingID
					var booking_Assignment = booking.Booking_Assignment
						.Where(x => x.BookingID == delete_.BookingID && x.AssignedDate > DateTime.Today).ToList();
					if (booking_Assignment.Any())
					{
						foreach (var itemm in booking_Assignment)
						{
							itemm.DeleteStatus = 0;
							bookingAss_List.Add(new Booking_AssignmentLoggin
							{
								AssignmentID = itemm.AssignmentID,
								BookingID = itemm.BookingID,
								ClinicID = itemm.ClinicID,
								ProductsOfServicesID = itemm.ProductsOfServicesID,
								UserName = itemm.UserName,
								ServiceName = itemm.ServiceName,
								NumberOrder = itemm.NumberOrder,
								AssignedDate = itemm.AssignedDate,
								Status = itemm.Status,
								DeleteStatus = itemm.DeleteStatus
							});
						}
					}

					//Commit transaction nếu thành công
					await transaction.CommitAsync();
					await _context.SaveChangesAsync();

					returnData.ResponseCode = 1;
					returnData.ResposeMessage = $"Xóa thành công BookingID: {delete_.BookingID}!";
					returnData.Booking_AssData = bookingAss_List;
					returnData.Booking_SerData = bookingSer_List;
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
				throw new Exception($"Error Delete_Booking Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
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
				if (getList_.UserID != null)
				{
					if (getList_.UserID <= 0 || await _userRepository.GetUserByUserID(getList_.UserID) == null)
					{
						responseData.ResponseCode = -1;
						responseData.ResposeMessage = "UserID không hợp lệ || không tồn tại!";
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
				parameters.Add("@UserID", getList_.UserID ?? null);
				parameters.Add("@StartDate", getList_.StartDate ?? null);
				parameters.Add("@EndDate", getList_.EndDate ?? null);
				var result = await DbConnection.QueryAsync<ResponseBooking>("GetList_SearchBooking", parameters);
				if (result != null && result.Any())
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
				throw new Exception($"Error GetList_SearchBooking Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
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
					if (getList_.BookingID <= 0 || await GetBooKingByBookingID(getList_.BookingID) == null)
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
					if (getList_.ClinicID <= 0 || await _clinicRepository.GetClinicByID(getList_.ClinicID) == null)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = $"ClinicID: {getList_.ClinicID} không hợp lệ || không tồn tại!";
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
				throw new Exception($"Error GetList_SearchBooking Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}

		public async Task<ResponseBooking_ServicesData> GetList_SearchBooking_Services(GetList_SearchBooking_Services getList_)
		{
			var returnData = new ResponseBooking_ServicesData();
			var listData = new List<ResponseBooking_Services>();
			try
			{
				if (getList_.BookingID <= 0 || await GetBooKingByBookingID(getList_.BookingID) == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "BookingID không hợp lệ || không tồn tại!";
					return returnData;
				}
				var parameters = new DynamicParameters();
				parameters.Add("@BookingID", getList_.BookingID);
				var result = await DbConnection.QueryAsync<ResponseBooking_Services>("GetList_SearchBooking_Services", parameters);
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
				throw new Exception($"Error GetList_SearchBooking_Assignment Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}

		public async Task<ResponseBooking_Ser_Ass> Insert_BookingSer_Assi(Insert_Booking_Services insert_)
		{
			var returnData = new ResponseBooking_Ser_Ass();
			var bookingAss_List = new List<Booking_AssignmentLoggin>();
			var bookingSer_List = new List<Booking_ServicessLoggin>();
			using var transaction = await _context.Database.BeginTransactionAsync();

			try
			{
				// Kiểm tra BookingID hợp lệ
				if (insert_.BookingID <= 0 || await GetBooKingByBookingID(insert_.BookingID) == null)
					return new ResponseBooking_Ser_Ass { ResponseCode = -1, ResposeMessage = "BookingID không hợp lệ || không tồn tại!" };

				// Kiểm tra ServiceID hợp lệ
				if (insert_.ServiceID <= 0 || await GetServicessByServicessID(insert_.ServiceID) == null)
					return new ResponseBooking_Ser_Ass { ResponseCode = -1, ResposeMessage = "ServiceID không hợp lệ || không tồn tại!" };

				// Lấy ProductOfServices
				int? productOfServices = await GetProductOfServicesIDByServisesID(insert_.ServiceID);
				if (productOfServices == null || productOfServices == 0)
					return new ResponseBooking_Ser_Ass { ResponseCode = -1, ResposeMessage = "productOfServices không hợp lệ || không tồn tại!" };

				// Lấy AssignedDate và UserName
				var bookingInfo = await _context.Booking_Assignment
					.Where(s => s.BookingID == insert_.BookingID && s.DeleteStatus == 1)
					.Select(s => new { s.AssignedDate, s.UserName })
					.FirstOrDefaultAsync();

				if (bookingInfo == null)
					return new ResponseBooking_Ser_Ass { ResponseCode = -1, ResposeMessage = "Không tìm thấy thông tin booking assignment!" };

				// Tạo BookingServicess
				var newBookingServices = new BookingServicess
				{
					BookingID = insert_.BookingID,
					ServiceID = insert_.ServiceID,
					ProductsOfServicesID = productOfServices,
					AssignedDate = bookingInfo.AssignedDate
				};

				_context.Booking_Servicess.Add(newBookingServices);
				await _context.SaveChangesAsync();

				bookingSer_List.Add(new Booking_ServicessLoggin
				{
					BookingServiceID = newBookingServices.BookingServiceID,
					BookingID = newBookingServices.BookingID,
					ServiceID = newBookingServices.ServiceID,
					ProductsOfServicesID = newBookingServices.ProductsOfServicesID,
					AssignedDate = newBookingServices.AssignedDate
				});

				// Lấy ClinicID, ServicesName
				var clinicID = await GetClinicIDByProductsOfServicesID(productOfServices);
				var servicesName = await GetServicesNamebyServicesID(insert_.ServiceID);

				// Lấy NumberOrder từ DB, nếu không có thì tạo mới
				var numberOrder = await _context.Booking_Assignment
					.Where(s => s.BookingID == insert_.BookingID && s.DeleteStatus == 1 && s.ProductsOfServicesID == productOfServices)
					.Select(s => s.NumberOrder)
					.FirstOrDefaultAsync();

				if (numberOrder == null)
				{
					var result = await GenerateNumberOrder(bookingInfo.AssignedDate, productOfServices);
					if (result.Message != null)
						return new ResponseBooking_Ser_Ass { ResponseCode = -1, ResposeMessage = result.Message };

					numberOrder = result.NumberOrder;
				}

				// Tạo BookingAssignment
				var newBookingAssi = new BookingAssignment
				{
					BookingID = insert_.BookingID,
					ClinicID = clinicID,
					ProductsOfServicesID = productOfServices,
					UserName = bookingInfo.UserName,
					ServiceName = servicesName,
					NumberOrder = numberOrder,
					AssignedDate = bookingInfo.AssignedDate,
					Status = 0,
					DeleteStatus = 1
				};

				_context.Booking_Assignment.Add(newBookingAssi);
				await _context.SaveChangesAsync();

				bookingAss_List.Add(new Booking_AssignmentLoggin
				{
					AssignmentID = newBookingAssi.AssignmentID,
					BookingID = insert_.BookingID,
					ClinicID = newBookingAssi.ClinicID,
					ProductsOfServicesID = newBookingAssi.ProductsOfServicesID,
					UserName = newBookingAssi.UserName,
					ServiceName = newBookingAssi.ServiceName,
					NumberOrder = newBookingAssi.NumberOrder,
					AssignedDate = newBookingAssi.AssignedDate,
					Status = newBookingAssi.Status,
					DeleteStatus = newBookingAssi.DeleteStatus
				});

				// Commit Transaction
				await transaction.CommitAsync();

				return new ResponseBooking_Ser_Ass
				{
					ResponseCode = 1,
					ResposeMessage = "Insert BookingService thành công!",
					Booking_AssData = bookingAss_List,
					Booking_SerData = bookingSer_List
				};
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				throw new Exception($"Error Insert_BookingSer_Assi: {ex.Message}", ex);
			}
		}

		public async Task<ResponseBooking_Ser_Ass> Delete_BookingSer_Assi(Delete_Booking_Services delete_)
		{
			var returnData = new ResponseBooking_Ser_Ass();
			var bookingAss_List = new List<Booking_AssignmentLoggin>();
			var bookingSer_List = new List<Booking_ServicessLoggin>();
			using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				var booking_Ass_Serv = await _context.Booking_Servicess
					.Where(s => s.BookingServiceID == delete_.BookingServiceID)
					.FirstOrDefaultAsync();
				if (booking_Ass_Serv != null)
				{
					//Xóa booking services
					_context.Booking_Servicess.Remove(booking_Ass_Serv);
					bookingSer_List.Add(new Booking_ServicessLoggin
					{
						BookingServiceID = booking_Ass_Serv.BookingServiceID,
						BookingID = booking_Ass_Serv.BookingID,
						ServiceID = booking_Ass_Serv.ServiceID,
						ProductsOfServicesID = booking_Ass_Serv.ProductsOfServicesID,
						AssignedDate = booking_Ass_Serv.AssignedDate,
					});

					//Xóa booking assignment
					var bookingID = booking_Ass_Serv.BookingID;
					var servicesID = booking_Ass_Serv.ServiceID;
					var servicesName = await GetServicesNamebyServicesID(servicesID);
					var bookingAssignment = _context.Booking_Assignment
						.Where(s => s.BookingID == bookingID
								&& s.AssignedDate > DateTime.Now
								&& s.ServiceName == servicesName
								&& s.DeleteStatus == 1).ToList();
					if (bookingAssignment.Any() && bookingAssignment != null)
					{
						foreach (var tmp in bookingAssignment)
						{
							if (tmp.Status != 0)
							{
								tmp.DeleteStatus = 0;
								bookingAss_List.Add(new Booking_AssignmentLoggin
								{
									AssignmentID = tmp.AssignmentID,
									BookingID = tmp.BookingID,
									ClinicID = tmp.ClinicID,
									ProductsOfServicesID = tmp.ProductsOfServicesID,
									UserName = tmp.UserName,
									ServiceName = tmp.ServiceName,
									NumberOrder = tmp.NumberOrder,
									AssignedDate = tmp.AssignedDate,
									Status = tmp.Status,
									DeleteStatus = tmp.DeleteStatus,
								});
							}
							returnData.ResponseCode = 2;
							returnData.ResposeMessage = "Dịch vụ đã thực hiện. Xóa không thành công!";
							return returnData;
						}
					}
					await _context.SaveChangesAsync();
					await transaction.CommitAsync();

					returnData.ResponseCode = 1;
					returnData.ResposeMessage = "Delele Booking Services thành công!";
					returnData.Booking_SerData = bookingSer_List;
					returnData.Booking_AssData = bookingAss_List;
					return returnData;
				}
				returnData.ResponseCode = -1;
				returnData.ResposeMessage = "BookingServiceID Không tồn tại!";
				return returnData;
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				throw new Exception($"Error Delete_BookingSer_Assi: {ex.Message}", ex);
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
				return (null, $"Ngày: {assignedDate.ToString("dd/MM/yyyy")} lịch ServicesID: ");
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

		public async Task<BookingAssignment> GetBooking_AssignmentByID(int? AssignmentID)
		{
			return await _context.Booking_Assignment.Where(s => s.DeleteStatus == 1
				&& s.AssignmentID == AssignmentID).FirstOrDefaultAsync();
		}

		public async Task<Servicess> GetServicessByServicessName(string? servicessName)
		{
			return await _context.Servicess.Where(s => s.ServiceName == servicessName
				&& s.DeleteStatus == 1).FirstOrDefaultAsync();
		}

		public async Task<int> GetProductOfServicesIDByServisesID(int servicesID)
		{
			return await _context.Servicess
				.Where(s => s.ServiceID == servicesID)
				.Select(s => s.ProductsOfServicesID).FirstOrDefaultAsync();
		}

		public async Task<string> GetUserNameByUserID(int userId)
		{
			return await _context.Users
				.Where(s => s.UserID == userId)
				.Select(s => s.UserName).FirstOrDefaultAsync();
		}

		public async Task<string> GetServicesNamebyServicesID(int servicesID)
		{
			return await _context.Servicess
				.Where(s => s.ServiceID == servicesID && s.DeleteStatus == 1)
				.Select(s => s.ServiceName)
				.FirstOrDefaultAsync();
		}
	}
}
