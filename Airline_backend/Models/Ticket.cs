using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }
        public string DestinationFrom { get; set; }
        public string DestinationTo { get; set; }
        public DateTime DateAndTime { get; set; }
        public string Seat { get; set; }
        public double OriginalPrice { get; set; }
        public int Sale { get; set; }
        public string NameOfCompany { get; set; }
        public int Passengers { get; set; }
        public ICollection<Passengers> ListOfPassengers { get; set; }
        public double TotalPrice { get; set; }
        public DateTime DateOfReservation { get; set; }
        public double BusinessPrice { get; set; }
        public int FlightID { get; set; }
        public Flight Flight { get; set; }
        public string User { get; set; }
        public string Type { get; set; }
        public string Passport { get; set; }

        public Ticket()
        {
            ListOfPassengers = new HashSet<Passengers>();
        }

    }
}
