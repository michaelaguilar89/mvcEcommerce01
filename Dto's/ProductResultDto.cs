using Microsoft.AspNetCore.Identity;
using MVCEcommerce.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MVCEcommerce.Dto_s
{
    public class ProductResultDto
    {
       
        public int Id { get; set; }
       
        public string Name { get; set; }
        public string Description { get; set; }

        public int Price { get; set; }
       
        public int Stock { get; set; }

       
        public DateTime CreationModificationDate { get; set; }
        public int CategoryId { get; set; }
       
        public string CategoryName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }

        public string? PublicId { get; set; }
        public string? Url { get; set; }

    }
}
