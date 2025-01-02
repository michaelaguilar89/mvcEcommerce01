using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop.Infrastructure;
using Mono.TextTemplating;
using MVCEcommerce.Data;
using MVCEcommerce.Dto_s;
using MVCEcommerce.Models;
using MVCEcommerce.Services;
using System.Reflection;

namespace MVCEcommerce.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ProductService _product;
        private readonly CategoryService _category;
        private int _pageNumber = 1;
        private int _pageSize = 5;
        public int totalProducts = 0;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ProductsController> _logger;
        public ProductsController(ProductService product,
            UserManager<IdentityUser> userManager,
            CategoryService category, ILogger<ProductsController> logger)
        {
            _product = product;
            _category = category;
            _logger = logger;

            _userManager = userManager;
        }
        // Método para obtener el UserId
        private string GetCurrentUserId()
        {
            return _userManager.GetUserId(User);
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize =5, string searchQuery="")
        {
           List<ProductResultDto> products = new List<ProductResultDto>();
            if (!string.IsNullOrEmpty(searchQuery))
            {
                products = await _product.GetProductsWithPaginationAndSearch(pageNumber, pageSize, searchQuery);

            }
            else
            {
                 products = await _product.GetProductsWithPagination(pageNumber, pageSize);

            }

            if (products!=null && products.Count>0)
            {
                totalProducts = products.Count;
            }
            var viewModel = new ProductListViewModel
            {
                Products = products,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalProducts = totalProducts,
                SearchQuery = searchQuery
            };
            Console.WriteLine(  );

            return View(viewModel);
        }

        
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            ProductForUpdatesDto dto = new ProductForUpdatesDto();
            try
            {
                
                if (id!=null || id>0)
                {
                   
                    var item = await _product.getProductById(id);
                    dto.Id=item.Id;
                    dto.Name = item.Name;
                    dto.Description = item.Description;
                    dto.Price=item.Price;
                    dto.Stock=item.Stock;
                    dto.CreationDate = item.CreationModificationDate;
                    dto.CategoryId=item.CategoryId;
                    Console.WriteLine("Id : " + dto.Id);
                    Console.WriteLine("Name : " + dto.Name);
                    Console.WriteLine("Price : " + dto.Price);
                    Console.WriteLine("Stock : " + dto.Stock);
                    Console.WriteLine("Category Id : " + dto.CategoryId);
                    Console.WriteLine("Date : " + dto.CreationDate);
                    var categories = await _category.GetCategories();
                    dto.Categories = new List<CategoriesDto>();
                    foreach (var i in categories)
                    {
                        CategoriesDto categoryDto = new CategoriesDto();
                        categoryDto.Id = i.Id;
                        categoryDto.Title = i.Title;
                        
                        dto.Categories.Add(categoryDto);
                    }
                    dto.Images = new List<ImageForUpdateDto>() ;
                    foreach (var e in item.Images)
                    {
                        ImageForUpdateDto img = new ImageForUpdateDto();
                        img.Id= e.Id;
                        img.PublicId=e.PublicId;
                        img.Url=e.Url;
                        img.Remove = false;
                        dto.Images.Add(img);
                    }
                    dto.Files = new List<IFormFile> ();
                    if (dto.Images != null)
                    {
                        Console.WriteLine("Images -Count: " + dto.Images.Count);
                        foreach (var e in dto.Images)
                        {
                            Console.WriteLine(" Id : " + e.Id + " ,Url : " + e.Url + " PublicId : " + e.PublicId + " , Remove : " + e.Remove);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Images is null!");
                    }
                    return View(dto);
                }
                ViewBag.Error = "Product Not Found!";
                return View(dto);
            }
            catch (Exception e)
            {
                ViewBag.Error = "Error : " + e.Message;
                return View(dto);
                
            }
            
        }
        [HttpPost]
        public async Task<IActionResult> Edit(ProductForUpdatesDto dto)
        {
            Console.WriteLine("Id : "+dto.Id);
            Console.WriteLine("Name : " + dto.Name);
            Console.WriteLine("Price : " + dto.Price);
            Console.WriteLine("Stock : " + dto.Stock);
            Console.WriteLine("Category Id : " + dto.CategoryId);
            Console.WriteLine("Date : " + dto.CreationDate);
            
            if (dto.Images!=null)
            {
                Console.WriteLine("Images -Count: " + dto.Images.Count);
                foreach (var e in dto.Images)
                {
                    Console.WriteLine(" Id : " + e.Id + " ,Url : " + e.Url + " PublicId : " + e.PublicId+" , Remove : "+e.Remove);
                }
            }
            else
            {
                Console.WriteLine("Images is null!");
            }

            if (dto.Files!=null )
            {
                Console.WriteLine("Files - Count : " + dto.Files.Count);

            }
            else
            {
                Console.WriteLine("Files is null!");
            }
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Models is Invalid");
            }
            var secret = GetCurrentUserId();

            var state = await _product.Edit(dto, secret);
            if (state.Equals("1"))
            {
                return RedirectToAction("Index");
            }
            ViewBag.Error = state;
           
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            //get categories form database
            var categories = await _category.GetCategories();
            Console.WriteLine(  "Categories from controller : ");
            foreach (var item in categories)
            {
                Console.WriteLine( "Id : "+item.Id+ " , name : "+item.Title);
            }
            int counter = 0;
           
            ProductDto dto = new ProductDto();
            dto.Categories = new List<CategoriesDto>();

            foreach (var item in categories)
            {
                CategoriesDto categoryDto = new CategoriesDto();
                categoryDto.Id=item.Id;
                categoryDto.Title = item.Title;
                
                
                if (counter==0)
                {
                    categoryDto.IsSelected = true;
                    counter++;
                }
               
                dto.Categories.Add(categoryDto);

            }
                return View(dto);
          
           
        }

         

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ProductDto dto)
        {

            Console.WriteLine( "data , Id : "+dto.Id+" ,Name :"+dto.Name+" ,Description : "
                +dto.Description+" , Price : "+dto.Stock+" , Categorie-Id : "+dto.CategoryId+" ,Images -Count : "+dto.Files.Count);
            if (!ModelState.IsValid)
            {
                //get categories form database
                var categories = await _category.GetCategories();
                // return categorie's view
                // Registrar error específico
              //  _logger.LogWarning("Product operation returned a state: {State}", state);

                ViewBag.Categories = categories;
                
                return View(dto);
            }


            var secret = GetCurrentUserId();
            var state = await _product.Add(dto, secret);
            if (state == "1")
            {
                return RedirectToAction("Index");

            }
            //get categories form database
            var categories2 = await _category.GetCategories();
            ViewBag.Categories = categories2;
            ViewBag.Error = state;
            return View(dto);
        }

        public async Task<IActionResult> Delete(int? id)
        {
           var product = await _product.getProductById(id);
            if (product !=null)
            {
                return View(product);
            }
            ViewBag.ErrorMessage = "Product Not Found!";
            return View();
        }
        [HttpPost,ActionName("DeleteConfirmed")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id>0 )
            {
                
                var state = await _product.Delete(id);
                if (state=="1")
                {
                    return RedirectToAction("Index");

                }
                ViewBag.ErrorMessage = state;
                return View();
            }
            ViewBag.ErrorMessage = "Error : Product Not Found!";
            return View();
        }
    }
}
