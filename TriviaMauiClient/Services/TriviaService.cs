
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TriviaMauiClient.Models;

namespace TriviaMauiClient.Services
{
    public class TriviaService
    {
        HttpClient _httpClient;
        JsonSerializerOptions _serializerOptions;
        const string URL = @"https://zr8z94hw-44376.euw.devtunnels.ms/AmericanQuestions/";

        public TriviaService()
        {
            _httpClient = new HttpClient();
          
                
            _serializerOptions = new JsonSerializerOptions() { WriteIndented=true, PropertyNameCaseInsensitive = true };

        }

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
            catch(Exception ex ) { Console.WriteLine(ex.Message); }
            return "ooops";
        }  
        public  async Task<UserDto> LogInAsync(string userName, string password)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(new User() { Email = userName, Password = password }, _serializerOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{URL}Login", content);
               switch(response.StatusCode)
                {
                    case (HttpStatusCode.OK):
                        {
                            jsonContent = await response.Content.ReadAsStringAsync();
                            User u = JsonSerializer.Deserialize<User>(jsonContent,_serializerOptions);
                            return new UserDto() { Success = true, Message = string.Empty, User = u };
                            
                        }
                        case (HttpStatusCode.Unauthorized):
                        {
                            return new UserDto() { Success = false, User = null, Message = ErrorMessages.INVALID_LOGIN };
                           
                        }

                }
              
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return new UserDto() { Success = false, User = null, Message = ErrorMessages.INVALID_LOGIN };

        }

        public async Task<string> GetUserEmail(string x)
        {
            try
            {
                var response =await _httpClient.GetAsync($@"{URL}GetUserEmail?nick={x}");
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return await response.Content.ReadAsStringAsync();
                    case HttpStatusCode.NotFound:
                        return "לא קיים יוזר כזה";
                }

                return "תקלה";
            }
            catch(Exception ex) { };
            return "תקלה";
        }
    }

}
