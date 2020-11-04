using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Airline
    {
        public Airline()
        {
            Destinations = new HashSet<Destination>();
            ListOfFlights = new HashSet<Flight>();
            Planes = new HashSet<Plane>();
            DiscountGroups = new HashSet<DiscountGroup>();
            AllRatings = new HashSet<Rating>();
        }
        [Key]
        public int Id { get; set; }
        public string NameOfAirline { get; set; }
        public int LocationID { get; set; }
        public Location Location { get; set; }
        public string DescriptionOfAirline { get; set; }
        public ICollection<Destination> Destinations { get; set; }
        public ICollection<Flight> ListOfFlights { get; set; }
        public ICollection<Plane> Planes { get; set; }
        public string Image { get; set; }
        public ICollection<DiscountGroup> DiscountGroups { get; set; }
        public double RatingOfService { get; set; }
        public ICollection<Rating> AllRatings { get; set; }

    }

    public class Flight
    {
        public Flight()
        {
            ListOfTickets = new HashSet<Ticket>();
            MultiFlights = new HashSet<Flight>();
            AllRatings = new HashSet<Rating>();
        }
        [Key]
        public int Id { get; set; }
        public string DestinationFrom { get; set; }
        public string DestinationTo { get; set; }
        public DateTime DepartingDateTime { get; set; }
        public DateTime ReturningDateTime { get; set; }
        public DateTime TimeOfFlight { get; set; }
        public DateTime Duration { get; set; }
        public int NumberOfChangeovers { get; set; }
        public string LocationsChangeover { get; set; }
        public double TicketPrice { get; set; }
        public string FlightClass { get; set; }
        public ICollection<Ticket> ListOfTickets { get; set; }
        public string NameOfAirline { get; set; }
        public double BusinessPrice { get; set; }
        public ICollection<Flight> MultiFlights { get; set; }
        public ICollection<Rating> AllRatings {get; set;}
        public double AverageRating { get; set; }


    }

    public class FlightModel
    {
        //public FlightModel()
        //{
        //    Changeovers = new List<string>();
        //}
        public string DestinationFrom { get; set; }
        public string DestinationTo { get; set; }
        public string DepartureDate { get; set; }
        public string LandingDate { get; set; }
        public string Duration { get; set; }
        public string Length { get; set; }
        public int NumberOfChangeover { get; set; }
        public string Changeovers { get; set; }
        public int TicketPrice { get; set; }
        public string NameOfAirline { get; set; }
    }

    public class Destination
    {
        [Key]
        public int Id { get; set; }
        public string DestinationName { get; set; }
        public int AirlineId { get; set; }

    }

    public class Plane
    {
        [Key]
        public  int Id { get; set; }
        public int BusinessSeats { get; set; }
        public ICollection<BusRow> BusinessRows { get; set; }
        public int EconomySeats { get; set; }
        public ICollection<EcoRow> EconomyRows { get; set; }
        public int CountBusRows { get; set; }
        public int CountEcoRows { get; set; }
        public int AirlineId { get; set; }


        public Plane()
        {
            BusinessRows = new HashSet<BusRow>();
            EconomyRows = new HashSet<EcoRow>();
        }

    }

    public class BusRow
    {
        [Key]
        public int Id { get; set; }
        public int IdRow { get; set; }
        public int Seat1 { get; set; }
        public int Seat2 { get; set; }
        public int Seat3 { get; set; }
        public int Seat4 { get; set; }
        public int Seat5 { get; set; }
        public int Seat6 { get; set; }
        public int Seat7 { get; set; }
        public int PlaneId { get; set; }
    }

    public class EcoRow
    {
        [Key]
        public int Id { get; set; }
        public int IdRow { get; set; }
        public int Seat1 { get; set; }
        public int Seat2 { get; set; }
        public int Seat3 { get; set; }
        public int Seat4 { get; set; }
        public int Seat5 { get; set; }
        public int Seat6 { get; set; }
        public int Seat7 { get; set; }
        public int PlaneId { get; set; }

        public EcoRow()
        {

        }
    }

    public class Seat
    {
        [Key]
        public int Id { get; set; }
        public string IdCol { get; set; }
        public string Class { get; set; }
        public int RowId { get; set; }
        public string Type { get; set; }
        public int AirlineId { get; set; }
        public bool Disabled { get; set; }
        public string FullId { get; set; }

        public Seat()
        {
            Class = "seat-noseat";
        }
    }

    public class Passengers
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Passport { get; set; }
        public string Seat { get; set; }
        public int TicketId { get; set; }
        public string InviteUsername { get; set; }
        public bool EmailConfirmed { get; set; }
    }

    public class FilterModel
    {
        public string NameOfCompany { get; set; }
        public int PriceFrom { get; set; }
        public int PriceTo { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public ICollection<Flight> Flights { get; set; }
        public bool EconomyClass { get; set; }

        public FilterModel()
        {
            Flights = new HashSet<Flight>();
        }
    }
}
