
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using TriviaMauiClient.Models;
using TriviaMauiClient.Services;
using TriviaMauiClient.Views;

namespace TriviaMauiClient.ViewModels
{

    public class GamePageViewModel : ViewModel
    {

        #region Fields
        private string message;
        private string foundEmail;
        private string image;
        #endregion

        #region Service
        readonly private TriviaService _gameService;
        #endregion

        #region Properties
        public string Message { get => message; set { message = value; OnPropertyChange(); } }
        public string FoundEmail { get => foundEmail; set { foundEmail = value; OnPropertyChange(); } }
        public string ImageLocation { get =>    image; set { if (value != image) { image = value; OnPropertyChange(); } } }

        public ImageSource PhotoImageSource { get;set;}
        #endregion

        #region Commands
        public ICommand SearchCommand { get; protected set; }
        public ICommand UploadPhoto { get; protected set; } 

        public ICommand TakePictureCommand { get; protected set; }

        public ICommand ChangePhoto { get; protected set; }
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
            Message = $"Hello {user.FirstName}";
            //מוצאת אימייל של יוזר לפי השם שלו
            SearchCommand = new Command<string>( async(x) => FoundEmail = await _gameService.GetUserEmail(x));
            UploadPhoto = new Command(async()=> { await Shell.Current.DisplayAlert("g", "g", "ok"); });
            TakePictureCommand = new Command( TakePicture);
            ChangePhoto = new Command( TakePicture) ;
          
        }
        private async void TakePicture()
        {
            //נכון לכרגע (נובמבר 2023) יש בעיה בתמיכה בווינדוס
            //#זו דרך לייצר תנאים ברמת הקומפילציה לפני זמן ריצה
            
            #if ANDROID||IOS
            try
            {
                FileResult photo=null;

                //אם יש תמיכה במצלמה
                //יש לשים לב שעל מנת שיהיה ניתן להשתמש במצלמה צריך לתת הרשאות
                //https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/device-media/picker?tabs=android#tabpanel_1_android

                if (MediaPicker.Default.IsCaptureSupported)
                {
                    //חייבים להריץ ב
                    //UI
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        photo = await MediaPicker.Default.CapturePhotoAsync();
                       
                        #region מסך טעינה
                        var lvm = new LoadingPageViewModel() { IsBusy = true };
                        await Task.Delay(1000);
                        await Shell.Current.Navigation.PushModalAsync(new LoadingPage(lvm));
                        #endregion

                        //הצגת התמונה במסך ושליחתה לממשק.
                        await LoadPhoto(photo);

                        #region סגירת מסך טעינה
                        lvm.IsBusy = false;
                        await Shell.Current.Navigation.PopModalAsync();
                        #endregion

                    });
                }

            }
            catch(Exception ex) { }
#elif WINDOWS
              Shell.Current.DisplayAlert("לא נתמך", "כרגע לא ניתן להשתמש", "אישור");
#endif
             
           
        }

        private async Task LoadPhoto(FileResult photo)
        {
            try
            {
               
                var stream = await photo.OpenReadAsync();
                PhotoImageSource = ImageSource.FromStream(() => stream);
                OnPropertyChange(nameof(PhotoImageSource));
                await Upload(photo);
                

            }
            catch(Exception ex) { }
        }
        private async Task Upload(FileResult file)
        {
           
            try
            {

                // bool success = await _gameService.UploadPhoto(file);
                bool success = await _gameService.UploadFile(file);
                if (success)
                {
                    var u = JsonSerializer.Deserialize < User >( await SecureStorage.Default.GetAsync("LoggedUser"));
                    ImageLocation = await _gameService.GetImage() + $"{u.Id}.jpg";
                }
                else
                    Shell.Current.DisplayAlert("אין קשר לשרת", "לא הצלחתי להעלות את התמונה. נסה שוב", "אישור");
            }
            catch(Exception ex) { }

        }

        
    }
}
