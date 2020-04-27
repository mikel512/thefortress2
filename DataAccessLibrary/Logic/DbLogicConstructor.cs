using DataAccessLibrary.Services;
using Microsoft.Extensions.Configuration;

namespace DataAccessLibrary.Logic
{
    public partial class DbAccessLogic : IDbAccessLogic
    {
        private readonly DataAccessService _dataAccessService;

        public DbAccessLogic(IConfiguration configuration)
        {
            _dataAccessService = new DataAccessService(configuration["Connection"]);
        }
        
    }
}