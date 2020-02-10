using System.Collections.Generic;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.SqlDataAccess
{
    public class DataRead : DataAccess
    {
        public int UseTrustedCode(string code, string userId)
        {
            // first check if code exists in db
            int exists = ExecuteProcedure("CheckTrustedCode", "@count",
                Pairing.Of("@code", code));

            // if exists = 1 continue, if 0 immediately exit
            if (exists == 0) return exists;

            // else, use the code
            return ExecuteProcedure("UseCodeUser", null,
                Pairing.Of("@codeString", code),
                Pairing.Of("@userId", userId));
        }

        public List<ApprovalQueue> GetQueueDash()
        {
            return ExecuteProcedure<ApprovalQueue>("GetQueueAdminDashboard");
        }

        public List<LocalConcert> GetLocalConcerts()
        {
            return ExecuteProcedure<LocalConcert>("GetLocalConcerts");
        }

        public List<HouseShow> GetHouseShows()
        {
            return ExecuteProcedure<HouseShow>("GetHouseShows");
        }

        public Dictionary<int, List<LocalConcert>> ApprovedConcertsByMonth()
        {
            var dict = new Dictionary<int, List<LocalConcert>>();
            var list = GetApprovedConcerts();

            // sort list by month into dictionary
            foreach (var item in list)
            {
                var date = item.TimeStart;
                dict[date.Month] = new List<LocalConcert>();
            }

            foreach (var item in list)
            {
                var date = item.TimeStart;
                dict[date.Month].Add(item);
            }

            return dict;
        }

        private List<LocalConcert> GetApprovedConcerts()
        {
            var list = ExecuteProcedure<LocalConcert>("GetApprovedLocalConcerts");
            foreach (var concert in list)
            {
                var concertId = concert.EventConcertId;

                // var tree = new CommentTree();
                // concert.Comments = tree.GetEventComments(concertId);
            }

            return list;
        }

        private List<HouseShow> GetApprovedShows()
        {
            var list = ExecuteProcedure<HouseShow>("GetApprovedHouseShows");
            foreach (var concert in list)
            {
                var concertId = concert.EventConcertId;

                // var tree = new CommentTree();
                // concert.Comments = tree.GetEventComments(concertId);
            }

            return list;
        }

        public List<TrustedCode> GetAllTrustedCodes()
        {
            return ExecuteProcedure<TrustedCode>("GetAllTrustedCodes");
        }

        public List<UserWithRole> GetUsersWithRoles()
        {
            return ExecuteProcedure<UserWithRole>("GetAllUsersWithRoles");
        }

        public List<AspNetRole> GetRoles()
        {
            return ExecuteProcedure<AspNetRole>("GetRoles");
        }

        public List<MessageModel> GetAdminMessages()
        {
            return ExecuteProcedure<MessageModel>("GetAdminMessages");
        }

        public DataRead(ApplicationDbContext configuration) : base(configuration)
        {
        }
    }
}