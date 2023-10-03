using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriviaMauiClient.Models
{
            public class AmericanQuestion
        {
            public string QText { get; set; }
            public string CorrectAnswer { get; set; }
            public string[] OtherAnswers { get; set; }
            public string CreatorNickName { get; set; }
        }
    
}
