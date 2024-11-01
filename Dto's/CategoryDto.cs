using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace MVCEcommerce.Dto_s
{
    public class CategoryDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
    }
}
