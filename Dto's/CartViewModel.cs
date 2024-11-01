using System.ComponentModel.DataAnnotations;

namespace MVCEcommerce.Dto_s
{
    public class CartViewModel
    {  // Información del Cliente
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        // Lista de Productos en el Carrito
        public List<CartItem> Items { get; set; } = new List<CartItem>();
    }

    public class CartItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal => Price * Quantity;
    }
}
