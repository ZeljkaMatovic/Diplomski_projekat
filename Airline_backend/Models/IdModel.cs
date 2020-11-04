using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class IdModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Email2 { get; set; }
        public int Rate1 { get; set; }
        public int Rate2 { get; set; }
        public string Date { get; set; }
        public string Type { get; set; }
        public int EcoNumber { get; set; }
        public int BusNumber { get; set; }
        public string Image { get; set; }
    }
}
