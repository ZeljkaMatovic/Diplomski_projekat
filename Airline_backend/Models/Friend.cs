using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Friend
    {
        [Key]
        public int Id { get; set; }
        public string User1 { get; set; }
        public string User2 { get; set; }
        public string Type { get; set; }
    }
}
