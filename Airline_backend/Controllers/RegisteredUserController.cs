using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BackEnd.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisteredUserController : ControllerBase
    {
        private readonly ApplicationSettings _appSettings;
        private GoogleApiTokenInfo googleApiTokenInfo;
        private AuthenticationContext _dataBase;
        private UserManager<User> _userManager;
        private bool verifyToken = false;
        private static string randomString;
        public RegisteredUserController(UserManager<User> userManager, IOptions<ApplicationSettings> appSettings,
            AuthenticationContext context)
        {
            _userManager = userManager;
            _appSettings = appSettings.Value;
            _dataBase = context;
            
        }

        [HttpGet]
        [Route("AddPhoto")]
        public async Task<Object> AddPhoto()
        {
            var arsadmin = new User();

            await Task.Run(() =>
            {
                arsadmin = _dataBase.Users.SingleOrDefault(a => a.UserName == "arsadmin");
                arsadmin.Image = "userAvatar.png";

                _dataBase.SaveChanges();
            });

            return Ok();
        }

        [HttpGet]
        [Route("RegisterSys")]
        public async Task<Object> RegisterSys()
        {
            var sys = new User("sys")
            {
                UserName = "dzeksiStar",
                Email = "admin@gmail.com",
                EmailConfirmed = true,
                Name = "Mihajlo",
                Lastname = "Dzever",
                Role = "sys"
            };

            var rcsa = new User("rcsa")
            {
                UserName = "rcsadmin",
                Email = "rcsadmin@gmail.com",
                EmailConfirmed = true,
                Name = "Mihajlo",
                Lastname = "Dzever",
                RentacarID = 1,
                //Rentacar = await _dataBase.Rentacars.FindAsync(1),
                Role = "rcsa"
            };

            var arsa = new User("arsa")
            {
                UserName = "arsadmin",
                Email = "arsadmin@gmail.com",
                EmailConfirmed = true,
                Name = "Zeljka",
                Lastname = "Matovic",
                ServiceID = 1,
                Airline = await _dataBase.Airlines.FindAsync(1),
                Role = "arsa"
            };
           

            try
            {
                
                await _userManager.CreateAsync(rcsa, "12345678");
                await _userManager.CreateAsync(arsa, "12345678");
                var result = await _userManager.CreateAsync(sys, "12345678");
                return Ok(result);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        [HttpPost]
        [Route("Register")]
        //POST : /api/RegisteredUser/Register
        public async Task<Object> PostRegisteredUser(RegisteredUserModel model)
        {
            var registeredUser = new User("ru")
            {
                UserName = model.Username,
                Email = model.Email,
                Name = model.Name,
                Lastname = model.Lastname,
                PhoneNumber = model.PhoneNumber,
                Image = "userAvatar.png",
                Role = "ru",
                PassportNumber = model.Passport,
                City = model.City
            };
            try
            {
                if (await _userManager.FindByEmailAsync(registeredUser.Email) != null)
                    return BadRequest("User with same email exists");
                if (await _userManager.FindByNameAsync(registeredUser.UserName) != null)
                    return BadRequest("User with same username exists");
                var result = await _userManager.CreateAsync(registeredUser, model.Password);
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(Logindata model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserID",user.UserName.ToString()),
                        new Claim("Roles", user.Role)
                    }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Ok(new { token, user });
            }
            else
            {
                return BadRequest("Invalid password or email");
            }
        }
        
        [HttpPost]
        [Route("SocialLogin")]
        // POST: api/<controller>/SocialLogin
        public async Task<IActionResult> SocialLogin([FromBody]SocialLogin model)
        {
            VerifyToken(model.IdToken);
            if (verifyToken)
            {
                var user = await _userManager.FindByEmailAsync(googleApiTokenInfo.email);
                if (user != null)
                {
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim("UserID",user.UserName.ToString()),
                            new Claim("Roles", "ru")
                        }),
                        Expires = DateTime.UtcNow.AddDays(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                    var token = tokenHandler.WriteToken(securityToken);
                    return Ok(new { token, user });
                   /* return Ok(new
                    {
                        token,
                        username = user.UserName,
                        name = user.Name,
                        lastname = user.Lastname,
                        email = user.Email,
                        role = user.Role,
                        picture = user.Image,
                        phone = user.PhoneNumber,
                        friends = user.Friends,
                        friendsRequest = user.FriendRequests,
                        discountPoints = user.DiscountPoints,
                        reservedTickets = user.ReservedTickets,
                        ticketHistory = user.TicketHistory,
                        reservedCars = user.ReservedCars,
                        reservedCarsHistory = user.ReservedCarsHistory,
                        passportNumber = user.PassportNumber
                    });*/
                }
                else
                {
                    try
                    {
                        IdentityResult result;
                        User registeredUser = null;
                        do
                        {
                            RandomString(5);
                            registeredUser = new User("ru")
                            {
                                UserName = googleApiTokenInfo.given_name + "_" + randomString,
                                Email = googleApiTokenInfo.email,
                                Name = googleApiTokenInfo.given_name,
                                Lastname = googleApiTokenInfo.family_name,
                                EmailConfirmed = googleApiTokenInfo.email_verified == "true" ? true : false,
                                PhoneNumber = "",
                                Image = googleApiTokenInfo.picture
                            };
                            result = await _userManager.CreateAsync(registeredUser);
                        } while (result.Errors.ToList().Count > 0 && result.Errors.ToList()[0].Code == "InvalidUserName");

                        user = await _userManager.FindByEmailAsync(googleApiTokenInfo.email);
                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(new Claim[]
                            {
                                new Claim("UserID",user.UserName.ToString()),
                                new Claim("Roles", "ru")
                            }),
                            Expires = DateTime.UtcNow.AddDays(1),
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
                        };
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                        var token = tokenHandler.WriteToken(securityToken);
                        return Ok(user);
                        /*return Ok(new {
                            token,
                            username = user.UserName,
                            name = user.Name,
                            lastname = user.Lastname,
                            email = user.Email,
                            role = user.Role,
                            picture = user.Image,
                            phone = user.PhoneNumber,
                            friends = user.Friends,
                            friendsRequest = user.FriendRequests,
                            discountPoints = user.DiscountPoints,
                            reservedTickets = user.ReservedTickets,
                            ticketHistory = user.TicketHistory,
                            reservedCars = user.ReservedCars,
                            reservedCarsHistory = user.ReservedCarsHistory,
                            passportNumber = user.PassportNumber
                        });*/
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
               
            return BadRequest(new { message = "Something went wrong! Try again later" });
        }

        private const string GoogleApiTokenInfoUrl = "https://www.googleapis.com/oauth2/v3/tokeninfo?id_token={0}";

        public async void VerifyToken(string providerToken)
        {
            await Task.Run(() =>
            {
                var httpClient = new HttpClient();
                var requestUri = new Uri(string.Format(GoogleApiTokenInfoUrl, providerToken));

                HttpResponseMessage httpResponseMessage = new HttpResponseMessage();

                try
                {
                    httpResponseMessage = httpClient.GetAsync(requestUri).Result;
                }
                catch (Exception)
                {
                    verifyToken = false;
                }

                if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
                {
                    verifyToken = false;
                }

                var response = httpResponseMessage.Content.ReadAsStringAsync().Result;
                googleApiTokenInfo = JsonConvert.DeserializeObject<GoogleApiTokenInfo>(response);

                verifyToken = true;
            });
        }

        private static Random random = new Random();
        public async static void RandomString(int length)
        {
            await Task.Run(() =>
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                randomString = new string(Enumerable.Repeat(chars, length)
                  .Select(s => s[random.Next(s.Length)]).ToArray());
            });
        }

    }
}