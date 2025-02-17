using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DataAccess.NetCore.DBContext;
using Aesthetics.DataAccess.NetCore.Repositories.Interface;
using Aesthetics.DataAccess.NetCore.Repositories.Interfaces;
using Aesthetics.DTO.NetCore.RequestData;
using BE_102024.DataAces.NetCore.CheckConditions;
using BE_102024.DataAces.NetCore.Dapper;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DataAccess.NetCore.Repositories.Implement
{
	public class ClinicRepository : BaseApplicationService, IClinicRepository
	{
		private DB_Context _context;
		private IConfiguration _configuration;
		private ITypeProductsOfServicesRepository _productsOfServicesRepository;
		public ClinicRepository(DB_Context context, IConfiguration configuration,
			IServiceProvider serviceProvider,
			ITypeProductsOfServicesRepository productsOfServicesRepository) : base(serviceProvider) 
		{
			_context = context;
			_configuration = configuration;
			_productsOfServicesRepository = productsOfServicesRepository;
		}

		public async Task<ResponseData> Insert_Clinic(ClinicRequest insert_)
		{
			var returnData = new ResponseData();
			try
			{
				if (!Validation.CheckString(insert_.ClinicName))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"Dữ liệu đầu vào {insert_.ClinicName} không hợp lệ!";
					return returnData;
				}
				if (!Validation.CheckXSSInput(insert_.ClinicName))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"Dữ liệu {insert_.ClinicName} chứa kí tự không hợp lệ!";
					return returnData;
				}
				if (insert_.ProductsOfServicesID <= 0)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"Dữ liệu đầu vào {insert_.ProductsOfServicesID} không hợp lệ!";
					return returnData;
				}
				if (await _productsOfServicesRepository
					.GetTypeProductsOfServicesIDByID(insert_.ProductsOfServicesID) == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"Không tồn tại ProductsOfServicesID: {insert_.ProductsOfServicesID}";
					return returnData;
				}
				if (!Validation.CheckString(insert_.ProductsOfServicesName))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"Dữ liệu đầu vào {insert_.ProductsOfServicesName} không hợp lệ!";
					return returnData;
				}
				if (!Validation.CheckXSSInput(insert_.ProductsOfServicesName))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"Dữ liệu {insert_.ProductsOfServicesName} chứa kí tự không hợp lệ!";
					return returnData;
				}
				if(await _productsOfServicesRepository
					.GetTypeProductsOfServicesByName(insert_.ProductsOfServicesName) == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"Không tồn tại ProductsOfServices có Name: {insert_.ProductsOfServicesName}";
					return returnData;
				}

				var parameters = new DynamicParameters();
				parameters.Add("@ClinicName",insert_.ClinicName);
				parameters.Add("@ProductsOfServicesID", insert_.ProductsOfServicesID);
				parameters.Add("@ProductsOfServicesName", insert_.ProductsOfServicesName);
				await DbConnection.ExecuteAsync("Insert_Clinic", parameters);
				returnData.ResponseCode = 11;
				returnData.ResposeMessage = "Insert thành công Clinic!";
				return returnData;
			}
			catch (Exception ex) 
			{
				returnData.ResponseCode = -99;
				returnData.ResposeMessage = ex.Message;
				return returnData;
			}
		}

		public Task<ResponseData> Update_Clinic(Update_Clinic update_)
		{
			throw new NotImplementedException();
		}

		public Task<ResponseData> Delete_Clinic(Delete_Clinic delete_)
		{
			throw new NotImplementedException();
		}

		public Task<ResponseData> GetList_SearchClinic(GetList_Search getList_)
		{
			throw new NotImplementedException();
		}
	}
}
