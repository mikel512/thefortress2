using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using Dapper;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
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

        public string Connection()
        {
            // AppConfiguration conn = new AppConfiguration();
            // return conn.ConnectionString;
            var str = _configuration.Database.GetDbConnection().ConnectionString;
            return str;
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
            List<T> list = new List<T>();
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                var command = new SqlCommand(procedure, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                var p = new DynamicParameters(pairs);

                list = conn.Query<T>(command.CommandText, p).ToList();
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

        public static class Pairing
        {
            public static KeyValuePair<string, object> Of(string key, object value)
            {
                return new KeyValuePair<string, object>(key, value);
            }
        }
    }
}

//     public class AppConfiguration
//     {
//         public readonly string _connectionString = string.Empty;
//
//         public AppConfiguration()
//         {
//             var configurationBuilder = new ConfigurationBuilder();
//             var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
//             configurationBuilder.AddJsonFile(path, false);
//
//             var root = configurationBuilder.Build();
//             _connectionString = root.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
//             var appSetting = root.GetSection("ApplicationSettings");
//         }
//
//         public string ConnectionString
//         {
//             get => _connectionString;
//         }
//     }
// }