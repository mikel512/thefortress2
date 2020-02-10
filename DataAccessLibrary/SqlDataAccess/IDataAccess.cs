using System.Collections.Generic;

namespace DataAccessLibrary.SqlDataAccess
{
    public interface IDataAccess
    {
        string ConnectionString { get; set; }
        string Connection();
        List<T> ExecuteProcedure<T>(string procedure);
        List<T> ExecuteProcedure<T>(string procedure, params KeyValuePair<string, object>[] pairs);
        int ExecuteProcedure(string procedure, string outputParam, params KeyValuePair<string, object>[] pairs);
    }
}