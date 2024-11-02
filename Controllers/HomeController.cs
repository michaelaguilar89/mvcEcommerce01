using Microsoft.AspNetCore.Mvc;
using MVCEcommerce.Dto_s;
using MVCEcommerce.Models;
using MVCEcommerce.Services;

using System.Diagnostics;
using System.Net.Mail;
using System.Text;

namespace MVCEcommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HomeService _homeService;
        private readonly EmailService _emailService;
        public HomeController(ILogger<HomeController> logger,
                                HomeService homeService,
                                EmailService emailService)
        {
            _logger = logger;
            _homeService = homeService;
            _emailService = emailService;
            _emailService = emailService;

        }

        public async Task< IActionResult> Index(int pageNumber = 1, int pageSize = 5, string searchQuery = "")
        {
            HomeViewDto dto = new HomeViewDto();
            if (!string.IsNullOrEmpty(searchQuery))
            {
                dto.List = await _homeService.GetProductsWithPaginationAndSearch(searchQuery, pageNumber , pageSize );
                
            }
            else
            {
                dto.List = await _homeService.GetProductsWithPagination(pageNumber, pageSize);

            }
            dto.TotalProducts = dto.List.Count;
            dto.Count=dto.List.Count;
            dto.PageSize = 5;
            dto.PageNumber = pageNumber;
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitOrderEmail(CartViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("CartView", model);
            }

            string emailBody = GenerateEmailBody(model);
            EmailDto email = new EmailDto();
            email.Para = model.Email;
            email.Asunto = "Online Order by " + model.Name;
            email.Contenido = emailBody;
            await _emailService.SendEmail(email);
             

            return RedirectToAction("Index");
        }

        // Método para construir el cuerpo del email

        private string GenerateEmailBody(CartViewModel model)
        {
            StringBuilder emailBody = new StringBuilder();

            // Encabezado del correo
            emailBody.AppendLine("<h2>Online Shopping INC</h2>");
            emailBody.AppendLine("<p>Thank you for your order!</p>");
            emailBody.AppendLine("<p>Order Details:</p>");

            // Tabla de productos
            emailBody.AppendLine("<table style='width: 100%; border-collapse: collapse;'>");
            emailBody.AppendLine("<tr style='background-color: #f2f2f2;'>");
            emailBody.AppendLine("<th style='padding: 8px; border: 1px solid #ddd;'>Product</th>");
            emailBody.AppendLine("<th style='padding: 8px; border: 1px solid #ddd;'>Quantity</th>");
            emailBody.AppendLine("<th style='padding: 8px; border: 1px solid #ddd;'>Price</th>");
            emailBody.AppendLine("<th style='padding: 8px; border: 1px solid #ddd;'>Subtotal</th>");
            emailBody.AppendLine("</tr>");

            // Filas de productos
            foreach (var item in model.Items)
            {
                emailBody.AppendLine("<tr>");
                emailBody.AppendLine($"<td style='padding: 8px; border: 1px solid #ddd;'>{item.Name}</td>");
                emailBody.AppendLine($"<td style='padding: 8px; border: 1px solid #ddd; text-align: center;'>{item.Quantity}</td>");
                emailBody.AppendLine($"<td style='padding: 8px; border: 1px solid #ddd;'>${item.Price:0.00}</td>");
                emailBody.AppendLine($"<td style='padding: 8px; border: 1px solid #ddd;'>${item.Price * item.Quantity:0.00}</td>");
                emailBody.AppendLine("</tr>");
            }

            // Total
            emailBody.AppendLine("<tr style='background-color: #f2f2f2; font-weight: bold;'>");
            emailBody.AppendLine("<td colspan='3' style='padding: 8px; border: 1px solid #ddd; text-align: right;'>Total:</td>");
            emailBody.AppendLine($"<td style='padding: 8px; border: 1px solid #ddd;'>${model.Items.Sum(i => i.Price * i.Quantity):0.00}</td>");
            emailBody.AppendLine("</tr>");
            emailBody.AppendLine("</table>");

            // Información del cliente
            emailBody.AppendLine("<br />");
            emailBody.AppendLine("<p><strong>Shipping Address:</strong> " + model.Address + "</p>");
            emailBody.AppendLine("<p>---------------------------</p>");
            emailBody.AppendLine("<p><strong>Client:</strong> " + model.Name + "</p>");
            emailBody.AppendLine("<p><strong>Email:</strong> " + model.Email + "</p>");
            emailBody.AppendLine("<p><strong>Phone Number:</strong> " + model.PhoneNumber + "</p>");
            emailBody.AppendLine("<p>---------------------------</p>");

            return emailBody.ToString();
        }


      
        public IActionResult CartView()
        {
            return View();
        }

        private string GenerateContactEmailBody(ContactDto model)
        {
            StringBuilder emailBody = new StringBuilder();

            // Encabezado del correo
            emailBody.AppendLine("<h2>Demo MVC - Ecommerce</h2>");
            emailBody.AppendLine("<p></p>");
            emailBody.AppendLine("<p>Contact Email</p>");

            // Información del cliente
            emailBody.AppendLine("<br />");
            emailBody.AppendLine("<p>---------------------------</p>");
            emailBody.AppendLine("<p><strong>Client:</strong> " + model.Name + "</p>");
            emailBody.AppendLine("<p><strong>Email:</strong> " + model.Email + "</p>");
            emailBody.AppendLine("<p><strong>Address:</strong> " + model.Address + "</p>");
            emailBody.AppendLine("<p><strong>Phone Number:</strong> " + model.PhoneNumber + "</p>");
            emailBody.AppendLine("<p><strong>Messages:</strong> " + model.Message + "</p>");
            emailBody.AppendLine("<p>---------------------------</p>");

            return emailBody.ToString();
        }
        [HttpPost]
        public async Task< IActionResult> Contact(ContactDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(dto);
                }

                string emailBody = GenerateContactEmailBody(dto);
                EmailDto email = new EmailDto();
                email.Para = dto.Email;
                email.Asunto = "Contact Form - MVC ecommerce Demo by : " + dto.Name;
                email.Contenido = emailBody;
                await _emailService.SendEmail(email);


                return RedirectToAction("Index");
            }
            catch (Exception e)
            {

                Console.WriteLine("Error : "+e.Message);
                return View(dto);
            }
          
        }

        public IActionResult About()
        {
            return View();
        }

        public async Task<IActionResult> Contact()
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
