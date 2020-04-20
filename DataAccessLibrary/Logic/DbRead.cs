using System.Collections.Generic;
using DataAccessLibrary.Models;
using DataAccessLibrary.Services;
using DataAccessLibrary.SqlDataAccess;
using DataAccessLibrary.Utilities;

namespace DataAccessLibrary.Logic
{
    public partial class DbAccessLogic
    {
        // Simple multi-row queries
        public List<ApprovalQueue> GetQueueDash()
        {
            return _dataAccessService.ExecuteProcedure<ApprovalQueue>("GetQueueAdminDashboard");
        }

        public List<LocalConcert> GetLocalConcerts()
        {
            return _dataAccessService.ExecuteProcedure<LocalConcert>("GetLocalConcerts");
        }
        public List<LocalConcert> GetApprovedLocalConcerts()
        {
            return _dataAccessService.ExecuteProcedure<LocalConcert>("GetApprovedLocalConcerts");
        }
        public List<HouseShow> GetApprovedHouseShows()
        {
            return _dataAccessService.ExecuteProcedure<HouseShow>("GetApprovedHouseShows");
        }

        public List<HouseShow> GetHouseShows()
        {
            return _dataAccessService.ExecuteProcedure<HouseShow>("GetHouseShows");
        }
        public List<TrustedCode> GetAllTrustedCodes()
        {
            return _dataAccessService.ExecuteProcedure<TrustedCode>("GetAllTrustedCodes");
        }

        public List<UserWithRole> GetUsersWithRoles()
        {
            return _dataAccessService.ExecuteProcedure<UserWithRole>("GetAllUsersWithRoles");
        }

        public List<AspNetRole> GetRoles()
        {
            return _dataAccessService.ExecuteProcedure<AspNetRole>("GetRoles");
        }

        public List<MessageModel> GetAdminMessages()
        {
            return _dataAccessService.ExecuteProcedure<MessageModel>("GetAdminMessages");
        }
        
        // Read the event dates
        public Dictionary<int, List<HouseShow>> ApprovedShowsByMonth()
        {
            var dict = new Dictionary<int, List<HouseShow>>();
            var list = GetApprovedShows();

            // sort list by month into dictionary
            foreach (var item in list)
            {
                var date = item.TimeStart;
                dict[date.Month] = new List<HouseShow>();
            }

            foreach (var item in list)
            {
                var date = item.TimeStart;
                dict[date.Month].Add(item);
            }

            return dict;
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
            var list = _dataAccessService.ExecuteProcedure<LocalConcert>("GetApprovedLocalConcerts");
            foreach (var concert in list)
            {
                var concertId = concert.EventConcertId;
                var allComments = GetAllComments(concertId);

                var tree = new CommentTree(allComments);
                concert.Comments = tree.GetEventComments();
            }

            return list;
        }
        private List<HouseShow> GetApprovedShows()
        {
            var list = _dataAccessService.ExecuteProcedure<HouseShow>("GetApprovedHouseShows");
            foreach (var concert in list)
            {
                var concertId = concert.EventConcertId;
                var allComments = GetAllComments(concertId);

                var tree = new CommentTree(allComments);
                concert.Comments = tree.GetEventComments();
            }

            return list;
        }
        public int UseTrustedCode(string code, string userId)
        {
            // first check if code exists in db
            int exists = _dataAccessService.ExecuteProcedure("CheckTrustedCode", "@count",
                Pairing.Of("@code", code));

            // if exists = 1 continue, if 0 immediately exit
            if (exists == 0) return exists;

            // else, use the code
            return _dataAccessService.ExecuteProcedure("UseCodeUser", null,
                Pairing.Of("@codeString", code),
                Pairing.Of("@userId", userId));
        }



        private Dictionary<int?, List<CommentModel>> GetAllComments(int eventId)
        {
            // Key: parentId; Value: list of child comments for that parentId
            var commentDict = new Dictionary<int?, List<CommentModel>>();
            
            var allComments 
                = _dataAccessService.ExecuteProcedure<CommentModel>("GetAllEventComments", 
                Pairing.Of("@eventId", eventId));
            
            // sort into dictionary
            foreach (var comment in allComments)
            {
                // root case:
                if (!commentDict.ContainsKey(0))
                {
                    commentDict.Add(0, new List<CommentModel>());
                }
                if (comment.ParentCommentId == null)
                {
                    commentDict[0].Add(comment);
                    continue;
                }
                // leaf case:
                var parentid = comment.ParentCommentId;
                if (!commentDict.ContainsKey(parentid))
                {
                    commentDict.Add(parentid, new List<CommentModel>());
                }
                
                if (comment.ParentCommentId == null)
                {
                    commentDict[0].Add(comment);
                }
                else
                {
                    commentDict[parentid].Add(comment);
                }
            }

            return commentDict;
        }


    }
}