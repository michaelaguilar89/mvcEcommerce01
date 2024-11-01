using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCEcommerce.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        
        public DateTime? Date { get; set; }
       
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }
    }
}
