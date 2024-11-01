namespace MVCEcommerce.Dto_s
{
    public class ProductViewDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Price { get; set; }

        public int Stock { get; set; }


        public int CategoryId { get; set; }

        public string CategoryName { get; set; }
        public string Url { get; set; }

    }
}
