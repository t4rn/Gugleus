using Npgsql;
using NpgsqlTypes;
using System.Data;
using static Dapper.SqlMapper;

namespace Gugleus.Core.Repositories
{
    public class CustomParameter : ICustomQueryParameter
    {
        private readonly string _value;
        private readonly NpgsqlDbType _dbType;

        public CustomParameter(string value, NpgsqlDbType dbType)
        {
            _value = value;
            _dbType = dbType;
        }

        public void AddParameter(IDbCommand command, string name)
        {
            command.Parameters.Add(new NpgsqlParameter
            {
                ParameterName = name,
                NpgsqlDbType = _dbType,
                Value = _value
            });
        }
    }
}
