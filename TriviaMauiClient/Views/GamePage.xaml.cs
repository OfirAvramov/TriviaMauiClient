using TriviaMauiClient.ViewModels;
namespace TriviaMauiClient.Views;

public partial class GamePage : ContentPage
{
	public GamePage(GamePageViewModel vm)
	{
		this.BindingContext = vm;	
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		var user = SecureStorage.GetAsync("LoggedUser");
		if(user == null)
		{
			await AppShell.Current.GoToAsync("MainPage");
		}
	}
}

