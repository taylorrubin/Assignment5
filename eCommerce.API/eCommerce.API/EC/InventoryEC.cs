using ShoppingApp.Library.Models;
using eCommerce.API.Database;
using eCommerce.Library.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerce.API.EC
{
    public class InventoryEC
    {
        public InventoryEC() { }

        public async Task<IEnumerable<ProductDTO>> Get()
        {
            // Retrieve products from Filebase and convert to ProductDTO
            var products = Filebase.Current.Products.Take(100).Select(p => new ProductDTO(p));
            return await Task.FromResult(products);
        }

        public async Task<IEnumerable<ProductDTO>> Search(string? query)
        {
            // Search products from Filebase based on query and convert to ProductDTO
            var products = Filebase.Current.Products
                .Where(p =>
                    (p?.Name != null && p.Name.ToUpper().Contains(query?.ToUpper() ?? string.Empty))
                    ||
                    (p?.Description != null && p.Description.ToUpper().Contains(query?.ToUpper() ?? string.Empty)))
                .Take(100)
                .Select(p => new ProductDTO(p));

            return await Task.FromResult(products);
        }

        public async Task<ProductDTO?> Delete(int id)
        {
            // Delete product from Filebase and return as ProductDTO
            var deletedProduct = Filebase.Current.Delete(id);
            return await Task.FromResult(new ProductDTO(deletedProduct));
        }

        public async Task<ProductDTO> AddOrUpdate(ProductDTO p)
        {
            // Add or update product in Filebase and return as ProductDTO
            var product = new Product(p);
            var updatedProduct = Filebase.Current.AddOrUpdate(product);
            return await Task.FromResult(new ProductDTO(updatedProduct));
        }
    }
}
