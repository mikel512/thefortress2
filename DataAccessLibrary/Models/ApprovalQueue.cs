using System.ComponentModel.DataAnnotations;

namespace DataAccessLibrary.Models
{
    public class ApprovalQueue
    {
        [Key]
        public int QueueId { get; set; }
        public string UserName { get; set; }
        public int LocalConcertId { get; set; }
    }
}
