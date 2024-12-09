using CloudinaryDotNet;
using dotenv.net;

using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using MVCEcommerce.Data;
using MVCEcommerce.Dto_s;
using MVCEcommerce.Models;

namespace MVCEcommerce.Services
{
    public class ProductService
    {
        private Cloudinary cloudinary;
        private readonly ApplicationDbContext _context;
        public ProductService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            var variable = configuration["CloudBinary:variable"];
            DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
            cloudinary = new Cloudinary(variable.ToString());
            cloudinary.Api.Secure = true;

        }


        private async Task<PictureDto> UploadPictureAsync(Stream stream, string folder, string fileName)
        {

            Console.WriteLine("upload picture on Cloudbinaryservice....");
            UploadResult uploadResult = null;


            ImageUploadParams uploadParams = new ImageUploadParams
            {
                Folder = folder,
                File = new FileDescription(fileName, stream)
            };

            uploadResult = await this.cloudinary.UploadAsync(uploadParams);

            Console.WriteLine($"Uploading status :  {uploadResult.Status}");
            PictureDto result = new PictureDto();
            result.PublicId = uploadResult.PublicId;
            result.Url = uploadResult.Url.ToString();
            return result;
        }

        public async Task<DeletionResult> DeleteImageAsync(string publicId)
        {

            var deletionParams = new DeletionParams(publicId);
            var result = await this.cloudinary.DestroyAsync(deletionParams);
            return result;
        }


        public async Task<List<ProductResultDto>> GetProductsWithPagination(int pageNumber,int pageSize)
        {
            try
            {
                var products = await _context.Products
                .Include(c => c.User)
                .Include(c => c.Category)
                .Select(c => new ProductResultDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description=c.Description,
                    Price = c.Price,
                    Stock = c.Stock,
                    CreationModificationDate = c.CreationModificationDate,
                    CategoryId = c.CategoryId,
                    CategoryName = c.Category.Title,
                    UserId = c.User.Id,
                    UserName = c.User.UserName,
                    Url = c.Url
                }
                ).Skip((pageNumber-1)*pageSize)
                .Take(pageSize)
                .ToListAsync();
                return products;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error : "+e.Message);
                return null;
                
            }
        }

        public async Task<List<ProductResultDto>> GetProductsWithPaginationAndSearch(int pageNumber, int pageSize,string search)
        {
            try
            {
                var products = await _context.Products
                .Include(c => c.User)
                .Include(c => c.Category)
                .Where(x=>x.Name.ToLower().Contains(search.ToLower()))
                .Select(c => new ProductResultDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Price = c.Price,
                    Stock = c.Stock,
                    CreationModificationDate = c.CreationModificationDate,
                    CategoryId = c.CategoryId,
                    CategoryName = c.Category.Title,
                    UserId = c.User.Id,
                    UserName = c.User.UserName,
                    PublicId=c.PublicId,
                    Url= c.Url
                }
                ).Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
                return products;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error : " + e.Message);
                return null;

            }
        }

        public async Task<string> AddOrEdit(ProductDto dto,string secret)
        {
            string response = string.Empty;
            try
            {
                
                Product product = new Product();
                product.Id = dto.Id;
                product.Name = dto.Name;
                product.Description = dto.Description;
                product.Price = dto.Price;
                product.Stock = dto.Stock;
                product.CreationModificationDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                product.CategoryId = dto.CategoryId;
                product.UserId = secret;
                
                if (dto.Id > 0)//update
                {
                    response = "2";
                    if (dto.File!=null)
                    {
                        //remove  actual image for product
                        await DeleteImageAsync(dto.PublicId);

                        var stream = dto.File.OpenReadStream();

                        PictureDto data = new PictureDto();
                        // save new image for product
                        data = await UploadPictureAsync(stream, "MVCEcommerce", dto.Name);
                        product.PublicId = data.PublicId;
                        product.Url = data.Url;
                    }
                    else
                    {
                        product.Url = dto.Url;
                        product.PublicId = dto.PublicId;
                    }
                   
                    _context.Products.Update(product);


                }
                else
                {//add new
                    response = "1";
                   
                    // Convertir el archivo seleccionado a un stream
                    var stream = dto.File.OpenReadStream();

                    PictureDto data = new PictureDto();
                    data =await UploadPictureAsync(stream, "MVCEcommerce", dto.Name);
                    product.PublicId = data.PublicId;
                    product.Url = data.Url;
                    await _context.Products.AddAsync(product);

                }
                //save on database
                await _context.SaveChangesAsync();
                return response;

            }
            catch (Exception e)
            {
                return e.Message;
                
            }
        }

        public async Task<ProductResultDto> getProductById(int? id){

            try
            {
                var product = await _context.Products
               .Where(c => c.Id == id)
               .Select(c => new ProductResultDto
               {
                   Id = c.Id,
                   Name = c.Name,
                   Description = c.Description,
                   Price = c.Price,
                   Stock = c.Stock,
                   CreationModificationDate = c.CreationModificationDate,
                   CategoryName = c.Category.Title,
                   UserName = c.User.UserName,
                   PublicId = c.PublicId,
                   Url = c.Url
               }).FirstOrDefaultAsync();

                return product;
            }
            catch (Exception)
            {
                return null;
             
            }
            
        }

        public async Task<string> Delete(int? id)
        {
            try
            {
                var exist = await _context.Products.FindAsync(id);
                if (exist != null)
                {
                    //remove product
                    _context.Products.Remove(exist);
                    await _context.SaveChangesAsync();
                    return "1";
                }
                return "Product Not Found!";
            }
            catch (Exception e)
            {
                return e.Message;
                throw;
            }
        }
    }
}
