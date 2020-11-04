using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class Rentacar
    {
        public Rentacar()
        {
            this.ListOfVehicles = new HashSet<Vehicle>();
            this.Branches = new HashSet<Branch>();
            this.ProfitList = new HashSet<Profit>();
            this.DiscountGroups = new HashSet<DiscountGroup>();
            this.ListOfRatings = new HashSet<Rating>();
        }
        [Key]
        public int Id { get; set; }
        public string NameOfService { get; set; }
        /*[ForeignKey("Location")]
        public int LocationId { get; set; }*/
        [ForeignKey("Location")]
        public int LocationID { get; set; }
        public Location Location { get; set; }
        public string DescriptionOfService { get; set; }
        public ICollection<Vehicle> ListOfVehicles { get; set; }
        public ICollection<Branch> Branches { get; set; }
        public double AverageRatingOfService { get; set; }
        public ICollection<Profit> ProfitList { get; set; }
        public string Image { get; set; }
        public ICollection<DiscountGroup> DiscountGroups { get; set; }
        public ICollection<Rating> ListOfRatings { get; set; }


    }

    public class Branch
    {
        public Branch()
        {
            ListOfVehicles = new HashSet<Vehicle>();
        }

        [Key]
        public int Id { get; set; }
        /*[ForeignKey("Rentacar")]
        public int? ServiceId { get; set; }
        [ForeignKey("Location")]
        public int LocationId { get; set; }*/
        public int? RentacarID { get; set; }
        public string NameOfBranch { get; set; }
        public ICollection<Vehicle> ListOfVehicles { get; set; }
        public int LocationID { get; set; }
        public Location Location { get; set; }

    }

    public class Vehicle
    {

        public Vehicle()
        {
            DatesTaken = new HashSet<Date>();
            SpecialOfferDates = new HashSet<Date>();
            ListOfRatings = new HashSet<Rating>();
        }
        [Key]
        public int Id { get; set; }
        public int? RentacarID {get; set; }
        public int BranchID { get; set; }
        public string Name { get; set; }
        public string MarkOfVehicle { get; set; }
        public string ModelOfVehicle { get; set; }
        public int YearMade { get; set; }
        public ICollection<Date> DatesTaken { get; set; }
        public ICollection<Date> SpecialOfferDates { get; set; }
        public int SpecialDiscount { get; set; }
        public int NumberOfSeats { get; set; }
        public string TypeOfVehicle { get; set; }
        public double PricePerDay { get; set; }
        public bool IsRented { get; set; }
        public bool CanBeRented { get; set; }
        public double AverageRatingOfVehicle { get; set; }
        public ICollection<Rating> ListOfRatings { get; set; }

    }

    public class Profit
    {
        [Key]
        public int Id { get; set; }
        public int CarReservationID { get; set; }
        public int RentacarID { get; set; }
        public double EarnedMoney { get; set; }
        public DateTime DateTransactionWasMade { get; set; }
    }

    public class Rate
    {

    }

    public class CarSearch
    {
        public int? BranchID { get; set; }
        public int? RentacarID { get; set; }
        public string CityOT { get; set; }
        public string CityOR { get; set; }
        public string DateOT { get; set; }
        public string DateOR { get; set; }
        public double? MPrice { get; set; }
        public string Type { get; set; }
        public int? Seats { get; set; }
    }

    public class RentacarSearch
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Number { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }

    public class SpecialOfferInfo
    {
        public int RentacarID { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string City { get; set; }
    }
}
