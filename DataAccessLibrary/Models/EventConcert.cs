using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DataAccessLibrary.Models
{
    public class EventConcert
    {
        [Key] 
        public int EventConcertId { get; set; }
        
        [Required] 
        [Display(Name = "Artists")] 
        public string Artists { get; set; }

        [Display(Name = "Flyer URL")]
        public string FlyerUrl { get; set; }
        
        [Display(Name = "Flyer")]
        [FileExtensions(Extensions = "jpg,jpeg")]
        public IFormFile FlyerFile { get; set; }

        // TODO adjust validation so DateTime objects without a time field produce ajax error
        [Required]
        [Display(Name = "Date and Start Time")]
        public DateTime TimeStart { get; set; }

        [Display(Name = "Time End (optional)")]
        public DateTime? TimeEnd { get; set; }

        [Display(Name = "Is Approved?")] 
        public bool IsApproved { get; set; }
        
        [Display(Name = "Notes (optional)")] 
        public string UserNotes { get; set; }

        public List<CommentModel> Comments { get; set; }
    }
}