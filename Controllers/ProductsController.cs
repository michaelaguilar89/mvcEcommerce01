using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop.Infrastructure;
using Mono.TextTemplating;
using MVCEcommerce.Data;
using MVCEcommerce.Dto_s;
using MVCEcommerce.Models;
using MVCEcommerce.Services;

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
        public ProductsController(ProductService product,
            UserManager<IdentityUser> userManager,
            CategoryService category)
        {
            _product = product;
            _category = category;
            
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

        public async Task<IActionResult> AddOrEdit(int? id)
        {
            //get categories form database
            var categories = await _category.GetCategories();

            if (id==null ||id==0)
            {
                // return categorie's view
                ViewBag.Categories = new SelectList(categories, "Id", "Title");
                return View(new ProductDto());
            }
            else
            {
                var exist = await _product.getProductById(id);
                if (exist!=null)
                {
                    ProductDto dto = new ProductDto();
                    dto.Id=exist.Id;
                    dto.Name=exist.Name;
                    dto.Price=exist.Price;
                    dto.Stock=exist.Stock;
                    dto.CategoryId=exist.CategoryId;
                    dto.Url=exist.Url;
                    dto.PublicId = exist.PublicId;

                    // return categorie's view
                    ViewBag.Categories = new SelectList(categories, "Id", "Title", dto.CategoryId);
                    return View(dto);
                }
                ViewBag.ErrorMessage = "Error : Product not Found!";
                //return View();
            }

            return View(new ProductDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(ProductDto dto)
        {
            if (ModelState.IsValid)
            {
                var secret = GetCurrentUserId();
                var state=await _product.AddOrEdit(dto,secret);
                if (state=="1" || state=="2")
                {
                    return RedirectToAction("Index");

                }
                //get categories form database
                var categories = await _category.GetCategories();
                // return categorie's view
                ViewBag.Categories = new SelectList(categories, "Id", "Title");
                ViewBag.ErrorMessage = state;
                return View(dto);
            }
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
