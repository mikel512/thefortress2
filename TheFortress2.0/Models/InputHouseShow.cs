using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using DataAccessLibrary.Models;

namespace TheFortress.Models
{
    public class TrustedInputEvent
    {
        public LocalConcert Concert { get; set; }
        public HouseShow Show { get; set; }
        [Display(Name ="Flyer")]
        public IFormFile FlyerUrlUpload { get; set; }
        // [Required]
        // [Display(Name ="Artists")]
        // public string Artists { get; set; }
        // [Display(Name ="House/space name")]
        // public string HouseName { get; set; }
        // [Display(Name ="Capacity")]
        // public int Capacity { get; set; }
        // [Required]
        // [Display(Name ="Flyer")]
        // public IFormFile FlyerUrl { get; set; }
        //
        // [Required]
        // [Display(Name ="Date and Start Time")]
        // public DateTime TimeStart { get; set; }
        //
        // [Display(Name ="Time End (optional)")]
        // public DateTime? TimeEnd { get; set; }
        // [Display(Name="Notes, max 256 characters (optional)")]
        // [MaxLength(256)]
        // public string UserNotes { get; set; }
    }
}
