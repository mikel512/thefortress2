using DataAccessLibrary.Services;
using DataAccessLibrary.SqlDataAccess;
using DataAccessLibrary.Utilities;

namespace DataAccessLibrary.Logic
{
    public partial class DbAccessLogic
    {
        private readonly DataAccessService _dataAccessService;

        public DbAccessLogic(DataAccessService dataAccessService)
        {
            _dataAccessService = dataAccessService;
        }
        public int DeleteConcert(int localConcertId)
        {
            return _dataAccessService.ExecuteProcedure("DeleteLocalConcert", "", 
                Pairing.Of("@concertId", localConcertId));
        }
        public int DeleteShow(int showId)
        {
            return _dataAccessService.ExecuteProcedure("DeleteHouseShow", "", 
                Pairing.Of("@houseshowId", showId));
        }
        public int DeleteCode(int codeId)
        {
            return _dataAccessService.ExecuteProcedure("DeleteTrustedCode", "",
                Pairing.Of("@codeid", codeId));
        }
        public int DeleteQueueEntry(int queueId)
        {
            return _dataAccessService.ExecuteProcedure("DeleteQueueEntry", "",
                Pairing.Of("@queueid", queueId));
        }

    }
}
