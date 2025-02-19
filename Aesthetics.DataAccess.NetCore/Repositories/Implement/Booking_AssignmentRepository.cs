using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DataAccess.NetCore.DBContext;
using Aesthetics.DataAccess.NetCore.Repositories.Interfaces;
using Aesthetics.DTO.NetCore.DataObject;
using Aesthetics.DTO.NetCore.RequestData;
using BE_102024.DataAces.NetCore.Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DataAccess.NetCore.Repositories.Implement
{
	public class Booking_AssignmentRepository : BaseApplicationService, IBooking_AssignmentRepository
	{
		private DB_Context _context;
		private IConfiguration _configuration;
		public Booking_AssignmentRepository(DB_Context context, IConfiguration configuration,
			IServiceProvider serviceProvider) : base(serviceProvider) 
		{
			_context = context;
			_configuration = configuration;
		}

		public Task<ResponseData> Delete_Booking_Assignment()
		{
			throw new NotImplementedException();
		}

		public Task<ResponseData> GetList_SearchBooking_Assignment()
		{
			throw new NotImplementedException();
		}
	}
}
