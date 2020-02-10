using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLibrary.Models
{
    public class EventConcert
    {
        [Key] public int EventConcertId { get; set; }
        [Required] [Display(Name = "Artists")] public string Artists { get; set; }

        [Required]
        [Display(Name = "Flyer URL")]
        public string FlyerUrl { get; set; }

        [Required]
        [Display(Name = "Date and Start Time")]
        public DateTime TimeStart { get; set; }

        [Display(Name = "Time End (optional)")]
        public DateTime? TimeEnd { get; set; }

        [Display(Name = "Is Approved?")] public bool IsApproved { get; set; }
        [Display(Name = "Notes (optional)")] public string UserNotes { get; set; }

        public List<CommentModel> Comments { get; set; }
    }
}