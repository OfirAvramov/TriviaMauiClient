
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using TriviaMauiClient.Models;


namespace TriviaMauiClient.Services
{
    public class TriviaService
    {
        readonly HttpClient _httpClient;
        readonly JsonSerializerOptions _serializerOptions;
        const string URL = @"https://zr8z94hw-7004.euw.devtunnels.ms/Walla/";
        const string IMAGE_URL = @"https://zr8z94hw-7004.euw.devtunnels.ms/";

        public TriviaService()
        {
            _httpClient = new HttpClient();

            //הגדרות הסיריליזאציה
            //האם להעלם מאותיות גדולות/קטנות
            //האם לייצר json עם רווחים
            //כיצד לטפל במקרה של הפניות מעגליות
            _serializerOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
            };

        }
        //"Get"
        #region GetHello
        /// <summary>
        /// פעולה קטנה לבדיקת תקשורת מול השרת.
        /// השרת מחזיר מחרוזת של שלום עולם
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetHello()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{URL}Hello");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                return "Something is Wrong";
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return "ooops";
        }
        #endregion

        //"Post"
        #region LogInAsync
        /// <summary>
        /// פעולה המבצעת התחברות
        /// הפעולה מקבלת שם משתמש וסיסמה
        /// יוצרת אובייקט יוזר שנשלח לשרת לצורך הזדהות
        /// אם ההזדהות הצליחה הפעולה תחזיר את היוזר עם פרטיו המלאים
        /// אחרת יוחזר אובייקט שגיאה
        /// <param name="userName">שם משתמש</param>
        /// <param name="password">סיסמה</param>
        /// <returns>אובייקט מטיפוס משתמש</returns>
        /// </summary>
        public async Task<UserDto> LogInAsync(string userName, string password)
        {
            try
            {
                //האובייקט לשליחה
                User user = new User() { Email = userName, Id = 1, FirstName = "kuku", LastName = "kaka", UserPswd = password };
                //מבצעת סיריליזציה
                var jsonContent = JsonSerializer.Serialize(user, _serializerOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($@"{URL}Login", content);

                switch (response.StatusCode)
                {
                    case (HttpStatusCode.OK):
                        {
                            jsonContent = await response.Content.ReadAsStringAsync();
                            User u = JsonSerializer.Deserialize<User>(jsonContent, _serializerOptions);
                            await Task.Delay(2000);
                            return new UserDto() { Success = true, Message = string.Empty, User = u };

                        }
                    case (HttpStatusCode.Forbidden):
                        {
                            return new UserDto() { Success = false, User = null, Message = ErrorMessages.INVALID_LOGIN };

                        }

                }

            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return new UserDto() { Success = false, User = null, Message = ErrorMessages.INVALID_LOGIN };

        }
        #endregion

        //"Get with Parameters"
        #region GetUserEmail
        /// <summary>
        /// פעולה המחזירה אימייל של יוזר על פי הכינוי שלו
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public async Task<string> GetUserEmail(string x)
        {
            try
            {
                var response = await _httpClient.GetAsync($@"{URL}GetUserEmail?nick={x}");
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return await response.Content.ReadAsStringAsync();
                    case HttpStatusCode.NotFound:
                        return "לא קיים יוזר כזה";
                    default:
                        return $":שגיאת שרת אחרת{response.StatusCode.ToString()}";
                }


            }
            catch (Exception ex) { Console.WriteLine(ex.Message); };
            return "תקלה";
        }
        #endregion
        #region Register

        public async Task<User> RegisterUser(User user)
        {
            //create the json
            var jsonContent = JsonSerializer.Serialize(user, _serializerOptions);
            //add the json to the content of the request
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            try
            {
                //send it to the server
                var response = await _httpClient.PostAsync($"{URL}RegisterUser", content);

                switch (response.StatusCode)
                {
                    case (HttpStatusCode.OK):
                    case (HttpStatusCode.Created):
                        {
                            jsonContent = await response.Content.ReadAsStringAsync();
                            user = JsonSerializer.Deserialize<User>(jsonContent, _serializerOptions);
                            return user;

                        }
                    case (HttpStatusCode.Conflict):
                        {
                            return null;
                        }

                }

            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return null;

        }
        #endregion


        /// <summary>
        /// upload photo/file to the server
        /// </summary>
        /// <param name="file">File result מייצג קובץ והנתוני שלו</param>
        /// <returns></returns>
        public async Task<bool> UploadPhoto(FileResult file)
        {

           try
            {
                //קובץ הוא לא מחלקה...
                //נדרש להמיר אותו למערך של בייטים על מנת שיוכל לעבור ברשת
                byte[] bytes;

                #region המרה של הקובץ
                using (MemoryStream ms = new MemoryStream())
                {
                    //קריאה את אוסף הנתונים בקובץ
                    var stream = await file.OpenReadAsync();
                    //העתקת רצף הבייטים למקום זמני בזיכרון
                   stream.CopyTo(ms);
                    //המרה למערך
                    bytes = ms.ToArray();
                }
                #endregion

                //אובייקט המאפשר לשמור אוסף של קבצים שנוכל לצרף אותו לבקשה לשרת
                var multipartFormDataContent = new MultipartFormDataContent();

                //תוכן של בקשה המבוסס על מערך בתים
                var content = new ByteArrayContent(bytes);
                //פרמטר הראשון הוא התוכן
                //פרמטר השני - זהה לשם הפרמטר של הקובץ בפעולה השרת
                //הפרמטר השלישי הוא שם הקובץ עצמו
                multipartFormDataContent.Add(content, "file","robot.jpg");
       

                // Send POST request
               
                var response = await _httpClient.PostAsync($@"{URL}UploadImage?Id=1", multipartFormDataContent);
                if (response.IsSuccessStatusCode) { return true; }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        public async Task<string> GetImage() { return $"{IMAGE_URL}images/"; }

        public async Task<bool> UploadFile(FileResult file)
        {

            try
            {
                //קובץ הוא לא מחלקה...
                //נדרש להמיר אותו למערך של בייטים על מנת שיוכל לעבור ברשת
                byte[] bytes;

                #region המרה של הקובץ
                using (MemoryStream ms = new MemoryStream())
                {
                    //קריאה את אוסף הנתונים בקובץ
                    var stream = await file.OpenReadAsync();
                    //העתקת רצף הבייטים למקום זמני בזיכרון
                    stream.CopyTo(ms);
                    //המרה למערך
                    bytes = ms.ToArray();
                }
                #endregion

                //אובייקט המאפשר לשמור אוסף של קבצים שנוכל לצרף אותו לבקשה לשרת
                var multipartFormDataContent = new MultipartFormDataContent();

                //תוכן של בקשה המבוסס על מערך בתים
                //פרמטר הראשון הוא התוכן
                //פרמטר השני - זהה לשם הפרמטר של הפרמטר כפי שמופיע בחתימת הפעולה בשרת
                //הפרמטר השלישי הוא שם הקובץ עצמו
                var content = new ByteArrayContent(bytes);
                multipartFormDataContent.Add(content, "file", "robot.jpg");
                //ניתן לחזור על הפעולה אם נרצה קובץ נוסף או במקרה הזה
                //אובייקט
                var userContent = JsonSerializer.Serialize(new User() { Id=1, Email="kuku@kuku.com", FirstName="kuku", LastName="kiki", UserPswd="1234"} , _serializerOptions);
                //הפרמטר הראשון הוא התוכן
                //הפרמטר השני זה שם הפרמטר כפי שמופיע בחתימת הפעולה בשרת
                multipartFormDataContent.Add(new StringContent(userContent,Encoding.UTF8, "application/json"),"user");

                // Send POST request
                var response = await _httpClient.PostAsync($@"{URL}UploadFile", multipartFormDataContent);
                if (response.IsSuccessStatusCode) { return true; }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
    }
        
}


