using System;
using System.Collections.Generic;
using DataAccessLibrary.Models;
using DataAccessLibrary.Services;
using DataAccessLibrary.SqlDataAccess;
using DataAccessLibrary.Utilities;

namespace DataAccessLibrary.Logic
{
    public partial class DbAccessLogic
    {
        // Admin create concert
        public int CreateConcertDate(LocalConcert localConcert)
        {
            bool nullTimeEnd = localConcert.TimeEnd == DateTime.MinValue;
            var end = localConcert.TimeEnd;
            
            // 0 : parent row Id(event), 1: child row id (concert)
            return _dataAccessService.ExecuteProcedureAsync("InsertLocalConcert", "@concertId",
                Pairing.Of("@artists", localConcert.Artists),
                Pairing.Of("@flyerurl", localConcert.FlyerUrl),
                Pairing.Of("@timestart", localConcert.TimeStart),
                Pairing.Of("@timeend", (nullTimeEnd) ? null : end),
                Pairing.Of("isapproved", localConcert.IsApproved),
                Pairing.Of("@venue", localConcert.VenueName)
            ).Result;
        }

        public int CreateTrustedCode(string code)
        {
            return _dataAccessService.ExecuteProcedureAsync("InsertTrustedCode", null,
                Pairing.Of("@codetext", code.ToUpper())).Result;
        }

        public int CreateTrustedCode(string code, int? max)
        {
            return _dataAccessService.ExecuteProcedureAsync("InsertTrustedCode", "@codeId",
                Pairing.Of("@codetext", code.ToUpper()),
                Pairing.Of("@maxTimes", max)).Result;
        }

        // Tested and working
        public int CreateQueuedDate(LocalConcert localConcert, string userId)
        {
            bool nullTimeEnd = localConcert.TimeEnd == DateTime.MinValue;
            var end = localConcert.TimeEnd;

            return _dataAccessService.ExecuteProcedureAsync("InsertConcertToApprovalQueue", null,
                Pairing.Of("@userId", userId),
                Pairing.Of("@event", localConcert.Artists),
                Pairing.Of("@venueName", localConcert.VenueName),
                Pairing.Of("@timeStart", localConcert.TimeStart),
                Pairing.Of("@flyerurl", localConcert.FlyerUrl),
                Pairing.Of("@timeend", (nullTimeEnd) ? null : end)).Result;
        }
        public int CreateQueuedDate(HouseShow houseShow, string userId)
        {
            bool nullTimeEnd = houseShow.TimeEnd == DateTime.MinValue;
            var end = houseShow.TimeEnd;

            return _dataAccessService.ExecuteProcedureAsync("InsertShowToApprovalQueue", null,
                Pairing.Of("@userId", userId),
                Pairing.Of("@event", houseShow.Artists),
                Pairing.Of("@venueName", houseShow.VenueName),
                Pairing.Of("@timeStart", houseShow.TimeStart),
                Pairing.Of("@flyerurl", houseShow.FlyerUrl),
                Pairing.Of("@timeend", (nullTimeEnd) ? null : end)).Result;
        }

        public int AddAdminMessage(MessageModel messageModel)
        {
            return _dataAccessService.ExecuteProcedureAsync("InsertAdminMessage", null,
                Pairing.Of("@sender", messageModel.Sender),
                Pairing.Of("@message", messageModel.Message),
                Pairing.Of("@date", messageModel.Date),
                Pairing.Of("@subject", messageModel.Subject)
            ).Result;
        }

        public int AddComment(CommentModel commentModel)
        {
            return _dataAccessService.ExecuteProcedureAsync("InsertComment", "commentId",
                Pairing.Of("@content", commentModel.Content),
                Pairing.Of("@userId", commentModel.UserId),
                Pairing.Of("@datestamp", commentModel.DateStamp),
                Pairing.Of("@eventId", commentModel.EventId),
                Pairing.Of("@username", commentModel.UserName),
                Pairing.Of("@parentId", commentModel.ParentCommentId)).Result;
        }
    }
}