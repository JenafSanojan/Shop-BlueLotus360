using System.ComponentModel.DataAnnotations;

namespace Shop.Models
{
    public class AddProductViewModel
    {
        [Required(ErrorMessage = "ID required")]
        public int Id { get; set; } // Product ID (Normally bar code reads to int)

        [Required(ErrorMessage = "Name is required")] // Applies required attribute to the name property
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a positive number")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "SKU is required")]
        [StringLength(50)]
        public string SKU { get; set; } // Stock keeping unit

        public string? Description { get; set; }

        // Nullable byte array or string for the image
        public IFormFile? Image { get; set; }
    }
}
