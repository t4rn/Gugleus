using Gugleus.Core.Domain;
using Gugleus.Core.Domain.Requests;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Gugleus.Core.Repositories
{
    public class RequestEfRepository : IRequestRepository
    {
        private readonly AppDbContext _appDbContext;

        public RequestEfRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public RequestEfRepository(AppDbContext appDbContext, string connectionString) : this(appDbContext)
        {
            _appDbContext.SetConnectionString(connectionString);
        }

        public async Task<long> AddRequestAsync(Request request)
        {
            long id = -1;
            // For MS SQL:
            //id = await _appDbContext.Database.ExecuteSqlCommandAsync(
            //    "EXEC he.add_request @p_id_request_type @p_input @p_id_ws_client", new NpgsqlParameter[]
            //    {
            //        new NpgsqlParameter("p_id_request_type", request.Type.Code),
            //        new NpgsqlParameter("p_input", NpgsqlDbType.Json) { Value = request.Input },
            //        new NpgsqlParameter("p_id_ws_client", request.WsClient?.Id)
            //    });

            using (DbConnection conn = _appDbContext.Database.GetDbConnection())
            {
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "he.add_request";
                    cmd.Parameters.Add(new NpgsqlParameter("p_id_request_type", request.Type.Code));
                    cmd.Parameters.Add(new NpgsqlParameter("p_input", NpgsqlDbType.Json) { Value = request.Input });
                    cmd.Parameters.Add(new NpgsqlParameter("p_id_ws_client", request.WsClient?.Id));

                    cmd.Connection.Open();
                    id = Convert.ToInt64(await cmd.ExecuteScalarAsync());
                }
            }

            return id;

        }

        public async Task<List<Request>> GetAllWithPaginationAsync(int page = 0, int pageSize = 0)
        {
            IQueryable<Request> linqQuery = GetAllLinqQuery();

            if (page > 0 && pageSize > 0)
            {
                linqQuery = linqQuery.OrderBy(r => r.Id)
                    .Skip((page - 1) * pageSize).Take(pageSize);
            }

            return await linqQuery.ToListAsync();
        }

        public async Task<List<Request>> GetAllAsync()
        {
            return await GetAllLinqQuery().ToListAsync();
        }

        public async Task<Request> GetRequestByIdAsync(long requestId)
        {
            return await _appDbContext.Requests.AsNoTracking()
                .Include(x => x.WsClient)
                .Include(x => x.Type)
                .Include(x => x.Queue)
                .Include(x => x.Queue.Status)
                .FirstOrDefaultAsync(x => x.Id == requestId);
        }

        public async Task<Request> GetRequestWithQueueAsync(long id, string requestType)
        {
            Request requestWithQueue = await GetAllLinqQuery()
                .FirstOrDefaultAsync(x => x.Id == id && x.TypeCode == requestType);

            return requestWithQueue;
        }

        public async Task<List<RequestStat>> GetStatsByDate(DateTime from, DateTime to)
        {
            List<RequestStat> stats = new List<RequestStat>();

            string query = @"SELECT COALESCE(q.id_request_type, 'ADDPOST') as Type, dic.code as Status, COALESCE(q.count, 0) as Count, q.avg as Avg 
                        FROM he.dic_request_status dic
                        LEFT JOIN (
                        SELECT  rq.id_status, rq.id_request_type, count(rq.id_status), avg(process_end_date - process_start_date) 
                        FROM he.requests_queue rq 
                        WHERE rq.id_request_type = 'ADDPOST' AND rq.add_date >= @from AND rq.add_date <= @to
                        GROUP BY rq.id_status, rq.id_request_type) q ON q.id_status = dic.code
                    UNION
                        SELECT COALESCE(q.id_request_type, 'GETINFO'), dic.code, COALESCE(q.count, 0), q.avg 
                        FROM he.dic_request_status dic
                        LEFT JOIN (
                        SELECT  rq.id_status, rq.id_request_type, count(rq.id_status), avg(process_end_date - process_start_date) 
                        FROM he.requests_queue rq 
                        WHERE rq.id_request_type = 'GETINFO' AND rq.add_date >= @from AND rq.add_date <= @to
                        GROUP BY rq.id_status, rq.id_request_type) q ON q.id_status = dic.code
                        ORDER BY 2, 1";

            using (DbConnection conn = _appDbContext.Database.GetDbConnection())
            {
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = query;
                    cmd.Parameters.Add(new NpgsqlParameter("from", from));
                    cmd.Parameters.Add(new NpgsqlParameter("to", to));

                    conn.Open();
                    DbDataReader dr = await cmd.ExecuteReaderAsync();
                    while (await dr.ReadAsync())
                    {
                        RequestStat rs = new RequestStat();
                        rs.Type = dr.GetString(0);
                        rs.Status = dr.GetString(1);
                        rs.Count = dr.GetInt64(2);
                        rs.Avg = dr[3] != DBNull.Value ? (TimeSpan?)dr[3] : null;
                        stats.Add(rs);
                    }
                }
            }

            return stats;
        }

        public async Task<List<WsClient>> GetWsClientsAsync()
        {
            return await _appDbContext.WsClients.ToListAsync();
        }

        public IQueryable<Request> GetAllQueryable()
        {
            return GetAllLinqQuery();
        }

        private IQueryable<Request> GetAllLinqQuery()
        {
            return _appDbContext.Requests.AsNoTracking()
                .Include(x => x.WsClient)
                .Include(x => x.Type)
                .Include(x => x.Queue)
                .Include(x => x.Queue.Status);
        }

        public string GetConnectionString()
        {
            return _appDbContext.Database?.GetDbConnection()?.ConnectionString;
        }
    }
}
