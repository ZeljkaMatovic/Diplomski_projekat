using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Date
    {
        [Key]
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
    }
}
