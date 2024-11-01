namespace MVCEcommerce.Dto_s
{
    public class ProductListViewModel
    {
        public List<ProductResultDto> Products { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalProducts { get; set; }
        public string SearchQuery { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalProducts / PageSize);
    }
}
