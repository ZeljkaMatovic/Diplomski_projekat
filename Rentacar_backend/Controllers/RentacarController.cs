using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.Classes;
using BackEnd.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentacarController : ControllerBase
    {
        private AuthenticationContext _dataBase;

        public RentacarController(AuthenticationContext context)
        {
            _dataBase = context;
        }

        [HttpGet]
        [Route("GetAllRentacars")]
        public async Task<Object> GetAllRentacars()
        {
            var rentacars = new List<Rentacar>();
            await Task.Run(() =>
            {
                rentacars = _dataBase.Rentacars.Include(r => r.Location).Include(r => r.ListOfVehicles).ThenInclude(v => v.DatesTaken)
                .Include(r => r.ListOfRatings).Include(r => r.ListOfVehicles).ThenInclude(v => v.SpecialOfferDates).Include(r => r.ListOfVehicles).ThenInclude(v => v.ListOfRatings)
                .Include(r => r.ProfitList).Include(r => r.Branches).ThenInclude(b => b.ListOfVehicles).Include(r => r.Branches).ThenInclude(b => b.Location).ToList();
            });

            if(rentacars.Count == 0)
            {
                return NotFound("No rentaracar values found");
            }

            return Ok(rentacars);
        }
        
        //[HttpGet]
        //[Route("GetAllRentacarsDiscGroups")]
        //public async Task<Object> GetAllRentacarsDiscGroups()
        //{
            
        //    //var rentacars = _dataBase.Rentacars.Include(r => r.DiscountGroups);

        //    //return Ok(rentacars);
        //}

        [HttpPost]
        [Route("GetSearchedCars")]
        public async Task<Object> GetSearchedCars(CarSearch carSearch)
        {
            List<Vehicle> cars = new List<Vehicle>();
            try
            {
                if (carSearch.RentacarID != null)
                    if (carSearch.BranchID != null)
                        cars = _dataBase.Vehicles.Where(v => v.RentacarID == carSearch.RentacarID && v.BranchID == carSearch.BranchID).Include(v => v.DatesTaken).ToList();
                    else
                        cars = _dataBase.Vehicles.Where(v => v.RentacarID == carSearch.RentacarID).Include(v => v.DatesTaken).ToList();
                else
                    cars = _dataBase.Vehicles.Include(v => v.DatesTaken).ToList();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            await Task.Run(() =>
            {
                int counter = 0;
                if (carSearch.CityOT != null)
                    cars.RemoveAll(v => !carSearch.CityOT.ToLower().Contains(_dataBase.Locations.Find(_dataBase.Branches.Find(v.BranchID).LocationID).NameOfCity.ToLower()));
                if (carSearch.DateOR != null)
                    for (var d = DateTime.Parse(carSearch.DateOT); d <= DateTime.Parse(carSearch.DateOR); d = d.AddDays(1))
                    {
                        ++counter;
                        cars.RemoveAll(v => v.DatesTaken.ToList().Find(date => date.DateTime == d) != null);
                    }
                else if (carSearch.DateOT != null)
                    cars.RemoveAll(v => v.DatesTaken.ToList().Find(date => date.DateTime == DateTime.Parse(carSearch.DateOT)) != null);
                counter = counter != 0 ? counter : 1;
                if (carSearch.MPrice != null)
                    cars.RemoveAll(v => carSearch.MPrice < v.PricePerDay * counter);
                if (carSearch.Type != null)
                    cars.RemoveAll(v => carSearch.Type != v.TypeOfVehicle);
                if (carSearch.Seats != null)
                    cars.RemoveAll(v => carSearch.Seats != v.NumberOfSeats);
            });
            return Ok(cars);
        }

        [HttpPost]
        [Route("GetSearchedRentacars")]
        public async Task<Object> GetSearchedRentacars(RentacarSearch search)
        {
            List<Rentacar> rentacars = new List<Rentacar>();
            try
            {
                rentacars = _dataBase.Rentacars.Include(r => r.Branches).ThenInclude(b => b.Location).Include(r => r.Location).Include(r => r.ListOfVehicles).ThenInclude(v => v.DatesTaken).ToList();
            }
            catch
            {
                return BadRequest("Database Error");
            }
            await Task.Run(() =>
            {
                if (search.Name != null)
                    rentacars.RemoveAll(r => !r.NameOfService.ToLower().Contains(search.Name.ToLower()));
                if (search.City != null)
                    rentacars.RemoveAll(r => !r.Location.NameOfCity.ToLower().Contains(search.City.ToLower()));
                if (search.Address != null)
                    rentacars.RemoveAll(r => !r.Location.NameOfStreet.ToLower().Contains(search.Address.ToLower()));
                if (search.Number != null)
                    rentacars.RemoveAll(r => !r.Location.NumberInStreet.ToLower().Contains(search.Number.ToLower()));
                if (search.EndDate != null)
                    for (var d = DateTime.Parse(search.StartDate); d <= DateTime.Parse(search.EndDate); d = d.AddDays(1))
                    {
                        rentacars.RemoveAll(r => r.ListOfVehicles.ToList().Find(v => v.DatesTaken.ToList().Find(da => da.DateTime == d) != null) != null);
                    }
                else if (search.StartDate != null)
                    rentacars.RemoveAll(r => r.ListOfVehicles.ToList().Find(v => v.DatesTaken.ToList().Find(da => da.DateTime == DateTime.Parse(search.StartDate)) != null) != null);
            });
            return Ok(rentacars);
        }

        [HttpGet]
        [Route("GetRentacar")]
        public Object GetRentacar(string id)
        {
            int idd = int.Parse(id);
            var rentacars = _dataBase.Rentacars.Include(r => r.Location).Include(r => r.ListOfVehicles).ThenInclude(v => v.DatesTaken)
                .Include(r => r.ListOfRatings).Include(r => r.ListOfVehicles).ThenInclude(v => v.SpecialOfferDates).Include(r => r.ListOfVehicles).ThenInclude(v => v.ListOfRatings)
                .Include(r => r.ProfitList).Include(r => r.DiscountGroups).Include(r => r.Branches).ThenInclude(b => b.ListOfVehicles).Include(r => r.Branches).ThenInclude(b => b.Location).ToList();

            if (rentacars.Count == 0)
                return NotFound("No rentacar values found");

            var rent = rentacars.Find(r => r.Id == idd);

            if (rent == null)
                return NotFound("No rentacar with given id!");

            return Ok(rent);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("ModifyRentacarInfo")]
        public async Task<Object> ModifyRentacarInfo([FromBody] RentacarInfo rentacar)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _dataBase.Users.FindAsync(userId);

            if(user != null)
            {
                if(user.RentacarID != rentacar.Id)
                {
                    return BadRequest("Not allowed to make modifications on given Rent-A-Car service");
                }

                var res = await _dataBase.Rentacars.Include(r => r.Location).SingleOrDefaultAsync(obj => obj.Id == rentacar.Id);

                if(res == null)
                {
                    return NotFound("Given rentacar was not found!");
                }

                res.Image = rentacar.Image;
                res.Location.NameOfCity = rentacar.CityName;
                res.Location.NameOfStreet = rentacar.StreetName;
                res.Location.NumberInStreet = rentacar.Number;
                res.Location.GeoWidth = rentacar.GeoWidth;
                res.Location.GeoHeight = rentacar.GeoHeight;
                res.NameOfService = rentacar.NameOfService;
                res.DescriptionOfService = rentacar.Description;
                _dataBase.SaveChanges();

                return Ok();
            }
            else
            {
                return BadRequest("Not allowed to make modifications on Rent-A-Car service!");
            }
        }

        #region DiscountGroups
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("AddNewDiscountGroup")]
        public async Task<Object> AddNewDiscountGroup(DiscountGroupInfo group)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _dataBase.Users.FindAsync(userId);

            if (user == null)
                return Unauthorized();
            if (user.Role != "sys")
                return Unauthorized();

            var rent = _dataBase.Rentacars.Include(r => r.DiscountGroups).First(r => r.Id == int.Parse(group.ServiceId));

            if (rent == null)
                return BadRequest("No rentacar found");

            if (rent.DiscountGroups.ToList().Find(g => g.MinPoints == group.MinPoints && g.DiscountPercentage == group.DiscountPercentage) != null)
                return BadRequest("Can't have groups with same Minimum points and Percengate values!");

            if (rent.DiscountGroups.ToList().Find(g => g.GroupName == group.GroupName) != null)
                return BadRequest("Group with same name exist!");
            var disc = new DiscountGroup() { DiscountPercentage = group.DiscountPercentage, GroupName = group.GroupName, MinPoints = group.MinPoints };
            rent.DiscountGroups.Add(disc);

            _dataBase.SaveChanges();
            var ret = rent.DiscountGroups.ToList();
            ret.Sort(new DiscountGroupListSort());
            return Ok(ret);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("ModifyDiscountGroup")]
        public async Task<Object> ModifyDiscountGroup(DiscountGroupInfo group)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _dataBase.Users.FindAsync(userId);

            if (user == null)
                return Unauthorized();
            if (user.Role != "sys")
                return Unauthorized();

            var rent = _dataBase.Rentacars.Include(r => r.DiscountGroups).First(r => r.Id == int.Parse(group.ServiceId));

            if (rent == null)
                return BadRequest("No rentacar found");

            var discGr = rent.DiscountGroups.ToList().Find(g => g.Id == group.Id);

            if (discGr == null)
                return BadRequest("Group doesn't exist!");

            discGr.GroupName = group.GroupName;
            discGr.MinPoints = group.MinPoints;
            discGr.DiscountPercentage = group.DiscountPercentage;

            _dataBase.SaveChanges();
            var ret = rent.DiscountGroups.ToList();
            ret.Sort(new DiscountGroupListSort());
            return Ok(ret);
        }

        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("DeleteDiscountGroup")]
        public async Task<Object> DeleteDiscountGroup(string rid, string did)
        {
            var rentId = int.Parse(rid);
            var disId = int.Parse(did);

            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _dataBase.Users.FindAsync(userId);

            if (user == null)
                return Unauthorized();
            if (user.Role != "sys")
                return Unauthorized();

            var disc = _dataBase.DiscountGroups.Find(disId);
            if (disc == null)
                return BadRequest("No rentacar found");

            _dataBase.DiscountGroups.Remove(disc);
            _dataBase.SaveChanges();
            var rent = _dataBase.Rentacars.Include(r => r.DiscountGroups).ToList().Find(r => r.Id == rentId);
            var ret = rent.DiscountGroups.ToList();
            ret.Sort(new DiscountGroupListSort());
            return Ok(ret);

        }

        #endregion

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("AddNewBranch")]
        public async Task<Object> AddNewBranch([FromBody] BranchInfo branch)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _dataBase.Users.FindAsync(userId);

            if(user != null)
            {
                if (user.RentacarID != branch.RentacarId)
                {
                    return BadRequest("Not allowed to make modifications on given Rent-A-Car service");
                }

                var res = await _dataBase.Rentacars.Include(r => r.Branches).Include(r => r.ListOfVehicles).SingleOrDefaultAsync(obj => obj.Id == branch.RentacarId);

                if(res == null)
                {
                    return NotFound("Given rentacar was not found!");
                }

                var loc = new Location() { GeoHeight = branch.GeoHeight, GeoWidth = branch.GeoWidth, NameOfCity = branch.City, NameOfStreet = branch.City, NumberInStreet = branch.Number };
                var br = new Branch() { Location = loc, NameOfBranch = branch.BranchName, RentacarID = res.Id };
                res.Branches.Add(br);
                _dataBase.SaveChanges();
                return Ok(new { br });
            }
            else
            {
                return BadRequest("Not allowed to make modifications on Rent-A-Car service!");
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("ModifyBranch")]
        public async Task<Object> ModifyBranch([FromBody] BranchInfo branch)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _dataBase.Users.FindAsync(userId);

            if (user == null)
            {
                return BadRequest("Not allowed to make modifications on Rent-A-Car service!");
            }

            if (user.RentacarID != branch.RentacarId)
            {
                return BadRequest("Not allowed to make modifications on given Rent-A-Car service");
            }

            var res = await _dataBase.Branches.Include(b => b.Location).SingleOrDefaultAsync(obj => obj.Id == branch.BranchId);

            if(res == null)
            {
                return NotFound("Branch not found!");
            }

            res.NameOfBranch = branch.BranchName;
            res.Location.NameOfCity = branch.City;
            res.Location.NameOfStreet = branch.Address;
            res.Location.NumberInStreet = branch.Number;
            res.Location.GeoWidth = branch.GeoWidth;
            res.Location.GeoHeight = branch.GeoHeight;
            _dataBase.SaveChanges();
            return Ok();
        }

        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("DeleteBranch")]
        public async Task<Object> DeleteBranch( string bid, string rid)
        {
            int rentId = int.Parse(rid);
            int branId = int.Parse(bid);
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _dataBase.Users.FindAsync(userId);

            if (user == null)
            {
                return BadRequest("Not allowed to make modifications on Rent-A-Car service!");
            }

            if (user.RentacarID != rentId)
            {
                return BadRequest("Not allowed to make modifications on given Rent-A-Car service");
            }

            var res = await _dataBase.Branches.Include(b => b.Location).Include(b => b.ListOfVehicles).SingleOrDefaultAsync(obj => obj.Id == branId);

            if (res == null)
            {
                return NotFound("Branch not found!");
            }

            if (res.ListOfVehicles.ToList().Find(v => v.IsRented) != null)
                return BadRequest("Can't delete Branch! Vehicle from branch is rented!");

            _dataBase.Branches.Remove(res);
            _dataBase.SaveChanges();
            return Ok();
        }

        [HttpGet]
        [Route("SearchBranches")]
        public async Task<Object> SearchBranches(string rid, string bname, string city)
        {
            var branches = new List<Branch>();
            await Task.Run(() =>
            {
                branches = _dataBase.Branches.Include(b => b.Location)
                            .Include(b => b.ListOfVehicles).ThenInclude(v => v.SpecialOfferDates)
                            .Include(b => b.ListOfVehicles).ThenInclude(v => v.DatesTaken)
                            .Include(b => b.ListOfVehicles).ThenInclude(v => v.ListOfRatings).ToList().FindAll(b => b.Id == int.Parse(rid));

                if (bname != null)
                    branches.RemoveAll(b => !b.NameOfBranch.ToLower().Contains(bname.ToLower()));
                if (city != null)
                    branches.RemoveAll(b => !b.Location.NameOfCity.ToLower().Contains(city.ToLower()));
            });
            return Ok(branches);
        }

        [HttpGet]
        [Route("GetBranch")]
        public async Task<Object> GetBranch(string bid, string rid)
        {
            int rentId = int.Parse(rid);
            int branId = int.Parse(bid);

            var res = await _dataBase.Branches.Include(b => b.Location).Include(b => b.ListOfVehicles).SingleOrDefaultAsync(obj => obj.Id == branId);

            if (res == null)
            {
                return NotFound("Branch not found!");
            }


            return Ok(res);
        }

        [HttpGet]
        [Route("GetRentacarBranches")]
        public async Task<Object> GetRentacarBranches(string rid)
        {
            var branches = new List<Branch>();
            await Task.Run(() =>
            {
                branches = _dataBase.Branches.Include(b => b.Location)
                            .Include(b => b.ListOfVehicles).ThenInclude(v => v.SpecialOfferDates)
                            .Include(b => b.ListOfVehicles).ThenInclude(v => v.DatesTaken)
                            .Include(b => b.ListOfVehicles).ThenInclude(v => v.ListOfRatings).ToList().FindAll(b => b.RentacarID == int.Parse(rid));
            });
            return Ok(branches);
        }

        #region Vehicles

        [HttpGet]
        [Route("GetAllVehicles")]
        public async Task<Object> GetAllVehicles()
        {
            var vehicles = new List<Vehicle>();
            await Task.Run(() =>
            {
                vehicles = _dataBase.Vehicles.Include(v => v.ListOfRatings).Include(v => v.SpecialOfferDates)
                .Include(v => v.DatesTaken).ToList();
            });

            return Ok(vehicles);
        }

        [HttpGet]
        [Route("GetRentacarVehicles")]
        public async Task<Object> GetRentacarVehicles(string rid)
        {
            var vehicles = new List<Vehicle>();
            await Task.Run(() =>
            {
                vehicles = _dataBase.Vehicles.Include(v => v.ListOfRatings).Include(v => v.SpecialOfferDates)
                .Include(v => v.DatesTaken).ToList().FindAll(v => v.RentacarID == int.Parse(rid));
            });
            return Ok(vehicles);
        }

        [HttpGet]
        [Route("GetBranchVehicles")]
        public async Task<Object> GetBranchVehicles(string bid)
        {
            var vehicles = new List<Vehicle>();
            await Task.Run(() =>
            {
                vehicles = _dataBase.Vehicles.Include(v => v.ListOfRatings).Include(v => v.SpecialOfferDates)
                .Include(v => v.DatesTaken).ToList().FindAll(v => v.BranchID == int.Parse(bid));
            });
            return Ok(vehicles);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("AddNewVehicle")]
        public async Task<Object> AddNewVehicle([FromBody] VehicleRequest vehicle)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _dataBase.Users.FindAsync(userId);

            if (user != null)
            {
                if (user.RentacarID != vehicle.RentacarID)
                {
                    return BadRequest("Not allowed to make modifications on given Rent-A-Car service");
                }

                var rentRes = await _dataBase.Rentacars.Include(r => r.ListOfVehicles).SingleOrDefaultAsync(obj => obj.Id == vehicle.RentacarID);
                var branchRes = await _dataBase.Branches.Include(b => b.ListOfVehicles).SingleOrDefaultAsync(obj => obj.Id == vehicle.BranchID);
                if (rentRes == null)
                {
                    return NotFound("Given rentacar was not found!");
                }
                else if(branchRes == null)
                {
                    return NotFound("Given branch was not found!");
                }
                var car = new Vehicle()
                {
                    RentacarID = rentRes.Id,
                    BranchID = branchRes.Id,
                    CanBeRented = vehicle.CanBeRented,
                    MarkOfVehicle = vehicle.MarkOfVehicle,
                    ModelOfVehicle = vehicle.ModelOfVehicle,
                    TypeOfVehicle = vehicle.TypeOfVehicle,
                    Name = vehicle.Name,
                    NumberOfSeats = vehicle.NumberOfSeats,
                    YearMade = vehicle.YearMade,
                    PricePerDay = vehicle.PricePerDay,
                    AverageRatingOfVehicle = 0
                };
                //branchRes.ListOfVehicles.Add(car);
                
                rentRes.ListOfVehicles.Add(car);
                _dataBase.SaveChanges();

                var br = await _dataBase.Branches.Include(b => b.ListOfVehicles).FirstAsync(bb => bb.Id == branchRes.Id);

                return Ok(car);
            }
            else
            {
                return BadRequest("Not allowed to make modifications on Rent-A-Car service!");
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("ModifyVehicle")]
        public async Task<Object> ModifyVehicle([FromBody] VehicleRequest vehicle)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _dataBase.Users.FindAsync(userId);

            if (user == null)
                return BadRequest("Not allowed to make modifications on Rent-A-Car service!");

            if (user.RentacarID != vehicle.RentacarID)
                return BadRequest("Not allowed to make modifications on given Rent-A-Car service");

            var res = await _dataBase.Vehicles.SingleOrDefaultAsync(obj => obj.Id == vehicle.ID);

            if(res == null)
                return NotFound("Vehicle doesn't exist!");

            res.Name = vehicle.Name;
            res.MarkOfVehicle = vehicle.MarkOfVehicle;
            res.ModelOfVehicle = vehicle.ModelOfVehicle;
            res.NumberOfSeats = vehicle.NumberOfSeats;
            res.PricePerDay = vehicle.PricePerDay;
            res.TypeOfVehicle = vehicle.TypeOfVehicle;
            res.YearMade = vehicle.YearMade;
            res.CanBeRented = vehicle.CanBeRented;
            res.SpecialDiscount = vehicle.SpecialOffer;
            res.SpecialOfferDates.Clear();
            vehicle.SpecialOfferDates.ForEach(s => res.SpecialOfferDates.Add(new Date() { DateTime = DateTime.Parse(s) }));
            
            _dataBase.SaveChanges();

            return Ok(res);
        }

        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("DeleteVehicle")]
        public async Task<Object> DeleteVehicle(string vid, string rid)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            int rentId = int.Parse(rid);
            int carId = int.Parse(vid);
            var user = await _dataBase.Users.FindAsync(userId);

            if (user == null)
                return BadRequest("Not allowed to make modifications on Rent-A-Car service!");
            

            if (user.RentacarID != rentId)
                return BadRequest("Not allowed to make modifications on given Rent-A-Car service");
            

            var res = await _dataBase.Vehicles.SingleOrDefaultAsync(obj => obj.Id == carId);

            if (res == null)
                return NotFound("Vehicle doesn't exist!");

            _dataBase.Vehicles.Remove(res);
            _dataBase.SaveChanges();
            return Ok();
        }
        #endregion

        [HttpPost]
        [Route("SalesGraph")]
        public async Task<Object> SalesGraph(IdModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _dataBase.Users.FindAsync(userId);


            if (user == null || user.Role != "rcsa")
                return BadRequest("Not allowed to make modifications on Rent-A-Car service!");

            List<Profit> profit = new List<Profit>();
            int[] salesDaily = new int[24];
            int[] salesWeekly = new int[7];
            int[] salestMonthly = new int[12];

            for (int i = 0; i < 24; i++)
                salesDaily[i] = 0;

            for (int i = 0; i < 7; i++)
                salesWeekly[i] = 0;

            for (int i = 0; i < 12; i++)
                salestMonthly[i] = 0;

            foreach (Profit t in _dataBase.Profits)
            {
                if (t.RentacarID == user.RentacarID)
                {
                    profit.Add(t);
                }
            }

            if (model.Id == 1)
            {
                foreach (Profit t in profit)
                {
                    if (DateTime.Parse(model.Date).Date == t.DateTransactionWasMade.Date)
                    {
                        if (model.Type == "sales")
                            salesDaily[t.DateTransactionWasMade.Hour]++;
                        else
                            salesDaily[t.DateTransactionWasMade.Hour] += (int)t.EarnedMoney;
                    }
                }
            }
            else if (model.Id == 2)
            {
                foreach (Profit t in profit)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        DateTime date = DateTime.Parse(model.Date);
                        date = date.AddDays(i);

                        if (date.Date == t.DateTransactionWasMade.Date)
                        {
                            if (model.Type == "sales")
                                salesWeekly[i]++;
                            else
                                salesWeekly[i] += (int)t.EarnedMoney;
                        }
                    }
                }
            }
            else if (model.Id == 3)
            {
                foreach (Profit t in profit)
                {
                    if (Int32.Parse(model.Date) == t.DateTransactionWasMade.Month)
                    {
                        if (model.Type == "sales")
                            salestMonthly[t.DateTransactionWasMade.Month - 1]++;
                        else
                            salestMonthly[t.DateTransactionWasMade.Month - 1] += (int)t.EarnedMoney;
                    }
                }
            }

            if (model.Id == 1)
                return Ok(salesDaily);
            else if (model.Id == 2)
                return Ok(salesWeekly);
            else if (model.Id == 3)
                return Ok(salestMonthly);
            else
                return NoContent();
        }

    }
}