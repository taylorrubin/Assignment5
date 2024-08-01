using ShoppingApp.Library.Utilities;
using eCommerce.Library.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingApp.Library.Services
{
    public class InventoryServiceProxy
    {
        private static InventoryServiceProxy? instance;
        private static object instanceLock = new object();

        private List<ProductDTO> products;

        public ReadOnlyCollection<ProductDTO> Products
        {
            get
            {
                return products.AsReadOnly();
            }
        }

        private int NextId
        {
            get
            {
                if (!products.Any())
                {
                    return 1;
                }
                return products.Select(p => p.Id).Max() + 1;
            }
        }

        private InventoryServiceProxy()
        {
            products = new List<ProductDTO>();
            InitializeProducts();
        }

        private void InitializeProducts()
        {
            try
            {
                var response = new WebRequestHandler().Get("/Inventory").Result;
                if (!string.IsNullOrEmpty(response))
                {
                    products = JsonConvert.DeserializeObject<List<ProductDTO>>(response) ?? new List<ProductDTO>();
                }
                else
                {
                    Console.WriteLine("Response from /Inventory is null or empty.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing products: {ex.Message}");
            }
        }

        public static InventoryServiceProxy Current
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new InventoryServiceProxy();
                    }
                }
                return instance!;
            }
        }

        public async Task<IEnumerable<ProductDTO>> Get()
        {
            await Task.Delay(100); // Simulate async delay
            return products;
        }

        public async Task<ProductDTO> AddOrUpdate(ProductDTO p)
        {
            await Task.Delay(100); // Simulate async delay

            var existingProduct = products.FirstOrDefault(prod => prod.Id == p.Id);

            if (existingProduct != null)
            {
                // Update existing product
                existingProduct.Name = p.Name;
                existingProduct.Price = p.Price;
                existingProduct.MarkdownPercentage = p.MarkdownPercentage;
            }
            else
            {
                // Add new product
                if (!products.Any())
                {
                    p.Id = 1; // If no products exist, start with ID 1
                }
                else
                {
                    int newId = products.Max(prod => prod.Id) + 1; // Generate a new unique ID
                    p.Id = newId;
                }
                products.Add(p);
            }

            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                };
                var result = await new WebRequestHandler().Post("/Inventory", p);
                var updatedProduct = JsonConvert.DeserializeObject<ProductDTO>(result, settings);

                if (updatedProduct != null)
                {
                    return updatedProduct;
                }
                else
                {
                    Console.WriteLine("Error deserializing the product data from response.");
                    return p;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding or updating product: {ex.Message}");
                return p;
            }
        }

        public async Task<ProductDTO?> Delete(int id)
        {
            await Task.Delay(100); // Simulate async delay

            var productToDelete = products.FirstOrDefault(p => p.Id == id);
            if (productToDelete != null)
            {
                try
                {
                    var result = await new WebRequestHandler().Delete($"/Inventory/{id}");
                    if (result != null)
                    {
                        products.Remove(productToDelete);
                        return productToDelete;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting product: {ex.Message}");
                }
            }
            return null;
        }

        public async Task<IEnumerable<ProductDTO>> Search(Query? query)
        {
            await Task.Delay(100); // Simulate async delay

            if (query == null || string.IsNullOrEmpty(query.QueryString))
            {
                return products;
            }

            // Simulate filtering products based on query
            return products.Where(p => p.Name.Contains(query.QueryString, StringComparison.OrdinalIgnoreCase));
        }
    }
}
