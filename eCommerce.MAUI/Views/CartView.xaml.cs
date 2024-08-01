using ShoppingApp.Library.Models;
using ShoppingApp.Library.Services;

namespace eCommerce.MAUI.Views;

public partial class CartView : ContentPage
{
	public CartView()
	{
		InitializeComponent();
	}

    private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
		BindingContext = new ShoppingCart();
    }

    private void OkClicked(object sender, EventArgs e)
    {
        var cart = BindingContext as ShoppingCart;
        if (cart != null)
        {
            ShoppingCartServiceProxy.Current.AddCart(cart);
            Shell.Current.GoToAsync("//Shop");
        }
        else
        {

        }
    }


    private void CancelClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//Shop");
    }
}