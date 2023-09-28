using System;
using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.WebAPI.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a non-negative value.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public string Category { get; set; }
    }
}
