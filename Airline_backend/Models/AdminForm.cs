using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class AdminForm
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Role { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public Nullable<int> RentacarID { get; set; }
        public Nullable<int> ServiceID { get; set; }
    }
}
