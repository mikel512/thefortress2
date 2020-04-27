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
        
        [Required(ErrorMessage = "Flyer image is required")]
        [Display(Name = "Flyer upload (only jpegs and jpgs allowed)")]
        // [FileExtensions(Extensions = ".jpg, .jpeg", ErrorMessage = "Only .jpeg and .jpg files are allowed.")]
        public IFormFile FlyerFile { get; set; }

        [Required]
        [Display(Name = "Date and Start Time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy HH/mm}")]
        public DateTime TimeStart { get; set; }

        [Display(Name = "Time End (optional)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy HH/mm}")]
        public DateTime? TimeEnd { get; set; }

        [Display(Name = "Is Approved?")] 
        public bool IsApproved { get; set; }
        
        [Display(Name = "Notes (optional)")] 
        public string UserNotes { get; set; }

        public List<CommentModel> Comments { get; set; }
    }
}