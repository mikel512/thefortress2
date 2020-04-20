using System.Collections.Generic;
using System.Linq;
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
            return _dataAccessService.ExecuteProcedureAsync<ApprovalQueue>("GetQueueAdminDashboard").Result.ToList();
        }

        public List<LocalConcert> GetLocalConcerts()
        {
            return _dataAccessService.ExecuteProcedureAsync<LocalConcert>("GetLocalConcerts").Result.ToList();
        }
        public List<LocalConcert> GetApprovedLocalConcerts()
        {
            return _dataAccessService.ExecuteProcedureAsync<LocalConcert>("GetApprovedLocalConcerts").Result.ToList();
        }
        public List<HouseShow> GetApprovedHouseShows()
        {
            return _dataAccessService.ExecuteProcedureAsync<HouseShow>("GetApprovedHouseShows").Result.ToList();
        }

        public List<HouseShow> GetHouseShows()
        {
            return _dataAccessService.ExecuteProcedureAsync<HouseShow>("GetHouseShows").Result.ToList();
        }
        public List<TrustedCode> GetAllTrustedCodes()
        {
            return _dataAccessService.ExecuteProcedureAsync<TrustedCode>("GetAllTrustedCodes").Result.ToList();
        }

        public List<UserWithRole> GetUsersWithRoles()
        {
            return _dataAccessService.ExecuteProcedureAsync<UserWithRole>("GetAllUsersWithRoles").Result.ToList();
        }

        public List<AspNetRole> GetRoles()
        {
            return _dataAccessService.ExecuteProcedureAsync<AspNetRole>("GetRoles").Result.ToList();
        }

        public List<MessageModel> GetAdminMessages()
        {
            return _dataAccessService.ExecuteProcedureAsync<MessageModel>("GetAdminMessages").Result.ToList();
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
            var list = _dataAccessService.ExecuteProcedureAsync<LocalConcert>("GetApprovedLocalConcerts").Result
                .ToList();
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
            var list = _dataAccessService.ExecuteProcedureAsync<HouseShow>("GetApprovedHouseShows").Result.ToList();
            foreach (var concert in list)
            {
                var concertId = concert.EventConcertId;
                var allComments = GetAllComments(concertId);

                var tree = new CommentTree(allComments);
                concert.Comments = tree.GetEventComments();
            }

            return list;
        }


        private Dictionary<int?, List<CommentModel>> GetAllComments(int eventId)
        {
            // Key: parentId; Value: list of child comments for that parentId
            var commentDict = new Dictionary<int?, List<CommentModel>>();

            var allComments
                = _dataAccessService.ExecuteProcedureAsync<CommentModel>("GetAllEventComments",
                    Pairing.Of("@eventId", eventId)).Result.ToList();
            
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