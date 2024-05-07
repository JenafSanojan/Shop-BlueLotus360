using System.ComponentModel.DataAnnotations;

namespace Shop.Models.Entities
{
    public class Product
    {

        public int Id { get; set; } // auto number

        [Required(ErrorMessage = "iD is required")] // Applies required attribute to the name property
        public long ProductId { get; set; } // Product ID (Normally bar code reads to int)

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

        public string Description { get; set; }
    }
}
