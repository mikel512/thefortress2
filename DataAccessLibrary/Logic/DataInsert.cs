using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using DataAccessLibrary.Models;
using DataAccessLibrary.SqlDataAccess;

namespace DataAccessLibrary.Logic
{
    public class DataInsert : DataAccess
    {
        // Admin create concert
        public Dictionary<string, string> CreateConcertDate(LocalConcert localConcert)
        {
            var list = new Dictionary<string, string>();
            var check = DateTime.MinValue;
            using (var conn = new SqlConnection(ConnectionString))
            {
                var p = new DynamicParameters();
                p.Add("@artists", localConcert.Artists);
                p.Add("@flyerurl", localConcert.FlyerUrl);
                p.Add("@timestart", localConcert.TimeStart);
                if (localConcert.TimeEnd != check) p.Add("@timeend", localConcert.TimeEnd);
                p.Add("isapproved", localConcert.IsApproved);
                p.Add("@venue", localConcert.VenueName);
                p.Add("@eventId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("@concertId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                conn.Execute("InsertLocalConcert", p, commandType: CommandType.StoredProcedure);
                list["0"] = p.Get<int>("@eventId").ToString();
                list["1"] = p.Get<int>("@concertId").ToString();
            }

            // 0 : parent row Id(event), 1: child row id (concert)
            return list;
        }

        public int CreateTrustedCode(string code)
        {
            return ExecuteProcedure("InsertTrustedCode", null,
                Pairing.Of("@codetext", code.ToUpper()));
        }

        public int CreateTrustedCode(string code, int? max)
        {
            return ExecuteProcedure("InsertTrustedCode", "@codeId",
                Pairing.Of("@codetext", code.ToUpper()),
                Pairing.Of("@maxTimes", max));
        }

        public int CreateQueuedDate(LocalConcert localConcert, string userId)
        {
            bool nullTimeEnd = localConcert.TimeEnd == DateTime.MinValue;
            var end = localConcert.TimeEnd;

            return ExecuteProcedure("InsertConcertToApprovalQueue", null,
                Pairing.Of("@userId", userId),
                Pairing.Of("@artists", localConcert.Artists),
                Pairing.Of("@venueName", localConcert.VenueName),
                Pairing.Of("@timeStart", localConcert.TimeStart),
                Pairing.Of("@flyerurl", localConcert.FlyerUrl),
                Pairing.Of("@timeend", (nullTimeEnd) ? null : end));
        }

        public int AddAdminMessage(MessageModel messageModel)
        {
            return ExecuteProcedure("InsertAdminMessage", null,
                Pairing.Of("@sender", messageModel.Sender),
                Pairing.Of("@message", messageModel.Message),
                Pairing.Of("@date", messageModel.Date),
                Pairing.Of("@subject", messageModel.Subject)
            );
        }

        public int AddComment(CommentModel commentModel)
        {
            return ExecuteProcedure("InsertComment", "commentId",
                Pairing.Of("@content", commentModel.Content),
                Pairing.Of("@userId", commentModel.UserId),
                Pairing.Of("@datestamp", commentModel.DateStamp),
                Pairing.Of("@eventId", commentModel.EventId),
                Pairing.Of("@username", commentModel.UserName),
                Pairing.Of("@parentId", commentModel.ParentCommentId));
        }

        public DataInsert(ApplicationDbContext configuration) : base(configuration)
        {
        }
    }
}