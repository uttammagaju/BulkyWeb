using System.ComponentModel.DataAnnotations;

namespace BulkyWeb.Models
{
    public class Category
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public String Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}
