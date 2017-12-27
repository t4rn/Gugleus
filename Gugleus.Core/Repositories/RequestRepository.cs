﻿using Dapper;
using Gugleus.Core.Domain.Requests;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.Threading.Tasks;
using System.Linq;
using Gugleus.Core.Domain;

namespace Gugleus.Core.Repositories
{
    public class RequestRepository : AbstractRepository, IRequestRepository
    {
        public RequestRepository(string connectionString) : base(connectionString)
        { }

        public async Task<long> AddRequest(Request request)
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
                        p_input = new CustomParameter(request.Input, NpgsqlDbType.Json)
                    },
                    commandType: CommandType.StoredProcedure);

                //NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                //cmd.CommandType = System.Data.CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@input", NpgsqlDbType.Json, request.Input);
                //await cmd.Connection.OpenAsync();
                //object o = cmd.ExecuteScalar();
                //if (o != null && o != DBNull.Value)
                //{
                //    id = Convert.ToInt64(o);
                //}
            }

            return id;
        }

        public async Task<Request> GetRequestDetails(long id)
        {
            Request requestWithQueue = null;

            string query = @"SELECT id as Id, r.id_ws_client as WsClient,
                                r.request_input as Input, r.request_output as Output,
                                r.add_date as AddDate, r.output_date as OutputDate,

                                r.id_request_type as Code,

                                rq.add_date as AddDate,
                                rq.process_start_date as ProcessStartDate, rq.process_end_date as ProcessEndDate,
                                rq.error_msg as ErrorMsg,

                                rq.id_status as Code

                            FROM he.requests r
                            JOIN he.requests_queue rq USING (id)
                            WHERE id = @id";

            using (NpgsqlConnection conn = new NpgsqlConnection(_connStr))
            {
                var cos = await conn.QueryAsync<Request, DictionaryItem, RequestQueue, DictionaryItem, Request>(
                    query,
                    (request, type, queue, status) =>
                    {
                        request.Queue = queue;
                        request.Type = type;
                        request.Queue.Status = status;
                        return request;
                    },
                    param: new { id },
                    splitOn: "Code, AddDate, Code");

                requestWithQueue = cos.FirstOrDefault();
            }

            return requestWithQueue;
        }
    }
}
