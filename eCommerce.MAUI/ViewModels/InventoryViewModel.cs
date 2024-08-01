using ShoppingApp.Library.Models;
using ShoppingApp.Library.Services;
using eCommerce.Library.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.MAUI.ViewModels
{
    public class InventoryViewModel : INotifyPropertyChanged
    {
        private List<ProductViewModel> _products;

        public string? Query { get; set; }

        public List<ProductViewModel> Products
        {
            get => _products;
            private set
            {
                if (_products != value)
                {
                    _products = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ProductViewModel? SelectedProduct { get; set; }

        public InventoryViewModel()
        {
            _products = new List<ProductViewModel>();
        }

        public void Edit()
        {
            Shell.Current.GoToAsync($"//Product?productId={SelectedProduct?.Model?.Id ?? 0}");
        }

        public async void Delete()
        {
            if (SelectedProduct?.Model?.Id != null)
            {
                await InventoryServiceProxy.Current.Delete(SelectedProduct.Model.Id);
                await Refresh();
            }
        }

        public async Task Refresh()
        {
            await Search();
        }

        public async Task Search()
        {
            var searchResults = await InventoryServiceProxy.Current.Search(new Query(Query ?? string.Empty));
            Products = searchResults?.Where(p => p != null)
                                     .Select(p => new ProductViewModel(p))
                                     .ToList()
                                     ?? new List<ProductViewModel>();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
