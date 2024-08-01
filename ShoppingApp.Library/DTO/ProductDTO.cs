using ShoppingApp.Library.Models;
using System;

namespace eCommerce.Library.DTO
{
    public class ProductDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Id { get; set; }
        public int Quantity { get; set; }
        public bool IsBuyOneGetOneFree { get; set; } // BOGO flag
        public decimal MarkdownPercentage { get; set; } // New property for markdown percentage

        public ProductDTO(Product p)
        {

            Name = p.Name;
            Description = p.Description;
            Price = p.Price;
            Id = p.Id;
            Quantity = p.Quantity;
            IsBuyOneGetOneFree = p.IsBuyOneGetOneFree; // Copy buy-one-get-one-free status
            MarkdownPercentage = p.MarkdownPercentage; // Copy markdown percentage
        }

        public ProductDTO(ProductDTO p)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));

            Name = p.Name;
            Description = p.Description;
            Price = p.Price;
            Id = p.Id;
            Quantity = p.Quantity;
            IsBuyOneGetOneFree = p.IsBuyOneGetOneFree; // Copy buy-one-get-one-free status
            MarkdownPercentage = p.MarkdownPercentage; // Copy markdown percentage
        }

        public ProductDTO() { }
    }
}
