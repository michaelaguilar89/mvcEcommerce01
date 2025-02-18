﻿using System.ComponentModel.DataAnnotations;

namespace MVCEcommerce.Dto_s
{
    public class ProductForUpdatesDto
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
        public DateTime CreationDate { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public List<IFormFile>? Files { get; set; } 

        public List<CategoriesDto> Categories { get; set; } 
        public List<ImageForUpdateDto>? Images { get; set; } 
    }

    public class ImageForUpdateDto
    {
        public int Id { get; set; }
        public string PublicId { get; set; }
        public string Url { get; set; }
        public bool Remove { get; set; } = true;
    }
}
