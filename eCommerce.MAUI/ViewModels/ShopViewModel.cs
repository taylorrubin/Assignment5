using eCommerce.Library.DTO;
using ShoppingApp.Library.Models;
using ShoppingApp.Library.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace eCommerce.MAUI.ViewModels
{
    public class ShopViewModel : INotifyPropertyChanged
    {
        public ShopViewModel()
        {
            InventoryQuery = string.Empty;
            SelectedCart = Carts.FirstOrDefault();
            TaxAmount = 0; // Initialize tax amount to zero
        }

        private string? inventoryQuery;
        public string InventoryQuery
        {
            set
            {
                inventoryQuery = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Products));
                NotifyPropertyChanged(nameof(ProductsInCart));
                NotifyPropertyChanged(nameof(TotalPriceWithTax)); // Update total price with tax
            }
            get { return inventoryQuery; }
        }

        private decimal _taxAmount;
        public decimal TaxAmount
        {
            get => _taxAmount;
            set
            {
                if (_taxAmount != value)
                {
                    _taxAmount = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(TotalPriceWithTax)); // Update total price with tax
                }
            }
        }

        public List<ProductViewModel> Products
        {
            get
            {
                return InventoryServiceProxy.Current.Products
                    .Where(p => p != null)
                    .Where(p => p.Quantity > 0)
                    .Where(p => p?.Name?.ToUpper()?.Contains(InventoryQuery.ToUpper()) ?? false)
                    .Select(p => new ProductViewModel(p)).ToList()
                    ?? new List<ProductViewModel>();
            }
        }

        private ShoppingCart? selectedCart;
        public ShoppingCart? SelectedCart
        {
            get
            {
                return selectedCart;
            }

            set
            {
                selectedCart = value;
                NotifyPropertyChanged(nameof(ProductsInCart));
                NotifyPropertyChanged(nameof(TotalPrice));
                NotifyPropertyChanged(nameof(TotalPriceWithTax));
            }
        }

        public ObservableCollection<ShoppingCart> Carts
        {
            get
            {
                var carts = ShoppingCartServiceProxy.Current.Carts;
                // Check for null and convert ReadOnlyCollection to List
                List<ShoppingCart> cartList = carts?.ToList() ?? new List<ShoppingCart>();
                return new ObservableCollection<ShoppingCart>(cartList);
            }
        }

        public List<ProductViewModel> ProductsInCart
        {
            get
            {
                return SelectedCart?.Contents?.Where(p => p != null)
                    .Where(p => p?.Name?.ToUpper()?.Contains(InventoryQuery.ToUpper()) ?? false)
                    .Select(p => new ProductViewModel(p)).ToList()
                    ?? new List<ProductViewModel>();
            }
        }

        private ProductViewModel? productToBuy;
        public ProductViewModel? ProductToBuy
        {
            get => productToBuy;
            set
            {
                productToBuy = value;

                if (productToBuy != null && productToBuy.Model == null)
                {
                    productToBuy.Model = new ProductDTO();
                }
                else if (productToBuy != null && productToBuy.Model != null)
                {
                    productToBuy.Model = new ProductDTO(productToBuy.Model);
                }
            }
        }

        public decimal TotalPrice
        {
            get
            {
                decimal totalPrice = 0;
                foreach (var productViewModel in ProductsInCart ?? new List<ProductViewModel>())
                {
                    if (productViewModel.Model != null)
                    {
                        int quantityToCharge = productViewModel.Model.Quantity;

                        // Apply BOGO logic
                        if (productViewModel.Model.IsBuyOneGetOneFree)
                        {
                            quantityToCharge = (productViewModel.Model.Quantity / 2) + (productViewModel.Model.Quantity % 2);
                        }

                        totalPrice += productViewModel.DiscountedPrice * quantityToCharge;
                    }
                }
                return totalPrice;
            }
        }

        public decimal TotalPriceWithTax
        {
            get
            {
                // Calculate total price including tax
                decimal totalPrice = TotalPrice;
                return totalPrice * (1 + TaxAmount / 100); // Assuming tax is a percentage
            }
        }

        public void Refresh()
        {
            InventoryQuery = string.Empty;
            NotifyPropertyChanged(nameof(Products));
            NotifyPropertyChanged(nameof(Carts));
            NotifyPropertyChanged(nameof(TotalPriceWithTax));
        }

        public void Search()
        {
            NotifyPropertyChanged(nameof(Products));
        }

        public void PlaceInCart()
        {
            if (ProductToBuy?.Model == null || SelectedCart == null)
            {
                return;
            }

            // Ensure ProductToBuy has a valid model instance
            if (ProductToBuy.Model == null)
            {
                ProductToBuy.Model = new ProductDTO();
            }

            // Set quantity to 1 (or any other desired value)
            ProductToBuy.Model.Quantity = 1;

            // Add the product to the selected cart
            ShoppingCartServiceProxy.Current.AddToCart(ProductToBuy.Model, SelectedCart.Id);

            // Clear the selected product after adding to cart
            ProductToBuy = null;

            // Notify UI updates
            NotifyPropertyChanged(nameof(ProductsInCart));
            NotifyPropertyChanged(nameof(Products));
            NotifyPropertyChanged(nameof(TotalPriceWithTax));
        }

        public void ApplyTax(decimal taxRate)
        {
            // Set TaxAmount based on the provided tax rate
            TaxAmount = taxRate;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
