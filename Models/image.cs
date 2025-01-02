using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCEcommerce.Models
{
    public class image
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string PublicId { get; set; }
        [Required]
        public string Url { get; set; }
        [Required]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}
