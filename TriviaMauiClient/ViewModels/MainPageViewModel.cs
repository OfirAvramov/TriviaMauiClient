
using TriviaMauiClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TriviaMauiClient.Models;
using TriviaMauiClient.Views;
using System.Text.Json;

namespace TriviaMauiClient.ViewModels
{
    public class MainPageViewModel : ViewModel
    {

        #region Fields
        private string _userName;//שם משתמש
        private bool _showUserNameError;//האם להציג שדה שגיאת שם משתמש
        private string _userErrorMessage;//תאור שגיאת שם משתמש
        private string _password;//סיסמה
        
        private bool _showPasswordError;//האם להציג שגיאת סיסמה
        private string _passwordErrorMessage;
        private bool _showLoginError;//
        private string _loginErrorMessage;
        #endregion

        #region Service component
        private readonly TriviaService _service;
        #endregion

        #region Properties
        public string UserName
        { 
            get => _userName;
            set { if (_userName != value) { _userName = value; if (!ValidateUser()) { ShowUserNameError = true; UserErrorMessage = ErrorMessages.INVALID_USERNAME; } else { ShowUserNameError = true; UserErrorMessage = string.Empty; } OnPropertyChange(); OnPropertyChange(nameof(IsButtonEnabled)); } } }

        public bool ShowUserNameError
        {
            get => _showUserNameError; set
            {
                if (_showUserNameError != value)
                {
                    _showUserNameError = value; OnPropertyChange();
                }
            }
        }
        public string Password 
        {
            get => _password;
            set
            { 
                if (_password != value) 
                { 
                    _password = value; if (!ValidatePassWord())
                    {
                        ShowPasswordError = true;
                        PasswordErrorMessage = ErrorMessages.INVALID_PASSWORD;
                    }
                    else
                    {
                        PasswordErrorMessage = string.Empty;
                        ShowPasswordError = false;
                    } ;
                    OnPropertyChange();
                    OnPropertyChange(nameof(IsButtonEnabled));
                }
            }
        }

        public string UserErrorMessage { get => _userErrorMessage; set { if (_userErrorMessage != value) { _userErrorMessage = value; OnPropertyChange(); } } }


        public bool ShowPasswordError { get => _showPasswordError; set { if (_showPasswordError != value) { _showPasswordError = value; OnPropertyChange(); } } }
        public string PasswordErrorMessage { get => _passwordErrorMessage; set { if (_passwordErrorMessage != value) { _passwordErrorMessage = value; OnPropertyChange(); } } }

        public bool ShowLoginError { get => _showLoginError; set { if (_showLoginError != value) { _showLoginError = value; OnPropertyChange(); } } }
        public string LoginErrorMessage { get => _loginErrorMessage; set { if (_loginErrorMessage != value) { _loginErrorMessage = value; OnPropertyChange(); } } }
        
        //האם כפתור התחבר יהיה זמין
        public bool IsButtonEnabled { get { return ValidatePage(); } }
        #endregion

        #region Commands
        public ICommand LogInCommand { get; protected set; }
        #endregion



        /// <summary>
        /// c'tor
        /// </summary>
        /// <param name="service">מקבלת באמצעות DI את אובייקט הAPI</param>
        
        public MainPageViewModel(TriviaService service)
        {
            _service = service;
            UserName = string.Empty;
            Password=string.Empty;

            LogInCommand = new Command(async () =>
            {
                ShowLoginError = false;//הסתרת שגיאת לוגין
                try
                {
                    #region טעינת מסך ביניים
                    var lvm =new LoadingPageViewModel() { IsBusy = true };
                    await AppShell.Current.Navigation.PushModalAsync(new LoadingPage(lvm));
                    #endregion
                    var user = await _service.LogInAsync(UserName, Password);

                    lvm.IsBusy = false;
                    await AppShell.Current.Navigation.PopModalAsync();
                    if (!user.Success)
                    {
                        ShowLoginError = true;
                        LoginErrorMessage = user.Message;
                    }
                    else
                    {
                        await AppShell.Current.DisplayAlert("התחברת", "אישור להתחלת משחק", "אישור");
                        await SecureStorage.Default.SetAsync("LoggedUser", JsonSerializer.Serialize(user.User));
                        await AppShell.Current.GoToAsync("Game");
                    }



                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                   
                    await AppShell.Current.Navigation.PopModalAsync();
                }
                  

            });
        }
        #region פעולות עזר
        private bool ValidateUser()
        {
            return !(string.IsNullOrEmpty(_userName) || _userName.Length < 3);
        }
        private bool ValidatePassWord()
        {
            return !string.IsNullOrEmpty(Password);
        }

        private bool ValidatePage()
        {
            return ValidateUser() && ValidatePassWord();
        }
        #endregion

    }

}
