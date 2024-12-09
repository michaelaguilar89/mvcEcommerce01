using Microsoft.AspNetCore.Identity;
using MVCEcommerce.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MVCEcommerce.Dto_s
{
    public class ProductDto
    {
        
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Required, MaxLength(150)]
        public string Description { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public int Stock { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public IFormFile? File { get; set; }
        public string? PublicId { get; set; }
        public string? Url {  get; set; }
       

    }
}
