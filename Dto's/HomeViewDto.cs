namespace MVCEcommerce.Dto_s
{
    public class HomeViewDto
    {
        public List<ProductViewDto> List { get; set; }
        public string SearchQuery { get; set; }
        public int Count {  get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalProducts { get; set; }
    }

}
