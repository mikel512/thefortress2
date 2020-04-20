using DataAccessLibrary.Services;

namespace DataAccessLibrary.Logic
{
    public partial class DbAccessLogic
    {
        private readonly DataAccessService _dataAccessService;

        public DbAccessLogic(DataAccessService dataAccessService)
        {
            _dataAccessService = dataAccessService;
        }
        
    }
}