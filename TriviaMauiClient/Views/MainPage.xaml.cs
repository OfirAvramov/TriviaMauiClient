
using TriviaMauiClient.ViewModels;

namespace TriviaMauiClient.Views;

public partial class MainPage : ContentPage
{
	
	public MainPage(MainPageViewModel vm)
	{
		this.BindingContext = vm;	
		InitializeComponent();
	}

	
}

