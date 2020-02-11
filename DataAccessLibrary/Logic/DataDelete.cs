using DataAccessLibrary.SqlDataAccess;

namespace DataAccessLibrary.Logic
{
    public class DataDelete : DataAccess
    {
        public int DeleteConcert(int eventId, int localConcertId)
        {
            return ExecuteProcedure("DeleteLocalConcert", "", 
                Pairing.Of("@eventId", eventId), Pairing.Of("@concertId", localConcertId));
        }
        public int DeleteCode(int codeId)
        {
            return ExecuteProcedure("DeleteTrustedCode", "",
                Pairing.Of("@codeid", codeId));
        }
        public int DeleteQueueEntry(int queueId)
        {
            return ExecuteProcedure("DeleteQueueEntry", "",
                Pairing.Of("@queueid", queueId));
        }

        public DataDelete(ApplicationDbContext configuration) : base(configuration)
        {
        }
    }
}
