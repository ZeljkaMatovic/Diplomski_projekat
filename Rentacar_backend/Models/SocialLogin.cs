using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class SocialLogin
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string IdToken { get; set; }
    }
}
