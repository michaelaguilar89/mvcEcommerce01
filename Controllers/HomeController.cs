using Microsoft.AspNetCore.Mvc;
using MVCEcommerce.Dto_s;
using MVCEcommerce.Models;
using MVCEcommerce.Services;
using System.Diagnostics;

namespace MVCEcommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HomeService _homeService;
        public HomeController(ILogger<HomeController> logger,
                                HomeService homeService)
        {
            _logger = logger;
            _homeService = homeService;
        }

        public async Task< IActionResult> Index(string searchQuery = "")
        {
            HomeViewDto dto = new HomeViewDto();
            if (!string.IsNullOrEmpty(searchQuery))
            {
                dto.List = await _homeService.GetProductsWithPaginationAndSearch(searchQuery);
            }
            else
            {
                dto.List = await _homeService.GetProductsWithPagination();

            }
            dto.Count=dto.List.Count;
           
            return View(dto);
        }

        [HttpPost]
        public IActionResult SubmitOrder(CartViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("CartView", model); // Volver a la vista si hay errores
            }

            // Aquí puedes procesar la orden, guardar en la base de datos o enviar un correo de confirmación
            // ...

            // Vaciar el carrito (si es necesario)
            TempData["SuccessMessage"] = "Your order has been submitted successfully!";
            return RedirectToAction("OrderConfirmation");
        }

        public IActionResult OrderConfirmation()
        {
            ViewBag.Message = TempData["SuccessMessage"];
            return View();
        }
        public IActionResult CartView()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
