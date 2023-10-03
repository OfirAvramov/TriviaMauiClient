using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using TriviaMauiClient.Models;
using TriviaMauiClient.Services;

namespace TriviaMauiClient.ViewModels
{

    public class GamePageViewModel : ViewModel
    {
       

        private TriviaService _gameService;
        private string message;
        public string Message { get => message; set { message = value; OnPropertyChange(); } }

        private string foundEmail;

        public string FoundEmail { get => foundEmail; set { foundEmail = value; OnPropertyChange(); } }
        public ICommand SearchCommand { get; protected set; }


        public GamePageViewModel(TriviaService gameService)
        {
            _gameService = gameService;
            var u=SecureStorage.Default.GetAsync("LoggedUser").Result;
            var user= JsonSerializer.Deserialize<User>(u);
            Message = $"Hello {user.NickName}";
            SearchCommand = new Command(async (x) => FoundEmail = await _gameService.GetUserEmail((string)x));
          
        }
    }
}
