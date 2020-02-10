using System.ComponentModel.DataAnnotations;

namespace DataAccessLibrary.Models
{

    public class LocalConcert : EventConcert
    {
        [Key]
        public int LocalConcertId { get; set; }
        [Required]
        [Display(Name ="Venue")]
        public string VenueName { get; set; }
        [Key]
        public int EventConcertFKId { get; set; }

    }
}
