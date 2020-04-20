using DataAccessLibrary.Utilities;

namespace DataAccessLibrary.Logic
{
    public partial class DbAccessLogic
    {
        public int UseTrustedCode(string code, string userId)
        {
            // first check if code exists in db
            int exists = _dataAccessService.ExecuteProcedureAsync("CheckTrustedCode", "@count",
                Pairing.Of("@code", code)).Result;

            // if exists = 1 continue, if 0 immediately exit
            if (exists == 0) return exists;

            // else, use the code
            return _dataAccessService.ExecuteProcedureAsync("UseCodeUser", null,
                Pairing.Of("@codeString", code),
                Pairing.Of("@userId", userId)).Result;
        }

        public int ApproveQueueItem(int itemId)
        {
            return _dataAccessService.ExecuteProcedureAsync("ApproveQueueItem", "",
                Pairing.Of("@queueId", itemId)).Result;
        }
        
    }
}