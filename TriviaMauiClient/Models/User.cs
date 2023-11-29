using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriviaMauiClient.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; } 

        public string LastName { get; set; } 

        public string UserPswd { get; set; }

    }
}
    