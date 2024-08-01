using eCommerce.Library.DTO;
using System;

namespace ShoppingApp.Library.Models
{
    public class Product
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Id { get; set; }
        public int Quantity { get; set; }
        public bool IsBuyOneGetOneFree { get; set; } // BOGO flag
        public decimal MarkdownPercentage { get; set; } // New property for markdown percentage

        public string? Property1 { get; set; }
        public string? Property2 { get; set; }
        public string? Property3 { get; set; }
        public string? Property4 { get; set; }
        public string? Property5 { get; set; }
        public string? Property6 { get; set; }

        public Product()
        {
            MarkdownPercentage = 0; // Default to no markdown
        }

        public Product(Product p)
        {
            Name = p.Name;
            Description = p.Description;
            Price = p.Price;
            Id = p.Id;
            Quantity = p.Quantity;
            IsBuyOneGetOneFree = p.IsBuyOneGetOneFree; // Copy buy-one-get-one-free status
            MarkdownPercentage = p.MarkdownPercentage; // Copy markdown percentage
        }

        public Product(ProductDTO d)
        {
            Name = d.Name;
            Description = d.Description;
            Price = d.Price;
            Id = d.Id;
            Quantity = d.Quantity;
            IsBuyOneGetOneFree = d.IsBuyOneGetOneFree; // Copy buy-one-get-one-free status
            MarkdownPercentage = d.MarkdownPercentage; // Copy markdown percentage
        }

        // Adjusted property to reflect markdown
        public decimal DiscountedPrice
        {
            get
            {
                decimal markdownAmount = Price * (MarkdownPercentage / 100);
                return Price - markdownAmount;
            }
        }
    }
}
