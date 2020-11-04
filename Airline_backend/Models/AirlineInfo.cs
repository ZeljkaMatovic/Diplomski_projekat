using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class AirlineInfo
    {
        public int Id { get; set; }
        public string NameOfService { get; set; }
        public string Description { get; set; }
        public string CityName { get; set; }
        public string StreetName { get; set; }
        public string Number { get; set; }
        public string Image { get; set; }
    }
}
