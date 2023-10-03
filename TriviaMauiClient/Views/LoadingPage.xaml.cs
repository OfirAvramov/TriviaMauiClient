using TriviaMauiClient.ViewModels;

namespace TriviaMauiClient.Views;

public partial class LoadingPage : ContentPage
{
	public LoadingPage(LoadingPageViewModel vm)
	{
        this.BindingContext = vm;
		InitializeComponent();
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
        this.Opacity = 50;
        this.Window.MinimumHeight = 100;
        this.Window.MinimumWidth = 100;
    }
}