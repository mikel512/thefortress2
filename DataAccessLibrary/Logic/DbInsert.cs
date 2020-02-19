using System;
using System.Collections.Generic;
using DataAccessLibrary.Models;
using DataAccessLibrary.SqlDataAccess;
using DataAccessLibrary.Utilities;

namespace DataAccessLibrary.Logic
{
    public class DbInsert
    {
        private readonly DataAccess _dataAccess;

        public DbInsert(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        // Admin create concert
        public Dictionary<string, string> CreateConcertDate(LocalConcert localConcert)
        {
            bool nullTimeEnd = localConcert.TimeEnd == DateTime.MinValue;
            var end = localConcert.TimeEnd;
            string[] output = new[] {"@eventId", "@concertId"};
            
            // 0 : parent row Id(event), 1: child row id (concert)
            return _dataAccess.ExecuteProcedureJson("InsertLocalConcert", output,
                Pairing.Of("@artists", localConcert.Artists),
                Pairing.Of("@flyerurl", localConcert.FlyerUrl),
                Pairing.Of("@timestart", localConcert.TimeStart),
                Pairing.Of("@timeend", (nullTimeEnd) ? null : end),
                Pairing.Of("isapproved", localConcert.IsApproved),
                Pairing.Of("@venue", localConcert.VenueName)
            );
        }

        public int CreateTrustedCode(string code)
        {
            return _dataAccess.ExecuteProcedure("InsertTrustedCode", null,
                Pairing.Of("@codetext", code.ToUpper()));
        }

        public int CreateTrustedCode(string code, int? max)
        {
            return _dataAccess.ExecuteProcedure("InsertTrustedCode", "@codeId",
                Pairing.Of("@codetext", code.ToUpper()),
                Pairing.Of("@maxTimes", max));
        }

        public int CreateQueuedDate(LocalConcert localConcert, string userId)
        {
            bool nullTimeEnd = localConcert.TimeEnd == DateTime.MinValue;
            var end = localConcert.TimeEnd;

            return _dataAccess.ExecuteProcedure("InsertConcertToApprovalQueue", null,
                Pairing.Of("@userId", userId),
                Pairing.Of("@event", localConcert.Artists),
                Pairing.Of("@venueName", localConcert.VenueName),
                Pairing.Of("@timeStart", localConcert.TimeStart),
                Pairing.Of("@flyerurl", localConcert.FlyerUrl),
                Pairing.Of("@timeend", (nullTimeEnd) ? null : end));
        }
        public int CreateQueuedDate(HouseShow houseShow, string userId)
        {
            bool nullTimeEnd = houseShow.TimeEnd == DateTime.MinValue;
            var end = houseShow.TimeEnd;

            return _dataAccess.ExecuteProcedure("InsertShowToApprovalQueue", null,
                Pairing.Of("@userId", userId),
                Pairing.Of("@event", houseShow.Artists),
                Pairing.Of("@venueName", houseShow.VenueName),
                Pairing.Of("@timeStart", houseShow.TimeStart),
                Pairing.Of("@flyerurl", houseShow.FlyerUrl),
                Pairing.Of("@timeend", (nullTimeEnd) ? null : end));
        }

        public int AddAdminMessage(MessageModel messageModel)
        {
            return _dataAccess.ExecuteProcedure("InsertAdminMessage", null,
                Pairing.Of("@sender", messageModel.Sender),
                Pairing.Of("@message", messageModel.Message),
                Pairing.Of("@date", messageModel.Date),
                Pairing.Of("@subject", messageModel.Subject)
            );
        }

        public int AddComment(CommentModel commentModel)
        {
            return _dataAccess.ExecuteProcedure("InsertComment", "commentId",
                Pairing.Of("@content", commentModel.Content),
                Pairing.Of("@userId", commentModel.UserId),
                Pairing.Of("@datestamp", commentModel.DateStamp),
                Pairing.Of("@eventId", commentModel.EventId),
                Pairing.Of("@username", commentModel.UserName),
                Pairing.Of("@parentId", commentModel.ParentCommentId));
        }
    }
}