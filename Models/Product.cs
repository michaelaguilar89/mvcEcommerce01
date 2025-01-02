using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MVCEcommerce.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required,MaxLength(150)]
        public string Description { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public int Stock { get; set; }

        [Required]
        public DateTime CreationModificationDate { get; set; }
        public int CategoryId { get; set; }
        [Required]
        public string UserId { get; set; }
      
        public List<image> Images {get;set;}

        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
    }
}
