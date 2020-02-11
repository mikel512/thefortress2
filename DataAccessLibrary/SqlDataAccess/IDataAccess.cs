using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.SqlDataAccess
{
    public interface IDataAccess
    {
        string ConnectionString { get; set; }
        List<T> ExecuteProcedure<T>(string procedure);
        List<T> ExecuteProcedure<T>(string procedure, params KeyValuePair<string, object>[] pairs);
        int ExecuteProcedure(string procedure, string outputParam, params KeyValuePair<string, object>[] pairs);
    }
}