using System.ComponentModel.DataAnnotations;

namespace DataAccessLibrary.Models
{
    public class AspNetRole
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
