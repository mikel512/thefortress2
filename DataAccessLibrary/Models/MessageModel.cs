using System;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLibrary.Models
{
    public class MessageModel
    {
        [Display(Name = "Your Email")]
        [Required(ErrorMessage = "Email required")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Sender { get; set; }

        [Display(Name = "Your message (Max. 500 characters)")]
        [Required(ErrorMessage = "A message is required")]
        [MaxLength(500, ErrorMessage ="Message too long")]
        public string Message { get; set; }

        public DateTime Date { get; set; }

        [Display(Name = "Subject line")]
        public string Subject { get; set; }
    }
}
