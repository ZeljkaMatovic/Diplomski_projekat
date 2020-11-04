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

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemAdminController : ControllerBase
    {
        private AuthenticationContext _dataBase;
        private UserManager<User> _userManager;

        public SystemAdminController(AuthenticationContext dataBase, UserManager<User> userManager)
        {
            _dataBase = dataBase;
            _userManager = userManager;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("NewRentacar")]
        public async Task<Object> NewRentacar(Rentacar rentacar)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _dataBase.Users.FindAsync(userId);

            if (user == null)
                return Unauthorized("");
            if (user.Role != "sys")
                return Unauthorized("");
            try
            {
                var loc = new Location()
                {
                    NameOfCity = rentacar.Location.NameOfCity,
                    NameOfStreet = rentacar.Location.NameOfStreet,
                    NumberInStreet = rentacar.Location.NumberInStreet,
                    GeoHeight = rentacar.Location.GeoHeight,
                    GeoWidth = rentacar.Location.GeoWidth
                };
                _dataBase.Locations.Add(loc);
                var dg = new DiscountGroup()
                {
                    GroupName = rentacar.DiscountGroups.ToList()[0].GroupName,
                    MinPoints = rentacar.DiscountGroups.ToList()[0].MinPoints,
                    DiscountPercentage = rentacar.DiscountGroups.ToList()[0].DiscountPercentage
                };
                //_dataBase.DiscountGroups.Add(dg);
                _dataBase.SaveChanges();
                var rent = new Rentacar()
                {
                    NameOfService = rentacar.NameOfService,
                    DescriptionOfService = "",
                    Image = "",
                    LocationID = loc.Id
                };
                rentacar.LocationID = rentacar.Location.Id;
                _dataBase.Rentacars.Add(rent);
                rent.DiscountGroups.Add(dg);
                _dataBase.SaveChanges();
                var test = _dataBase.Rentacars.Include(r => r.DiscountGroups).Include(r => r.Location);
                return Ok(rentacar);
            }
            catch(Exception e)
            {
                return BadRequest(e);
            }
        }

        //[HttpPost]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Route("NewAirline")]
        //public async Task<Object> NewAirline(Airline airline)
        //{
        //    string userId = User.Claims.First(c => c.Type == "UserID").Value;
        //    var user = await _dataBase.Users.FindAsync(userId);

        //    if (user == null)
        //        return Unauthorized("");
        //    if (user.Role != "sys")
        //        return Unauthorized("");
        //    try
        //    {
        //        var loc = new Location()
        //        {
        //            NameOfCity = airline.Location.NameOfCity,
        //            NameOfStreet = airline.Location.NameOfStreet,
        //            NumberInStreet = airline.Location.NumberInStreet,
        //            GeoHeight = airline.Location.GeoHeight,
        //            GeoWidth = airline.Location.GeoWidth
        //        };
        //        _dataBase.Locations.Add(loc);
        //        var dg = new DiscountGroup()
        //        {
        //            GroupName = airline.DiscountGroups.ToList()[0].GroupName,
        //            MinPoints = airline.DiscountGroups.ToList()[0].MinPoints,
        //            DiscountPercentage = airline.DiscountGroups.ToList()[0].DiscountPercentage
        //        };
        //        //_dataBase.DiscountGroups.Add(dg);
        //        _dataBase.SaveChanges();
        //        var air = new Airline()
        //        {
        //            NameOfAirline = airline.NameOfAirline,
        //            DescriptionOfAirline = airline.DescriptionOfAirline,
        //            Image = "",
        //            LocationID = loc.Id
        //        };
        //        _dataBase.Airlines.Add(air);
        //        air.DiscountGroups.Add(dg);
        //        _dataBase.SaveChanges();
        //        return Ok(air);
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e);
        //    }
        //}

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("NewAdmin")]
        public async Task<Object> NewAdmin(AdminForm newUser)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _dataBase.Users.FindAsync(userId);

            if (user == null)
                return Unauthorized("");
            if (user.Role != "sys")
                return Unauthorized("");
            if (await _userManager.FindByEmailAsync(newUser.Email) != null)
                return BadRequest("User with same email exists");
            if (await _userManager.FindByNameAsync(newUser.UserName) != null)
                return BadRequest("User with same username exists");

            try
            {
                var userNew = new User(newUser.Role)
                {
                    UserName = newUser.UserName,
                    Name = newUser.Name,
                    Lastname = newUser.Lastname,
                    Email = newUser.Email,
                    City = newUser.City,
                    PhoneNumber = newUser.Phone
                };

                if (newUser.Role == "rcsa")
                {
                    userNew.RentacarID = newUser.RentacarID;
                }
                else if(newUser.Role == "arsa")
                {
                    userNew.ServiceID = newUser.ServiceID;
                }
                _dataBase.Users.Add(userNew);
                _dataBase.SaveChanges();
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("FindServiceByName")]
        public async Task<Object> FindServiceByName(string name)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _dataBase.Users.FindAsync(userId);

            if (user == null)
                return Unauthorized("");
            if (user.Role != "sys")
                return Unauthorized("");
            var rents = _dataBase.Rentacars.ToList().FindAll(r => r.NameOfService.ToLower().Contains(name.ToLower()));
            //var airs = _dataBase.Airlines.ToList().FindAll(r => r.NameOfAirline.ToLower().Contains(name.ToLower()));

            return Ok(new { rentacars = rents });
        }

        //[HttpGet]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Route("FindDiscountGroupsByName")]
        //public async Task<Object> FindDiscountGroupsByName(string sid, string ser, string name)
        //{
        //    string userId = User.Claims.First(c => c.Type == "UserID").Value;
        //    var user = await _dataBase.Users.FindAsync(userId);

        //    if (user == null)
        //        return Unauthorized("");
        //    if (user.Role != "sys")
        //        return Unauthorized("");

        //    var disc = ser == "airline" ? _dataBase.Airlines.Include(a => a.DiscountGroups).ToList().Find(a => a.Id == int.Parse(sid)).DiscountGroups.ToList() :
        //                                      _dataBase.Rentacars.Include(r => r.DiscountGroups).ToList().Find(r => r.Id == int.Parse(sid)).DiscountGroups.ToList();
        //    disc.RemoveAll(g => !g.GroupName.ToLower().Contains(name.ToLower()));

        //    return Ok(disc);
        //}

    }
}