using System.ComponentModel.DataAnnotations;

namespace DataAccessLibrary.Models
{
    public class HouseShow : EventConcert
    {
        [Key] public int HouseShowId { get; set; }
        [Key] public int EventConcertFKId { get; set; }

        [Required]
        [Display(Name = "House Name")]
        public string VenueName { get; set; }

        [Required]
        [Display(Name = "Capacity")]
        public int Capacity { get; set; }
    }
}