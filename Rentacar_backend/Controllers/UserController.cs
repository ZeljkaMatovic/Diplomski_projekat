using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackEnd.Classes;
using System.Text.Json.Serialization;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private UserManager<User> _userManager;
        private AuthenticationContext _dataBase;

        public UserController(UserManager<User> arsManager, AuthenticationContext context)
        {
            _userManager = arsManager;
            _dataBase = context;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("GetUserProfile")]
        public async Task<Object> GetUserProfile()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            return new
            {
                user
            };
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("ModifyUser")]
        public async Task<Object> ModifyUser(User userChange)
        {
            string userId = "";
            await Task.Run(() =>
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
            });
            var user = await _dataBase.Users.FindAsync(userId);

            if (user == null)
                return Unauthorized();

            user.Name = userChange.Name;
            user.Lastname = userChange.Lastname;
            user.Email = userChange.Email;
            user.City = userChange.City;
            user.PhoneNumber = userChange.PhoneNumber;

            try
            {
                _dataBase.SaveChanges();
                return Ok();
            }
            catch(Exception)
            {
                return BadRequest("Error while processing!");
            }
        }

        [HttpGet]
        [Route("MakeCarReservation")]
        public async Task<Object> MakeCarReservation()
        {
            await Task.Run(() =>
            {
                var carRes = new CarReservation() { User = "dzema", StartDate = new DateTime(2020, 6, 20), EndDate = new DateTime(2020, 7, 20), Rated = false, PricePerDay = 10, TotalPrice = 130, SpecialOffer = false, VehicleID = 1 };
                //_dataBase.CarReservations.Add(carRes);
                var carRes1 = new CarReservation() { User = "dzema", StartDate = new DateTime(2020, 4, 20), EndDate = new DateTime(2020, 5, 20), Rated = false, PricePerDay = 10, TotalPrice = 130, SpecialOffer = false, VehicleID = 1 };
                carRes1.Vehicle = _dataBase.Vehicles.Find(carRes.VehicleID);
                var user = _dataBase.Users.Include(u => u.ReservedCars).ToList().Find(u => u.UserName == "dzema");
                user.ReservedCars.Add(carRes);
                user.ReservedCars.Add(carRes1);
                _dataBase.SaveChanges();
            });

            return Ok();
        }

        // SVE ISPOD MORAS DA TESTIRAS IDIOTE

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("GetMyCarReservations")]
        public async Task<Object> GetMyCarReservations()
        {
            string userId = "";
            User user = new User();
            await Task.Run(() =>
            {
                userId = User.Claims.First(c => c.Type == "UserID").Value;
                user = _dataBase.Users.Include(u => u.ReservedCars).ThenInclude(r => r.Vehicle).ToList().Find(u => u.UserName == userId);
            });
            if (user == null || user.Role != "ru")
                return Unauthorized();

            return Ok(user.ReservedCars);
        }

        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("CancelCarReservation")]
        public async Task<Object> CancelCarReservation(string id)
        {
            string userId = "";
            User user = new User();
            await Task.Run(() =>
            {
                userId = User.Claims.First(c => c.Type == "UserID").Value;
                user = _dataBase.Users.Include(u => u.ReservedCars).ToList().Find(u => u.UserName == userId);
            });
            if (user == null || user.Role != "ru")
                return Unauthorized();

            var resId = 0;
            var res = new CarReservation();
            var date = new DateTime();
            await Task.Run(() =>
            {
                resId = int.Parse(id);

                res = _dataBase.CarReservations.Include(res => res.Vehicle).ToList().Find(c => c.Id == resId);
                date = res.StartDate;
                date.AddDays(2);
            });
            if (date <= DateTime.Now)
                return BadRequest("Deadline for canceling reservation has passed");

            var car = new Vehicle();
            await Task.Run(() =>
            {
                car = _dataBase.Vehicles.Include(v => v.DatesTaken).ToList().Find(v => v.Id == res.Vehicle.Id);

                for (var d = res.StartDate; d <= res.EndDate; d = d.AddDays(1))
                {
                    car.DatesTaken.ToList().RemoveAll(t => t.DateTime == d);
                }

                //_dataBase.CarReservations.Remove(res);
                user.ReservedCars.Remove(res);
                var rent = _dataBase.Rentacars.Include(r => r.ProfitList).ToList().Find(r => r.Id == res.Vehicle.RentacarID);
                var profit = _dataBase.Profits.ToList().Find(p => p.RentacarID == rent.Id && p.CarReservationID == res.Id);
                rent.ProfitList.Remove(profit);
                _dataBase.CarReservations.Remove(res);
                _dataBase.Profits.Remove(profit);
                _dataBase.SaveChanges();
                var ret = _dataBase.CarReservations.Include(c => c.Vehicle).ToList().FindAll(c => c.User == user.UserName);
            });
            return Ok(user.ReservedCars);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("RateRentacarAndCar")]
        public async Task<Object> RateRentacarAndCar(IdModel model)
        {

            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _dataBase.Users.FindAsync(userId);

            if (user == null || user.Role != "ru")
                return Unauthorized();

            var reservation = _dataBase.CarReservations.Find(model.Id);

            if (reservation == null)
                return BadRequest("Reservation does not exist!");
            if (reservation.Rated)
                return BadRequest("Already rated!");

            var vehicle = _dataBase.Vehicles.Include(v => v.ListOfRatings).ToList().Find(v => v.Id == reservation.VehicleID);
            var rentacar = _dataBase.Rentacars.Include(r => r.ListOfRatings).ToList().Find(r => r.Id == vehicle.RentacarID);

            if (vehicle == null)
                return BadRequest("Vehicle doesn't exist!");
            if (rentacar == null)
                return BadRequest("Rentacar doesn0t exist!");
            reservation.Rated = true;

            var rating1 = new Rating() { Rated = model.Rate1, VehicleID = vehicle.Id };
            var rating2 = new Rating() { Rated = model.Rate2, RentacarID = rentacar.Id };
            vehicle.ListOfRatings.Add(rating1);
            rentacar.ListOfRatings.Add(rating2);
            vehicle.AverageRatingOfVehicle = BackEnd.Classes.CalculateRating.Calculate(vehicle.ListOfRatings.ToList());
            rentacar.AverageRatingOfService = BackEnd.Classes.CalculateRating.Calculate(rentacar.ListOfRatings.ToList());
            _dataBase.SaveChanges();

            var ret = _dataBase.Users.Include(u => u.ReservedCars).ThenInclude(r => r.Vehicle);

            return Ok(ret);
        }


        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("ReserveCar")]
        public async Task<Object> ReserveCar(CarReservationInfo info)
        {
            string userId = "";
            User user = new User();
            await Task.Run(() =>
            {
                userId = User.Claims.First(c => c.Type == "UserID").Value;
                user = _dataBase.Users.Include(u => u.ReservedCars).ToList().Find(u => u.UserName == userId);
            });

            if (user == null || user.Role != "ru")
                return Unauthorized();

            var vehicle = new Vehicle();
            await Task.Run(() =>
            {
                vehicle = _dataBase.Vehicles.Include(v => v.DatesTaken).Include(v => v.SpecialOfferDates).ToList().Find(v => v.Id == info.VehicleID);
            });

            if (vehicle == null)
                return BadRequest("Vehicle doesn't exist");

            var listOfDates = new List<DateTime>();

            for(var d = DateTime.Parse(info.StartDate); d <= DateTime.Parse(info.EndDate); d = d.AddDays(1))
            {
                if (info.SpecialOffer ? Classes.DatesManipulator.ExistsInList(vehicle.SpecialOfferDates.ToList(), d) : Classes.DatesManipulator.ExistsInList(vehicle.DatesTaken.ToList(), d))
                    return BadRequest("Car is not avaible in selected dates");
                listOfDates.Add(d);
            }

            var reservation = new CarReservation()
            {
                User = user.UserName,
                Rated = false,
                EndDate = DateTime.Parse(info.EndDate),
                StartDate = DateTime.Parse(info.StartDate),
                SpecialOffer = info.SpecialOffer,
                PricePerDay = info.PricePerDay,
                TotalPrice = info.TotalPrice,
                VehicleID = info.VehicleID
            };

            await Task.Run(() =>
            {
                listOfDates.ForEach(d => vehicle.DatesTaken.Add(new Date() { DateTime = d }));
                user.ReservedCars.Add(reservation);
                vehicle.DatesTaken.ToList().Sort(new DateListSort());
                _dataBase.SaveChanges();
                var rent = _dataBase.Rentacars.Include(r => r.ProfitList).ToList().Find(r => r.Id == vehicle.RentacarID);
                var profit = new Profit() { RentacarID = rent.Id, EarnedMoney = reservation.TotalPrice, CarReservationID = reservation.Id, DateTransactionWasMade = DateTime.Now };
                rent.ProfitList.Add(profit);

                _dataBase.SaveChanges();
            });

            return Ok(reservation);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("GiveMeSpecialOfferCars")]
        public async Task<Object> GiveMeSpecialOfferCars(SpecialOfferInfo info)
        {
            string userId = "";
            User user = new User();
            await Task.Run(() =>
            {
                userId = User.Claims.First(c => c.Type == "UserID").Value;
                user = _dataBase.Users.ToList().Find(u => u.UserName == userId);
            });

            if (user == null || user.Role != "ru")
                return Unauthorized();

            var rent = new List<Vehicle>();
            var ret = new List<Vehicle>();
            await Task.Run(() =>
            {
                rent = _dataBase.Vehicles.Include(v => v.DatesTaken).Include(v => v.SpecialOfferDates).ToList().FindAll(v => v.RentacarID == info.RentacarID);

                var branches = _dataBase.Branches.Include(b => b.Location).ToDictionary(b => b.Id);

                
                rent.ForEach(v =>
                {
                    if (branches[v.BranchID].Location.NameOfCity.ToLower().Contains(info.City.ToLower())
                    && DatesManipulator.CompareLists(v.DatesTaken.ToList(), DatesManipulator.GiveMeDateList(DateTime.Parse(info.StartDate), DateTime.Parse(info.EndDate)))
                    && DatesManipulator.ContainsAllDates(v.SpecialOfferDates.ToList(), DatesManipulator.GiveMeDateList(DateTime.Parse(info.StartDate), DateTime.Parse(info.EndDate))))
                        ret.Add(v);
                });
            });
            return Ok(ret);
        }

        //[HttpGet]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Route("FindRoundAwayTrips")]
        //public async Task<Object> FindRoundAwayTrips()
        //{
        //    string userId = User.Claims.First(c => c.Type == "UserID").Value;
        //    var user = _dataBase.Users.Include(u => u.ReservedTickets).ToList().Find(u => u.UserName == userId);

        //    var retVal = new List<Ticket>();
        //    if (user == null || user.Role != "ru")
        //        return Unauthorized();

        //    for(int i = 0; i < user.ReservedTickets.Count; i++)
        //    {
        //        for(int j = i + 1; j < user.ReservedTickets.Count; j++)
        //        {
        //            if(user.ReservedTickets.ToList()[i].DestinationFrom == user.ReservedTickets.ToList()[j].DestinationTo &&
        //                user.ReservedTickets.ToList()[i].DestinationTo == user.ReservedTickets.ToList()[j].DestinationFrom)
        //            {
        //                retVal.Add(user.ReservedTickets.ToList()[i]);
        //                retVal.Add(user.ReservedTickets.ToList()[j]);
        //            }
        //        }
        //    }

        //    if(retVal.Count == 2)
        //    {
        //        return Ok(new { canDo = true, destination = retVal[0].DestinationTo, startDate = retVal[0].DateAndTime, endDate = retVal[1].DateAndTime });
        //    }

        //    return Ok( new { canDo = false });
        //}

        [HttpPost]
        [Route("GetAllUsers")]
        public async Task<Object> GetAllUsers(IdModel model)
        {
            var users = new List<User>();
            await Task.Run(() =>
            {
                foreach (User u in _dataBase.Users)
                {
                    if (u.Email != model.Email && u.Role == "ru")
                    {
                        users.Add(u);
                    }
                }
            });

            List<User> friends = GetFriendsTemp(model);

            await Task.Run(() =>
            {
                foreach (User f in friends)
                {
                    if (users.Contains(f))
                        users.Remove(f);
                }
            });
            return Ok(users);
        }

        [HttpPost]
        [Route("AddFriend")]
        public async Task<Object> AddFriend(IdModel model)
        {
            var friend = new Friend
            {
                User1 = model.Email,
                User2 = model.Email2,
                Type = "Request"
            };
            await Task.Run(() =>
            {
                _dataBase.Friends.Add(friend);
                _dataBase.SaveChanges();
            });
            return Ok();
        }

        [HttpPost]
        [Route("GetRequests")]
        public async Task<Object> GetRequests(IdModel model)
        {
            var requests = new List<string>();
            await Task.Run(() =>
            {
                foreach (Friend f in _dataBase.Friends)
                {
                    if (f.User2 == model.Email && f.Type == "Request")
                    {
                        requests.Add(f.User1);
                    }
                }
            });
            var users = new List<User>();
            await Task.Run(() =>
            {
                foreach (string s in requests)
                {
                    var user = _dataBase.Users.SingleOrDefault(u => u.Email == s);
                    users.Add(user);
                }
            });
            return Ok(users);
        }

        [HttpPost]
        [Route("AcceptFriend")]
        public async Task<Object> AcceptFriend(IdModel model)
        {
            await Task.Run(() =>
            {
                foreach (Friend f in _dataBase.Friends)
                {
                    if (f.User1 == model.Email && f.User2 == model.Email2)
                    {
                        f.Type = "Accepted";
                    }
                }

                _dataBase.SaveChanges();
            });
            return Ok();
        }

        [HttpPost]
        [Route("DeclineFriend")]
        public async Task<Object> DeclineFriend(IdModel model)
        {
            await Task.Run(() =>
            {
                foreach (Friend f in _dataBase.Friends)
                {
                    if (f.User1 == model.Email && f.User2 == model.Email2)
                    {
                        f.Type = "Declined";
                        _dataBase.Friends.Remove(f);
                    }
                }

                _dataBase.SaveChanges();
            });
            return Ok();
        }

        [HttpPost]
        [Route("GetFriends")]
        public async Task<Object> GetFriends(IdModel model)
        {
            var friends = new List<string>();
            await Task.Run(() =>
            {
                foreach (Friend f in _dataBase.Friends)
                {
                    if (f.User1 == model.Email && f.Type == "Accepted")
                    {
                        friends.Add(f.User2);
                    }
                    else if (f.User2 == model.Email && f.Type == "Accepted")
                    {
                        friends.Add(f.User1);
                    }
                }
            });

            var users = new List<User>();
            await Task.Run(() =>
            {
                foreach (string s in friends)
                {
                    var user = _dataBase.Users.SingleOrDefault(u => u.Email == s);
                    users.Add(user);
                }
            });
            return Ok(users);
        }

        public List<User> GetFriendsTemp(IdModel model)
        {
            var friends = new List<string>();

            foreach (Friend f in _dataBase.Friends)
            {
                if (f.User1 == model.Email && (f.Type == "Accepted" || f.Type == "Request"))
                {
                    friends.Add(f.User2);
                }
                else if (f.User2 == model.Email && (f.Type == "Accepted" || f.Type == "Request"))
                {
                    friends.Add(f.User1);
                }
            }

            var users = new List<User>();

            foreach (string s in friends)
            {
                var user = _dataBase.Users.SingleOrDefault(u => u.Email == s);
                users.Add(user);
            }

            return users;
        }

        [HttpPost]
        [Route("DeleteFriend")]
        public async Task<Object> DeleteFriend(IdModel model)
        {
            await Task.Run(() =>
            {
                foreach (Friend f in _dataBase.Friends)
                {
                    if (f.User1 == model.Email && f.User2 == model.Email2)
                    {
                        f.Type = "Deleted";
                        _dataBase.Friends.Remove(f);
                    }
                    else if (f.User2 == model.Email && f.User1 == model.Email2)
                    {
                        f.Type = "Deleted";
                        _dataBase.Friends.Remove(f);
                    }
                }

                _dataBase.SaveChanges();
            });
            return Ok();
        }

        [HttpPost]
        [Route("Base64Encode")]
        public async Task<Object> Base64Encode(IdModel model)
        {
            var user = await _dataBase.Users.FindAsync(model.Name);

            user.Image = model.Image;
            _dataBase.SaveChanges();

            return Ok();
        }

        [HttpPost]
        [Route("Base64Decode")]
        public async Task<Object> Base64Decode(IdModel model)
        {
            var user = await _dataBase.Users.FindAsync(model.Name);

            string retval = "{\"image\":\"" + user.Image + "\"}";
            return Ok(retval);
        }
    }
}