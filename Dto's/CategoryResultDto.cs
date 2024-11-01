using System.ComponentModel.DataAnnotations;

namespace MVCEcommerce.Dto_s
{
    public class CategoryResultDto
    {
        
        public int Id { get; set; }
        
        public string Title { get; set; }

        public DateTime? Date { get; set; }

        public string UserId { get; set; }
        public string UserName { get; set; }

    }
}
