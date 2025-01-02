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
                    .Include(c => c.Images)
                    .Where(c=>c.Name.ToLower().Contains( search.ToLower()))
                    .Select(m => new ProductViewDto
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Price = m.Price,
                        Stock = m.Stock,
                        CategoryId = m.CategoryId,
                        CategoryName = m.Category.Title,
                        Images = m.Images.Select(s => new ImageDto
                         {
                             Id = s.Id,
                             Url = s.Url
                         }).ToList()
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
                    .Include(c => c.Category)
                    .Include(c => c.Images)
                    .Select(m => new ProductViewDto
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Price = m.Price,
                        Stock = m.Stock,
                        CategoryId = m.CategoryId,
                        CategoryName = m.Category.Title,
                        Images =m.Images.Select( s=> new ImageDto
                        {
                            Id=s.Id,
                            Url= s.Url
                        }).ToList()
       
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
