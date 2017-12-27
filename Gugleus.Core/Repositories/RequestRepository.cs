using Dapper;
using Gugleus.Core.Domain.Requests;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.Threading.Tasks;
using System.Linq;

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

        public async Task<RequestQueue> GetRequestQueue(long id)
        {
            RequestQueue requestQueue = null;

            string query = @"SELECT id, r.id_ws_client, r.id_request_type,
                                r.request_input, r.request_output, r.add_date, r.output_date,
                                rq.id_status, rq.add_date, rq.process_start_date, rq.process_end_date, rq.error_msg
                            FROM he.requests r
                            JOIN he.requests_queue rq USING (id)
                            WHERE id = @id";

            using (NpgsqlConnection conn = new NpgsqlConnection(_connStr))
            {
                requestQueue = await conn.QueryFirstOrDefaultAsync<RequestQueue>(query, param: new { id });
            }

            return requestQueue;
        }
    }
}
