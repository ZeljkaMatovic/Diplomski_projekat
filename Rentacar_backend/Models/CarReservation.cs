using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class CarReservation
    {
        [Key]
        public int Id { get; set; }
        public string User { get; set; }
        public int VehicleID { get; set; }
        public Vehicle Vehicle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double PricePerDay { get; set; }
        public double TotalPrice { get; set; }
        public bool SpecialOffer { get; set; }
        public bool Rated { get; set; }
    }

    public class CarReservationInfo
    {
        public int Id { get; set; }
        public string User { get; set; }
        public int VehicleID { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public double PricePerDay { get; set; }
        public double TotalPrice { get; set; }
        public bool SpecialOffer { get; set; }
        public bool Rated { get; set; }
    }
}
