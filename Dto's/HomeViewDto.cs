namespace MVCEcommerce.Dto_s
{
    public class HomeViewDto
    {
        public List<ProductViewDto> List { get; set; }
        public string searchQuery { get; set; }
        public int Count {  get; set; }
    }

}
