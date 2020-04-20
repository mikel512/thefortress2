using System.Collections.Generic;

namespace DataAccessLibrary.Services
{
    public interface IDataAccessService
    {
        string ConnectionString { get; set; }
        List<T> ExecuteProcedure<T>(string procedure);
        List<T> ExecuteProcedure<T>(string procedure, params KeyValuePair<string, object>[] pairs);
        int ExecuteProcedure(string procedure, string outputParam, params KeyValuePair<string, object>[] pairs);
    }
}