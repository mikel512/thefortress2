using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using Dapper;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace DataAccessLibrary.SqlDataAccess
{
    public class DataAccess : IDataAccess
    {
        private ApplicationDbContext _configuration;
        private string _connection;

        public DataAccess(ApplicationDbContext configuration)
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

    }
}
