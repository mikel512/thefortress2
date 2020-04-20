using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DataAccessLibrary.SqlDataAccess;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLibrary.Services
{
    public class DataAccessService : IDataAccessService
    {
        private ApplicationDbContext _configuration;
        private string _connection;

        public DataAccessService(ApplicationDbContext configuration)
        {
            _configuration = configuration;
        }

        public string ConnectionString
        {
            get => Connection();
            set => _connection = value;
        }

        private string Connection()
        {
            return _configuration.Database.GetDbConnection().ConnectionString;
        }

        public List<T> ExecuteProcedure<T>(string procedure)
        {
            var list = new List<T>();
            using (var cnn = new SqlConnection(ConnectionString))
            {
                cnn.Open();
                var command = new SqlCommand(procedure, cnn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                list = cnn.Query<T>(command.CommandText).ToList();
            }

            return list;
        }
        public async Task<IEnumerable<T>> ExecuteProcedureAsync<T>(string procedure)
        {
            using (var cnn = new SqlConnection(ConnectionString))
            {
                cnn.Open();
                var command = new SqlCommand(procedure, cnn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                return await cnn.QueryAsync<T>(command.CommandText);
            }
        }

        public List<T> ExecuteProcedure<T>(string procedure, params KeyValuePair<string, object>[] pairs)
        {
            var list = new List<T>();
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                var command = new SqlCommand(procedure, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                var p = new DynamicParameters(pairs);

                list = conn.Query<T>(procedure, p, commandType: CommandType.StoredProcedure).ToList();
            }
            return list;
        }
        
        public async Task<IEnumerable<T>> ExecuteProcedureAsync<T>(string procedure, params KeyValuePair<string, object>[] pairs)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                var command = new SqlCommand(procedure, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                var p = new DynamicParameters(pairs);

                return await conn.QueryAsync<T>(procedure, p, commandType: CommandType.StoredProcedure);
            }
        }

        public int ExecuteProcedure(string procedure, string outputParam, params KeyValuePair<string, object>[] pairs)
        {
            int id = 1;
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                var command = new SqlCommand(procedure, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                var p = new DynamicParameters(pairs);
                if (!string.IsNullOrEmpty(outputParam))
                {
                    p.Add(outputParam, dbType: DbType.Int32, direction: ParameterDirection.Output);
                }

                conn.Execute(procedure, p, commandType: CommandType.StoredProcedure);
                if (!string.IsNullOrEmpty(outputParam)) id = p.Get<int>(outputParam);
            }

            return id;
        }
        public async Task<int> ExecuteProcedureAsync(string procedure, string outputParam, params KeyValuePair<string, object>[] pairs)
        {
            int id = 1;
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                var command = new SqlCommand(procedure, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                var p = new DynamicParameters(pairs);
                if (!string.IsNullOrEmpty(outputParam))
                {
                    p.Add(outputParam, dbType: DbType.Int32, direction: ParameterDirection.Output);
                }

                await conn.ExecuteAsync(procedure, p, commandType: CommandType.StoredProcedure);
                if (!string.IsNullOrEmpty(outputParam)) id = p.Get<int>(outputParam);
            }

            return id;
        }
        // return dictionary that will serialise to json
        public Dictionary<string,string> ExecuteProcedureJson(string procedure, string[] outputParams, params KeyValuePair<string, object>[] pairs)
        {
            var dict = new Dictionary<string, string>();
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                var command = new SqlCommand(procedure, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                var p = new DynamicParameters(pairs);
                if (outputParams.Length > 0)
                {
                    for (int i = 0; i < outputParams.Length; i++)
                    {
                        p.Add(outputParams[i], dbType: DbType.Int32, direction: ParameterDirection.Output);
                    }
                }

                conn.Execute(procedure, p, commandType: CommandType.StoredProcedure);
                if (outputParams.Length > 0)
                {
                    for (int i = 0; i < outputParams.Length; i++)
                    {
                        dict.Add(i.ToString(), p.Get<int>(outputParams[i]).ToString());
                    }
                }
            }
            return dict;
        }
        public async Task<Dictionary<string,string>> ExecuteProcedureJsonAsync(string procedure, string[] outputParams, params KeyValuePair<string, object>[] pairs)
        {
            var dict = new Dictionary<string, string>();
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                var command = new SqlCommand(procedure, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                var p = new DynamicParameters(pairs);
                if (outputParams.Length > 0)
                {
                    for (int i = 0; i < outputParams.Length; i++)
                    {
                        p.Add(outputParams[i], dbType: DbType.Int32, direction: ParameterDirection.Output);
                    }
                }

                await conn.ExecuteAsync(procedure, p, commandType: CommandType.StoredProcedure);
                if (outputParams.Length > 0)
                {
                    for (int i = 0; i < outputParams.Length; i++)
                    {
                        dict.Add(i.ToString(), p.Get<int>(outputParams[i]).ToString());
                    }
                }
            }
            return dict;
        }

    }
}
