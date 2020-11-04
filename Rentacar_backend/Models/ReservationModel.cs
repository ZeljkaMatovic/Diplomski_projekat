using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class ReservationModel
    {
        public string FlightType { get; set; }
        public string FlightClass { get; set; }
        public string DestinationFrom { get; set; }
        public string DestinationTo { get; set; }
        public string MultiDestinationFrom { get; set; }
        public string MultiDestinationTo { get; set; }
        public string DepartureDate { get; set; }
        public string ReturnDate { get; set; }


    }

    public class ReserveFlightModel
    {
        public int IdFlight1 { get; set; }
        public int IdFlight2 { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Passport { get; set; }
        public string Seat1 { get; set; }
        public string Seat2 { get; set; }
        public string SeatPass { get; set; }
        public int NumberOfPassengers { get; set; }
        public string FlightClass { get; set; }
        public string UserEmail { get; set; }
        public int IdTicket { get; set; }
        public int Sale { get; set; }
       // public List<Passengers> ListOfPassengers { get; set; }

        public ReserveFlightModel()
        {
            //ListOfPassengers = new List<Passengers>();
        }
    }
}
