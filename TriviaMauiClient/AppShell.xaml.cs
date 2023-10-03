using TriviaMauiClient.Views;

namespace TriviaMauiClient;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute("Game", typeof(GamePage));
	}

  

}
