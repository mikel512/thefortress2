using System.Collections.Generic;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.Logic
{
    public interface IDbAccessLogic
    {
        // Read
        List<ApprovalQueue> GetQueueDash();
        List<LocalConcert> GetLocalConcerts();
        List<LocalConcert> GetApprovedLocalConcerts();
        List<HouseShow> GetApprovedHouseShows();
        List<HouseShow> GetHouseShows();
        List<TrustedCode> GetAllTrustedCodes();
        List<UserWithRole> GetUsersWithRoles();
        List<AspNetRole> GetRoles();
        List<MessageModel> GetAdminMessages();
        Dictionary<int, List<HouseShow>> ApprovedShowsByMonth();
        Dictionary<int, List<LocalConcert>> ApprovedConcertsByMonth();
        // Insert
        int CreateConcertDate(LocalConcert localConcert);
        int CreateTrustedCode(string code);
        int CreateTrustedCode(string code, int? max);
        int CreateQueuedDate(LocalConcert localConcert, string userId);
        int CreateQueuedDate(HouseShow houseShow, string userId);
        int AddAdminMessage(MessageModel messageModel);
        int AddComment(CommentModel commentModel);
        // Delete
        int DeleteConcert(int localConcertId);
        int DeleteShow(int showId);
        int DeleteCode(int codeId);
        int DeleteQueueEntry(int queueId);
        // Update
        int UseTrustedCode(string code, string userId);
        int ApproveQueueItem(int itemId);
    }
}