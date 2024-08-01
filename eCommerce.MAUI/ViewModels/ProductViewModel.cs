using ShoppingApp.Library.Models;
using ShoppingApp.Library.Services;
using eCommerce.Library.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.MAUI.ViewModels
{
    public class ProductViewModel
    {
        public override string ToString()
        {
            if (Model == null)
            {
                return string.Empty;
            }
            return $"{Model.Id} - {Model.Name} - {Model.Price:C}";
        }

        public ProductDTO? Model { get; set; }

        public string DisplayPrice
        {
            get
            {
                if (Model == null) { return string.Empty; }
                return $"{Model.Price:C}";
            }
        }

        public string PriceAsString
        {
            set
            {
                if (Model == null)
                {
                    return;
                }
                if (decimal.TryParse(value, out var price))
                {
                    Model.Price = price;
                }
                else
                {
                    
                }
            }
        }

        public string OriginalPriceDisplay
        {
            get
            {
                if (Model == null) { return string.Empty; }
                return $"{Model.Price:C}";
            }
        }

        public bool IsBuyOneGetOneFree
        {
            get { return Model?.IsBuyOneGetOneFree ?? false; }
            set
            {
                if (Model != null)
                {
                    Model.IsBuyOneGetOneFree = value;
                }
            }
        }

        public string BOGOStatus
        {
            get { return IsBuyOneGetOneFree ? "BOGO" : string.Empty; }
        }

        public decimal MarkdownPercentage
        {
            get { return Model?.MarkdownPercentage ?? 0; }
            set
            {
                if (Model != null)
                {
                    Model.MarkdownPercentage = value;
                }
            }
        }

        public decimal DiscountedPrice
        {
            get
            {
                if (Model == null)
                {
                    return 0;
                }

                if (MarkdownPercentage > 0)
                {
                    decimal markdownAmount = Model.Price * (MarkdownPercentage / 100);
                    return Model.Price - markdownAmount;
                }
                else
                {
                    return Model.Price;
                }
            }
        }

        public string DiscountedPriceDisplay
        {
            get
            {
                return $"{DiscountedPrice:C}";
            }
        }

        public ProductViewModel(int productId = 0)
        {
            if (productId == 0)
            {
                Model = new ProductDTO();
            }
            else
            {
                Model = InventoryServiceProxy
                    .Current
                    .Products.FirstOrDefault(p => p.Id == productId)
                    ?? new ProductDTO();
            }
        }

        public ProductViewModel(ProductDTO? model)
        {
            Model = model ?? new ProductDTO();
        }

        public async void Add()
        {
            if (Model != null)
            {
                Model = await InventoryServiceProxy.Current.AddOrUpdate(Model);
            }
        }
    }
}
