using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class VehicleRequest
    {
        public int ID { get; set; }
        public int RentacarID { get; set; }
        public int BranchID { get; set; }
        public string Name { get; set; }
        public string MarkOfVehicle { get; set; }
        public string ModelOfVehicle { get; set; }
        public int YearMade { get; set; }
        public int NumberOfSeats { get; set; }
        public string TypeOfVehicle { get; set; }
        public double PricePerDay { get; set; }
        public bool CanBeRented { get; set; }
        public List<string> SpecialOfferDates { get; set; }
        public int SpecialOffer { get; set; }
    }
}
