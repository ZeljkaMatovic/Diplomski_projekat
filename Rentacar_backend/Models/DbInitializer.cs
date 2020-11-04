using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public static class DbInitializer
    {
        public static void Initialize(AuthenticationContext context, UserManager<User> _userManager)
        {
            context.Database.EnsureCreated();

            if(context.Rentacars.Any())
            {
                return;
            }

            var loc = new Location() { Id = 0, GeoHeight = -0.1477, GeoWidth = 51.520599, NameOfCity = "London", NameOfStreet = "Harley Street", NumberInStreet = "10" };
            var rentcar = new Rentacar() { Id = 0, AverageRatingOfService = 0, Location = loc, DescriptionOfService = "", NameOfService = "Rent-a-England"
            , Image = "London.jpg" };
            
            context.Rentacars.Add(rentcar);
            var loc1 = new Location() { Id = 0, GeoHeight = -0.1477, GeoWidth = 51.520599, NameOfCity = "London", NameOfStreet = "Harley Street", NumberInStreet = "10" };
            var branch = new Branch() { Location = loc1, NameOfBranch = "London", RentacarID = rentcar.Id };
            var vehicle = new Vehicle() { Name = "Auto1", MarkOfVehicle = "Audi", ModelOfVehicle = "A5", YearMade = 2010, CanBeRented = true, NumberOfSeats = 5, PricePerDay = 20, AverageRatingOfVehicle = 0, RentacarID = rentcar.Id, BranchID = branch.Id, TypeOfVehicle = "Limousine" };
            var vehicle1 = new Vehicle() { Name = "Auto2", MarkOfVehicle = "Audi", ModelOfVehicle = "A3", YearMade = 2010, CanBeRented = true, NumberOfSeats = 5, PricePerDay = 20, AverageRatingOfVehicle = 0, RentacarID = rentcar.Id, BranchID = branch.Id, TypeOfVehicle = "Limousine" };
            rentcar.ListOfVehicles.Add(vehicle);
            branch.ListOfVehicles.Add(vehicle);
            rentcar.ListOfVehicles.Add(vehicle1);
            branch.ListOfVehicles.Add(vehicle1);
            rentcar.Branches.Add(branch);

            var sys = new User("sys")
            {
                UserName = "dzeksiStar",
                Email = "admin@gmail.com",
                EmailConfirmed = true,
                Name = "Mihajlo",
                Lastname = "Dzever",
                Role = "sys", 
                Image = "userAvatar.png"
            };

            var rcsa = new User("rcsa")
            {
                UserName = "rcsadmin",
                Email = "rcsadmin@gmail.com",
                EmailConfirmed = true,
                Name = "Mihajlo",
                Lastname = "Dzever",
                RentacarID = 1,
                Rentacar = rentcar,
                Role = "rcsa",
                Image = "userAvatar.png"
            };

            var arsa = new User("arsa")
            {
                UserName = "arsadmin",
                Email = "arsadmin@gmail.com",
                EmailConfirmed = true,
                Name = "Zeljka",
                Lastname = "Matovic",
                ServiceID = 1,
                //Airline = airline,
                Role = "arsa",
                Image = "userAvatar.png"
            };


            try
            {

                _userManager.CreateAsync(rcsa, "12345678");
                _userManager.CreateAsync(arsa, "12345678");
                _userManager.CreateAsync(sys, "12345678");
            }
            catch (Exception e)
            {
                throw e;
            }

            //var seat10 = context.Seats.SingleOrDefault(s => s.IdCol == "A" && s.RowId == 1);
            //seat10.Class = "seat-yellow";

            //var seat20 = context.Seats.SingleOrDefault(s => s.IdCol == "C" && s.RowId == 1);
            //seat20.Class = "seat-yellow";

            //var seat30 = context.Seats.SingleOrDefault(s => s.IdCol == "D" && s.RowId == 1);
            //seat30.Class = "seat-yellow";

            context.SaveChanges();
        }
    }
}
