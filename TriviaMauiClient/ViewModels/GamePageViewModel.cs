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

        #region Fields
        private string message;
        private string foundEmail;
        #endregion

        #region Service
        readonly private TriviaService _gameService;
        #endregion

        #region Properties
        public string Message { get => message; set { message = value; OnPropertyChange(); } }
        public string FoundEmail { get => foundEmail; set { foundEmail = value; OnPropertyChange(); } }
        #endregion

        #region Commands
        public ICommand SearchCommand { get; protected set; }
        #endregion


        /// <summary>
        /// c'tor
        /// </summary>
        /// <param name="gameService"></param>
        public GamePageViewModel(TriviaService gameService)
        {
            _gameService = gameService;
            var u=SecureStorage.Default.GetAsync("LoggedUser").Result;
            var user= JsonSerializer.Deserialize<User>(u);
            Message = $"Hello {user.NickName}";
            //מוצאת אימייל של יוזר לפי השם שלו
            SearchCommand = new Command<string>( async(x) => FoundEmail = await _gameService.GetUserEmail(x));
          
        }
    }
}
