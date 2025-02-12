using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DataAccess.NetCore.Dapper
{
	public class ApplicationDbConnection : IApplicationDbConnection
	{
		private readonly IDbConnection connection;
		public IDbConnection GetConnection => this.connection;
		public ApplicationDbConnection(IConfiguration configuration)
		{
			connection = new SqlConnection(configuration.GetConnectionString("Aesthetics_ConnString"));
		}

		public async Task<int> ExecuteAsync(string sql, object param = null, CommandType commandType = CommandType.StoredProcedure, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
		{
			return await connection.ExecuteAsync(sql, param, transaction, commandTimeout: 600, commandType: commandType);
		}

		public async Task<List<T>> QueryAsync<T>(string sql, object param = null, CommandType commandType = CommandType.StoredProcedure, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
		{
			return (await connection.QueryAsync<T>(sql, param, transaction, commandTimeout: 600, commandType: commandType))?.AsList();
		}

		public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, CommandType commandType = CommandType.StoredProcedure, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
		{
			return await connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandType: commandType);
		}

		public async Task<T> QuerySingleAsync<T>(string sql, object param = null, CommandType commandType = CommandType.StoredProcedure, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
		{
			return await connection.QuerySingleAsync<T>(sql, param, transaction, commandType: commandType);
		}
	}
}
