using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Location
    {
        [Key]
        public int Id { get; set; }
        public string NameOfCity { get; set; }
        public string NameOfStreet { get; set; }
        public string NumberInStreet { get; set; }
        public double GeoWidth { get; set; }
        public double GeoHeight { get; set; }
    }
}
