using eCommerce.Library.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingApp.Library.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public List<ProductDTO>? Contents { get; set; }
        public string Name { get; set; } = string.Empty;

        public ShoppingCart()
        {
            Contents = new List<ProductDTO>();
        }

        public override string ToString()
        {
            return Name;
        }

        public decimal CalculateTotalPrice()
        {
            if (Contents == null || Contents.Count == 0)
                return 0;

            decimal totalPrice = Contents.Sum(p => p.Price * p.Quantity);
            return totalPrice;
        }
    }
}
