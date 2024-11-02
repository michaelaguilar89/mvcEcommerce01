using Microsoft.EntityFrameworkCore;
using MVCEcommerce.Data;
using MVCEcommerce.Dto_s;

namespace MVCEcommerce.Services
{
    public class HomeService
    {
        private readonly ApplicationDbContext _context;
        public HomeService(ApplicationDbContext context)
        {
            _context = context;
        }
       
        public async Task<List<ProductViewDto>> GetProductsWithPaginationAndSearch(string search, int pageNumber, int pageSize)
        {
            try
            {
               
                var products = await _context.Products
                    .Include(c => c.Category)
                    .Where(c=>c.Name.ToLower().Contains( search.ToLower()))
                    .Select(m => new ProductViewDto
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Price = m.Price,
                        Stock = m.Stock,
                        CategoryId = m.CategoryId,
                        CategoryName = m.Category.Title,
                        Url = m.Url
                    }).Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                    .ToListAsync();

                return products;
            }
            catch (Exception e)
            {
                return null;

            }
        }
        public async Task<List<ProductViewDto>> GetProductsWithPagination(int pageNumber, int pageSize)
        {
            try
            {
                var products = await _context.Products
                    .Include(c=>c.Category)
                    .Select(m=> new ProductViewDto
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Price= m.Price,
                        Stock=m.Stock,
                        CategoryId=m.CategoryId,
                        CategoryName=m.Category.Title,
                        Url= m.Url
                    }).Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                    .ToListAsync();

                return products;
            }
            catch (Exception e)
            {
                return null;
              
            }
        }
    }
}
