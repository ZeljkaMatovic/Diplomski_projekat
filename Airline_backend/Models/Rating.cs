using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }
        public int Rated { get; set; }
        public int? AirlineID { get; set; }
        public int? RentacarID { get; set; }
        public int? VehicleID { get; set; }
        public int? FlightID { get; set; }
    }
}
