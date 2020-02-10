using System.ComponentModel.DataAnnotations;

namespace DataAccessLibrary.Models
{
    public class TrustedCode
    {
        [Key]
        public int TrustedCodeId { get; set; }
        [Required]
        [Display(Name ="Desired code")]
        public string CodeString { get; set; }
        public int TimesUsed { get; set; }
        [Display(Name ="Maximum times code can be used")]
        public int? MaxTimesUsed { get; set; }
    }
}
