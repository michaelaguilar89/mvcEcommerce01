using CloudinaryDotNet;
using dotenv.net;

using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using MVCEcommerce.Data;
using MVCEcommerce.Dto_s;
using MVCEcommerce.Models;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.DataProtection;

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
                .Include(C=> C.Images) 
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
                    Images= c.Images.Select(img => new ImageDto
                    {
                        Id= img.Id,
                        PublicId=img.PublicId,
                        Url=img.Url
                    }).ToList()
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
        
        public async Task<string> Edit(ProductForUpdatesDto dto,string secret)
        {
            try
            {
                if (dto!=null)
                {
                    Product product = new Product();
                    product.Id = dto.Id;
                    product.Name = dto.Name;
                    product.Description=dto.Description;
                    product.Price = dto.Price;
                    product.Stock = dto.Stock;
                    product.CreationModificationDate = dto.CreationDate;
                    product.CategoryId = dto.CategoryId;
                    product.UserId = secret;
                    //update information in database
                    Console.WriteLine("update information in database");
                    _context.Products.Update(product);
                    await _context.SaveChangesAsync();
                    if (dto.Files!=null && dto.Files.Count>0)
                    {
                        //add more images when Files is more zero,
                        //add to cloudinary and database
                        foreach (var item in dto.Files)
                        {

                            var stream = item.OpenReadStream();

                            PictureDto data = new PictureDto();
                            // save new image for product on Cloudinary
                            data = await UploadPictureAsync(stream, "MVCEcommerce", dto.Name);

                            image newImage = new();
                            newImage.ProductId = product.Id;
                            newImage.PublicId = data.PublicId;
                            newImage.Url = data.Url;

                            //save image data on database
                            await _context.Images.AddAsync(newImage);
                            await _context.SaveChangesAsync();
                            Console.WriteLine(  "Add more images on database and cloudinary");
                            //remove  actual image for product
                            //  await DeleteImageAsync(dto.PublicId);




                        }//end of foreach

                    }//end of if

                    if (dto.Images!=null && dto.Images.Count>0)
                    {
                        foreach (var item in dto.Images)
                        {
                            if (item.Remove)
                            {
                                var state = DeleteImageAsync(item.PublicId);
                                var image = await _context.Images.FindAsync(item.Id);
                                if (image!=null)
                                {
                                    _context.Images.Remove(image);
                                    await _context.SaveChangesAsync();
                                }
                            }
                        }
                    }//remove all images when remove equals true , from 
                    //cloudinary and database
                    Console.WriteLine("remove all images when remove equals true , from cloudinary and database");
                    return "1";
                }
                return "Model is Wrong!";
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in ProductService-Edit : Date :" +DateTime.UtcNow+" , Error : "+e.Message);
                return e.Message;
            }
        }    
        public async Task<List<ProductResultDto>> GetProductsWithPaginationAndSearch(int pageNumber, int pageSize,string search)
        {
            try
            {
                var products = await _context.Products
                .Include(c => c.User)
                .Include(c => c.Category)
                .Include(c=> c.Images)
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
                    Images = c.Images.Select(img => new ImageDto
                    {
                        Id = img.Id,
                        PublicId = img.PublicId,
                        Url = img.Url
                    }).ToList()
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

        
        public async Task<string> Add(ProductDto dto,string secret)
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

                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
            
                
                    response = "0";
                    if (dto.Files!=null && dto.Files.Count>0)
                    {
                   
                        foreach (var item in dto.Files)
                        {

                            var stream = item.OpenReadStream();

                            PictureDto data = new PictureDto();
                            // save new image for product on Cloudinary
                             data = await UploadPictureAsync(stream, "MVCEcommerce", dto.Name);
                      
                             image newImage = new();
                             newImage.ProductId = product.Id;
                             newImage.PublicId = data.PublicId;
                             newImage.Url = data.Url;
                     
                        //save image data on database
                        await _context.Images.AddAsync(newImage);
                        await _context.SaveChangesAsync();

                        //remove  actual image for product
                        //  await DeleteImageAsync(dto.PublicId);




                    }//end of foreach

                 

                }
                response = "1";
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
               .Include(c=>c.Images)
               .Where(c => c.Id == id)
               .Select(c => new ProductResultDto
               {
                   Id = c.Id, 
                   Name = c.Name,
                   Description = c.Description,
                   Price = c.Price,
                   Stock = c.Stock,
                   CreationModificationDate = c.CreationModificationDate,
                   CategoryId= c.Category.Id,
                   CategoryName = c.Category.Title,
                   UserName = c.User.UserName,
                   Images = c.Images.Select(img => new ImageDto
                   {
                       Id = img.Id,
                       PublicId = img.PublicId,
                       Url = img.Url
                   }).ToList()
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
                var exist = await _context.Products
                    .FindAsync(id);
                if (exist != null)
                { var ProductId = exist.Id;
                   
                    //remove product on database
                    _context.Products.Remove(exist);
                    await _context.SaveChangesAsync();
                    var images = await _context.Images
                        .Where(c=>c.ProductId==ProductId)
                        .ToListAsync();
                    if (images !=null)
                    {
                        foreach (var item in images)
                        {   //delete from cloudinary
                            var state = DeleteImageAsync(item.PublicId);
                            //delete on database
                            _context.Images.Remove(item);
                            await _context.SaveChangesAsync();
                        }
                    }
                   
                    return "1";
                }
                return "Product Not Found!";
            }
            catch (Exception e)
            {
                return e.Message;
               
            }
        }
    }
}
