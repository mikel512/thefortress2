using System.ComponentModel.DataAnnotations;

namespace DataAccessLibrary.Models
{
    public class HouseShow : EventConcert
    {
        [Key] public int HouseShowId { get; set; }
        public string HouseName { get; set; }
        public int Capacity { get; set; }
        [Key] public int EventConcertFKId { get; set; }
        
    }
}