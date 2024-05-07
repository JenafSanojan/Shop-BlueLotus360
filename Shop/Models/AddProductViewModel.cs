using System.ComponentModel.DataAnnotations;

namespace Shop.Models
{
    public class AddProductViewModel
    {
        public int Id { get; set; } // Product ID (Normally bar code reads to int)

        [StringLength(100)]
        public string Name { get; set; }

        public string Category { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number")]
        public double Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a positive number")]
        public int Quantity { get; set; }

        [StringLength(50)]
        public string SKU { get; set; } // Stock keeping unit

        public string Description { get; set; }
    }
}
