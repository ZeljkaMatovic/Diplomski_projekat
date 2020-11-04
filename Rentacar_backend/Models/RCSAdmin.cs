using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class RCSAdmin : IdentityUser
    {
        [Key]
        public override string UserName { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Role { get; set; }
        public string Image { get; set; }

        /*[ForeignKey("Rentacar")]
        public int ServiceId { get; set; }*/
        public int RentacarID { get; set; }
        public Rentacar Rentacar { get; set; }

    }
}
