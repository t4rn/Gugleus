using Dapper;
using Gugleus.Core.Domain;
using Gugleus.Core.Domain.Requests;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Gugleus.Core.Repositories
{
    public class RequestRepository : AbstractRepository, IRequestRepository
    {
        public RequestRepository(string connectionString) : base(connectionString)
        { }

        public async Task<long> AddRequestAsync(Request request)
        {
            long id = -1;
            string query = @"he.add_request";

            using (NpgsqlConnection conn = new NpgsqlConnection(_connStr))
            {
                id = await conn.ExecuteScalarAsync<long>(
                    sql: query,
                    param: new
                    {
                        p_id_request_type = request.Type.Code,
                        p_input = new CustomParameter(request.Input, NpgsqlDbType.Json),
                        p_id_ws_client = request.WsClient?.Id
                    },
                    commandType: CommandType.StoredProcedure);

                //NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                //cmd.CommandType = System.Data.CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@input", NpgsqlDbType.Json, request.Input);
                //await cmd.Connection.OpenAsync();
                //object o = cmd.ExecuteScalar();
                //if (o != null && o != DBNull.Value)
                //    id = Convert.ToInt64(o);
            }

            return id;
        }

        public async Task<List<Request>> GetRequestsAsync()
        {
            List<Request> requests = null;

            string query = @"SELECT id as Id, --r.id_ws_client as WsClient,
                                r.request_input as Input, r.request_output as Output,
                                r.add_date as AddDate, r.output_date as OutputDate,

                                r.id_request_type as Code,

                                rq.add_date as AddDate,
                                rq.process_start_date as ProcessStartDate, rq.process_end_date as ProcessEndDate,
                                rq.error_msg as ErrorMsg,

                                rq.id_status as Code

                            FROM he.requests r
                            JOIN he.requests_queue rq USING (id)";

            using (NpgsqlConnection conn = new NpgsqlConnection(_connStr))
            {
                requests =
                    (
                        await conn.QueryAsync<Request, DictionaryItem, RequestQueue, DictionaryItem, Request>(
                        sql: query,
                        map: (request, type, queue, status) =>
                        {
                            request.Queue = queue;
                            request.Type = type;
                            request.Queue.Status = status;
                            return request;
                        },
                        splitOn: "Code, AddDate, Code")
                    ).ToList();
            }

            return requests;
        }

        public async Task<Request> GetRequestWithQueueAsync(long id, string requestType)
        {
            Request requestWithQueue = null;

            string query = @"SELECT id as Id, --r.id_ws_client as WsClient,
                                r.request_input as Input, r.request_output as Output,
                                r.add_date as AddDate, r.output_date as OutputDate,

                                r.id_request_type as Code,

                                rq.add_date as AddDate,
                                rq.process_start_date as ProcessStartDate, rq.process_end_date as ProcessEndDate,
                                rq.error_msg as ErrorMsg,

                                rq.id_status as Code

                            FROM he.requests r
                            JOIN he.requests_queue rq USING (id)
                            WHERE id = @id AND r.id_request_type = @request_type";

            using (NpgsqlConnection conn = new NpgsqlConnection(_connStr))
            {
                IEnumerable<Request> queryResult =
                    await conn.QueryAsync<Request, DictionaryItem, RequestQueue, DictionaryItem, Request>(
                    sql: query,
                    map: (request, type, queue, status) =>
                    {
                        request.Queue = queue;
                        request.Type = type;
                        request.Queue.Status = status;
                        return request;
                    },
                    param: new { id, request_type = requestType },
                    splitOn: "Code, AddDate, Code");

                requestWithQueue = queryResult.FirstOrDefault();
            }

            return requestWithQueue;
        }

        public async Task<List<RequestStat>> GetStatsByDate(DateTime from, DateTime to)
        {
            List<RequestStat> requestStats = null;

            string query = @"SELECT id_request_type as Type, id_status as Status, count(*) as Amount, avg(process_end_date - process_start_date) as AvgProcessTime
                                FROM he.requests_queue 
                                WHERE add_date >= @from AND add_date <= @to
                                GROUP BY id_request_type, id_status
                                ORDER BY id_request_type";

            query = @"SELECT COALESCE(q.id_request_type, 'ADDPOST') as Type, dic.code as Status, COALESCE(q.count, 0) as Count, q.avg as Avg 
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


            using (NpgsqlConnection conn = new NpgsqlConnection(_connStr))
            {
                requestStats = (await conn.QueryAsync<RequestStat>(query, new { from = from, to = to })).ToList();
            }

            return requestStats;
        }

        public async Task<List<WsClient>> GetWsClientsAsync()
        {
            List<WsClient> wsClients = null;

            string query = @"SELECT id, client_name as Name, hash, ghost, add_date as AddDate
                                FROM he.ws_clients
                                WHERE ghost = false";

            using (NpgsqlConnection conn = new NpgsqlConnection(_connStr))
            {
                wsClients = (await conn.QueryAsync<WsClient>(query)).ToList();
            }

            return wsClients;
        }
    }
}
