using DataAccessLibrary.Services;
using DataAccessLibrary.SqlDataAccess;
using DataAccessLibrary.Utilities;

namespace DataAccessLibrary.Logic
{
    public partial class DbAccessLogic
    {
        public int DeleteConcert(int localConcertId)
        {
            return _dataAccessService.ExecuteProcedureAsync("DeleteLocalConcert", "",
                Pairing.Of("@concertId", localConcertId)).Result;
        }
        public int DeleteShow(int showId)
        {
            return _dataAccessService.ExecuteProcedureAsync("DeleteHouseShow", "",
                Pairing.Of("@houseshowId", showId)).Result;
        }
        public int DeleteCode(int codeId)
        {
            return _dataAccessService.ExecuteProcedureAsync("DeleteTrustedCode", "",
                Pairing.Of("@codeid", codeId)).Result;
        }
        public int DeleteQueueEntry(int queueId)
        {
            return _dataAccessService.ExecuteProcedureAsync("DeleteQueueEntry", "",
                Pairing.Of("@queueid", queueId)).Result;
        }

    }
}
