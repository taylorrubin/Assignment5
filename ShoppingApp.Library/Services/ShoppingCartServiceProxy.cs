using eCommerce.Library.DTO;
using ShoppingApp.Library.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingApp.Library.Services
{
    public class ShoppingCartServiceProxy
    {
        private static ShoppingCartServiceProxy? instance;
        private static object instanceLock = new object();

        private List<ShoppingCart> carts;

        public ReadOnlyCollection<ShoppingCart> Carts
        {
            get
            {
                return carts.AsReadOnly();
            }
        }

        public ShoppingCart Cart
        {
            get
            {
                if (!carts.Any())
                {
                    var newCart = new ShoppingCart();
                    carts.Add(newCart);
                    return newCart;
                }
                return carts.FirstOrDefault() ?? new ShoppingCart();
            }
        }

        private ShoppingCartServiceProxy()
        {
            // Initialize with a default cart or fetch from a persistent source
            carts = new List<ShoppingCart>() { new ShoppingCart { Id = 1, Name = "Default" } };
        }

        public static ShoppingCartServiceProxy Current
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new ShoppingCartServiceProxy();
                    }
                }
                return instance!;
            }
        }

        public ShoppingCart AddCart(ShoppingCart cart)
        {
            if (cart.Id == 0)
            {
                cart.Id = carts.Select(c => c.Id).Max() + 1;
            }

            carts.Add(cart);

            return cart;
        }

        public void AddToCart(ProductDTO newProduct, int id)
        {
            var cartToUse = Carts.FirstOrDefault(c => c.Id == id);
            if (cartToUse == null || cartToUse.Contents == null)
            {
                return;
            }

            var existingProduct = cartToUse.Contents.FirstOrDefault(existingProducts => existingProducts.Id == newProduct.Id);
            var inventoryProduct = InventoryServiceProxy.Current.Products.FirstOrDefault(invProd => invProd.Id == newProduct.Id);

            if (inventoryProduct == null)
            {
                return;
            }

            inventoryProduct.Quantity -= newProduct.Quantity;

            if (existingProduct != null)
            {
                // Update quantity
                existingProduct.Quantity += newProduct.Quantity;
            }
            else
            {
                // Add new product to cart
                cartToUse.Contents.Add(newProduct);
            }
        }

        public void AddToCart(ProductDTO newProduct)
        {
            if (Cart == null || Cart.Contents == null)
            {
                return;
            }

            var existingProduct = Cart.Contents.FirstOrDefault(existingProducts => existingProducts.Id == newProduct.Id);
            var inventoryProduct = InventoryServiceProxy.Current.Products.FirstOrDefault(invProd => invProd.Id == newProduct.Id);

            if (inventoryProduct == null)
            {
                return;
            }

            inventoryProduct.Quantity -= newProduct.Quantity;

            if (existingProduct != null)
            {
                // Update quantity
                existingProduct.Quantity += newProduct.Quantity;
            }
            else
            {
                // Add new product to cart
                Cart.Contents.Add(newProduct);
            }
        }
    }
}
