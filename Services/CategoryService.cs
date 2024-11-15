using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop.Infrastructure;
using MVCEcommerce.Data;
using MVCEcommerce.Dto_s;
using MVCEcommerce.Models;

namespace MVCEcommerce.Services
{
    public class CategoryService
    {
        private readonly ApplicationDbContext _context;
        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryResultDto>> GetCategories()
        {
            try
            {
                var category = await _context.Categorys
                .Include(c => c.User)
                .Select(p => new CategoryResultDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Date = p.Date,
                    UserId = p.UserId,
                    UserName = p.User.UserName
                }
               ).ToListAsync();
                return category;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error : "+e.Message);
                return null;
                
            }
        }

        public async Task<CategoryResultDto> DetailsWithUser(int? id)
        {
            try
            {
                var category = await _context.Categorys
                 .Include(c => c.User)
                 .Where(c => c.Id == id)
                 .Select(p => new CategoryResultDto
                 {
                     Id = p.Id,
                     Title = p.Title,
                     Date = p.Date,
                     UserId = p.UserId,
                     UserName = p.User.UserName
                 }
                ).FirstOrDefaultAsync();
                return category;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error : " + e.Message);
                return null;

            }
        }
        public async Task<CategoryDto> Details(int? id)
        {
            try
            {
                var categorie = await _context.Categorys.FindAsync(id);
                CategoryDto dto = new CategoryDto();

                if (categorie!=null)
                {
                    dto.Id = categorie.Id;
                    dto.Title = categorie.Title;
                    return dto;
                }
                return null;


            }
            catch (Exception e)
            {
                Console.WriteLine("Error : " + e.Message);
                return null;

            }
        }
        public async Task<int> Exist(string title)
        {
            try
            {
                var exist = _context.Categorys
                .Any(x => x.Title.ToLower().Equals(title.ToLower()) );
                if (exist)
                {
                    return 1;
                }
                return 0;
            }
            catch (Exception)
            {

                return 0;
            }
        }
        public async Task<string> AddOrEdit(CategoryDto dto, string secret)
        {
            try
            {
                string state = string.Empty;
                Category category = new Category
                {
                    Id = dto.Id,
                    Title = dto.Title,
                    Date = DateTime.Now,
                    UserId = secret
                };
                if (category.Id == 0)
                {
                    state = "1";
                    await _context.AddAsync(category); // Agregar categoría nueva
                }
                else
                {
                    state = "2";
                    _context.Update(category); // Editar categoría existente
                }

                await _context.SaveChangesAsync();
                return state;
            }
            catch (Exception e)
            {

                Console.WriteLine("Error : " + e.Message);
                return e.Message;
            }
        }

        public async Task<string> Delete(int? id)
        {
            try
            {
                string state = string.Empty;
                if (id>0)
                {
                    //if category exist?
                    var category = await _context.Categorys.FindAsync(id);
                    if (category != null)
                    {
                        var product= _context.Products
                            .Any(x=>x.CategoryId== category.Id);
                        if (product)
                        {
                           
                            return "Category : " +category.Title+" is used by Products!";
                        }
                        else
                        {
                            //delete categori
                            _context.Categorys.Remove(category);
                            await _context.SaveChangesAsync();
                            return "1";
                        }
                      
                    }
                    return "Product Not Found!";

                    
                }
                return "Product Not Found!";
            }   
            catch (Exception e)
            {

                Console.WriteLine("Error : " + e.Message);
                return e.Message;
            }
        
        }
    }
}
