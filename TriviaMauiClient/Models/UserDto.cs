using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriviaMauiClient.Models
{
    public class UserDto
    {
       public User User { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
