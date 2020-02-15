using DataAccessLibrary.SqlDataAccess;
using DataAccessLibrary.Utilities;

namespace DataAccessLibrary.Logic
{
    public class DbDelete
    {
        private readonly DataAccess _dataAccess;

        public DbDelete(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        public int DeleteConcert(int localConcertId)
        {
            return _dataAccess.ExecuteProcedure("DeleteLocalConcert", "", 
                Pairing.Of("@concertId", localConcertId));
        }
        public int DeleteCode(int codeId)
        {
            return _dataAccess.ExecuteProcedure("DeleteTrustedCode", "",
                Pairing.Of("@codeid", codeId));
        }
        public int DeleteQueueEntry(int queueId)
        {
            return _dataAccess.ExecuteProcedure("DeleteQueueEntry", "",
                Pairing.Of("@queueid", queueId));
        }

    }
}
