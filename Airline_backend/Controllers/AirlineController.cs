using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BackEnd.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using BackEnd.Classes;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.VisualBasic;
using System.Collections.Specialized;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AirlineController : ControllerBase
    {
        private AuthenticationContext _dataBase;

        public AirlineController(AuthenticationContext context)
        {
            _dataBase = context;
        }

        [HttpGet]
        [Route("GetAllAirlines")]
        public async Task<Object> GetAllAirlines()
        {
            var airlines = new List<Airline>();
            await Task.Run(() =>
            {
                airlines = _dataBase.Airlines.Include(a => a.Location).Include(a => a.Destinations)
                    .Include(a => a.Planes).ThenInclude(p => p.EconomyRows).Include(a => a.Planes).ThenInclude(p => p.BusinessRows)
                    .Include(a => a.DiscountGroups).Include(a => a.AllRatings).Include(a => a.ListOfFlights)
                    .ThenInclude(t => t.ListOfTickets).Include(a => a.ListOfFlights).ThenInclude(r => r.AllRatings).ToList();
            });

            if (airlines.Count == 0)
            {
                return NotFound("No airlines values found");
            }

             return Ok(airlines);
        }

        [HttpPost]
        [Route("FindAirline")]
        public async Task<Object> FindAirline(IdModel idMod)
        {
            var airlines = new List<Airline>();
            await Task.Run(() =>
            {
                airlines = _dataBase.Airlines.Include(a => a.Location).Include(a => a.Destinations).Include(a => a.DiscountGroups)
                .Include(a => a.Planes).ThenInclude(p => p.EconomyRows).Include(a => a.Planes).ThenInclude(p => p.BusinessRows)
                .Include(a => a.DiscountGroups).ToList();
            });

            Airline airline = new Airline();
            foreach (Airline a in airlines)
            {
                if (a.Id == idMod.Id)
                    airline = a;
            }

            if (airline == null)
                return NotFound("Airline with that id not found!");

            return Ok(airline);
        }

        [HttpPost]
        [Route("FindAirlineByName")]
        public async Task<Object> FindAirlineByName(IdModel idMod)
        {
            var airline = new Airline();
            await Task.Run(() =>
            {
                airline = _dataBase.Airlines.SingleOrDefault(a => a.NameOfAirline == idMod.Name);
            });

            if (airline == null)
                return NotFound("Airline with that id not found!");

            return Ok(airline);
        }

        [HttpPost]
        [Route("FindAirlineFlights")]
        public async Task<Object> FindAirlineFlights(IdModel idMod)
        {
            var flights = new List<Flight>();

            await Task.Run(() =>
            {
                foreach (Flight flight in _dataBase.Flights)
                {
                    if (flight.NameOfAirline == idMod.Name)
                    {
                        flights.Add(flight);
                    }
                }
            });
            return Ok(flights);
        }

        [HttpPost]
        [Route("FindFlightTickets")]
        public async Task<Object> FindFlightTickets(IdModel idMod)
        {
            var tickets = new List<Ticket>();

            await Task.Run(() =>
            {
                foreach (Ticket t in _dataBase.Ticket)
                {
                    if (t.FlightID == idMod.Id)
                    {
                        tickets.Add(t);
                    }
                }
            });
            return Ok(tickets);

        }

        [HttpPost]
        [Route("GetAirlineDestinations")]
        public async Task<Object> GetAirlineDestinations(IdModel idMod)
        {
            var destinations = new List<Destination>();

            await Task.Run(() =>
            {
                foreach (var d in _dataBase.Destinations)
                {
                    if (d.AirlineId == idMod.Id)
                    {
                        destinations.Add(d);
                    }
                }
            });
            return Ok(destinations);
        }

        [HttpGet]
        [Route("GetAllDestinations")]
        public async Task<Object> GetAllDestinations()
        {
            var destinations = new List<Destination>();

            await Task.Run(() =>
            {
                foreach (var d in _dataBase.Destinations)
                {
                    destinations.Add(d);

                }
            });
            return Ok(destinations);
        }

        [HttpPost]
        [Route("AddNewFlight")]
        public async Task<Object> AddNewFlight(FlightModel model)
        {
            var flight = new Flight
            {
                DestinationFrom = model.DestinationFrom,
                DestinationTo = model.DestinationTo,
                DepartingDateTime = DateTime.Parse(model.DepartureDate),
                ReturningDateTime = DateTime.Parse(model.LandingDate),
                TimeOfFlight = DateTime.Parse(model.Duration),
                Duration = DateTime.Parse(model.Length),
                NumberOfChangeovers = model.NumberOfChangeover,
                LocationsChangeover = model.Changeovers,
                TicketPrice = model.TicketPrice,
                NameOfAirline = model.NameOfAirline,
                BusinessPrice = model.TicketPrice + (model.TicketPrice * 80 / 100)
            };

            var airline = new Airline();
            await Task.Run(() =>
            {
                airline = _dataBase.Airlines.SingleOrDefault(a => a.NameOfAirline == model.NameOfAirline);
            });

            try
            {
                airline.ListOfFlights.Add(flight);
                _dataBase.SaveChanges();
                return Ok(flight);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        [HttpPost]
        [Route("SearchFlights")]
        public async Task<Object> SearchFlights(ReservationModel model)
        {
            var flights = new List<Flight>();
            List<Flight> temp = new List<Flight>();

            await Task.Run(() =>
            {
                foreach (Flight f in _dataBase.Flights)
                {
                    temp.Add(f);
                }

                if (model.FlightType == "one-way")
                {
                    foreach (Flight f in temp)
                    {
                        if (f.DestinationFrom == model.DestinationFrom && f.DestinationTo == model.DestinationTo
                            && f.DepartingDateTime.Date == DateTime.Parse(model.DepartureDate).Date)
                        {
                            flights.Add(f);
                        }
                    }
                }
                else if (model.FlightType == "roundtrip")
                {
                    foreach (Flight f in temp)
                    {
                        if (f.DestinationFrom == model.DestinationFrom && f.DestinationTo == model.DestinationTo
                            && f.DepartingDateTime.Date == DateTime.Parse(model.DepartureDate).Date)
                        {
                            flights.Add(f);
                        }
                        if (f.DestinationFrom == model.DestinationTo && f.DestinationTo == model.DestinationFrom
                            && f.DepartingDateTime.Date == DateTime.Parse(model.ReturnDate).Date)
                        {
                            flights.Add(f);
                        }
                    }
                }
                else if (model.FlightType == "multiCity")
                {
                    foreach (Flight f in temp)
                    {
                        if (f.DestinationFrom == model.DestinationFrom && f.DestinationTo == model.DestinationTo
                            && f.DepartingDateTime.Date == DateTime.Parse(model.DepartureDate).Date)
                        {
                            flights.Add(f);
                        }
                        if (f.DestinationFrom == model.MultiDestinationFrom && f.DestinationTo == model.MultiDestinationTo
                            && f.DepartingDateTime.Date == DateTime.Parse(model.ReturnDate).Date)
                        {
                            flights.Add(f);
                        }
                    }
                }
            });
            return Ok(flights);
        }

        [HttpPost]
        [Route("FindFlight")]
        public async Task<Object> FindFlight(IdModel model)
        {
            var flight = await _dataBase.Flights.FindAsync(model.Id);
            return Ok(flight);
        }

        [HttpPost]
        [Route("FindPlane")]
        public async Task<Object> FindPlane(IdModel model)
        {
            var plane = new Plane();

            await Task.Run(() =>
            {
                plane = _dataBase.Planes.SingleOrDefault(p => p.AirlineId == model.Id);
            });

            return Ok(plane);
        }

        [HttpPost]
        [Route("GetBusRows")]
        public async Task<Object> GetBusRows(IdModel model)
        {
            List<BusRow> busRows = new List<BusRow>();

            await Task.Run(() =>
            {
                foreach (BusRow br in _dataBase.BusRows)
                {
                    if (br.PlaneId == model.Id)
                    {
                        busRows.Add(br);
                    }
                }
            });

            return Ok(busRows);
        }

        [HttpPost]
        [Route("GetEcoRows")]
        public async Task<Object> GetEcoRows(IdModel model)
        {
            List<EcoRow> ecoRows = new List<EcoRow>();

            await Task.Run(() =>
            {
                foreach (EcoRow er in _dataBase.EcoRows)
                {
                    if (er.PlaneId == model.Id)
                    {
                        ecoRows.Add(er);
                    }
                }
            });

            return Ok(ecoRows);
        }

        [HttpPost]
        [Route("GetSeats")]
        public async Task<Object> GetSeats(IdModel model)
        {
            var seats = new List<Seat>();

            await Task.Run(() =>
            {
                foreach (Seat s in _dataBase.Seats)
                {
                    if (s.RowId == model.Id)
                    {
                        seats.Add(s);
                    }
                }
            });
            return Ok(seats);
        }

        [HttpGet]
        [Route("CountTickets")]
        public async Task<Object> CountTickets()
        {
            int counter = 0;

            await Task.Run(() =>
            {
                foreach (Ticket t in _dataBase.Ticket)
                    counter++;
            });

            return Ok(counter);
        }

        [HttpPost]
        [Route("ReserveTicket")]
        public async Task<object> ReserveTicket(ReserveFlightModel model)
        {
            using (var transaction = _dataBase.Database.BeginTransaction())
            {
                var flight1 = await _dataBase.Flights.FindAsync(model.IdFlight1);
                var flight2 = new Flight();
                var user = new User();
                var ticket1 = new Ticket();
                var ticket2 = new Ticket();

                await Task.Run(() =>
                {
                    user = _dataBase.Users.SingleOrDefault(u => u.Email == model.UserEmail);

                    ticket1.DestinationFrom = flight1.DestinationFrom;
                    ticket1.DestinationTo = flight1.DestinationTo;
                    ticket1.DateAndTime = flight1.DepartingDateTime;
                    ticket1.Seat = model.Seat1;
                    if (model.FlightClass == "Economy")
                        ticket1.TotalPrice = flight1.TicketPrice;
                    else
                        ticket1.TotalPrice = flight1.BusinessPrice;
                    ticket1.NameOfCompany = flight1.NameOfAirline;
                    ticket1.DateOfReservation = DateTime.Now;
                    ticket1.User = user.Email;
                    ticket1.Passport = model.Passport;
                    ticket1.FlightID = flight1.Id;
                    ticket1.Type = "Regular";

                    var airline1 = _dataBase.Airlines.SingleOrDefault(a => a.NameOfAirline == flight1.NameOfAirline);
                });

                var airline2 = new Airline();
                if (model.IdFlight2 != 0)
                {
                    flight2 = await _dataBase.Flights.FindAsync(model.IdFlight2);
                    airline2 = _dataBase.Airlines.SingleOrDefault(a => a.NameOfAirline == flight2.NameOfAirline);
                }

                var seat1 = new Seat();
                await Task.Run(() =>
                {
                    seat1 = _dataBase.Seats.SingleOrDefault(s => s.FullId == ticket1.Seat);
                });

                if (seat1.Class == "seat-white")
                {
                    seat1.Class = "seat-red";
                }
                else
                {
                    return NotFound();
                }


                if (model.IdFlight2 != 0)
                {
                    var seat2 = new Seat();
                    await Task.Run(() =>
                    {
                        ticket2.DestinationFrom = flight2.DestinationFrom;
                        ticket2.DestinationTo = flight2.DestinationTo;
                        ticket2.DateAndTime = flight2.DepartingDateTime;
                        ticket2.Seat = model.Seat2;
                        if (model.FlightClass == "Economy")
                            ticket2.TotalPrice = flight2.TicketPrice;
                        else
                            ticket2.TotalPrice = flight2.BusinessPrice;
                        ticket2.NameOfCompany = flight2.NameOfAirline;
                        ticket2.DateOfReservation = DateTime.Now;
                        ticket2.User = user.Email;
                        ticket2.Passport = model.Passport;
                        ticket2.FlightID = flight2.Id;
                        ticket2.Type = "Regular";

                        seat2 = _dataBase.Seats.SingleOrDefault(s => s.FullId == ticket2.Seat);
                    });

                    if (seat2.Class == "seat-white")
                    {
                        //seat2.Disabled = true;
                        seat2.Class = "seat-red";
                    }
                    else
                    {
                        return NotFound();
                    }
                }

                if (model.ListOfPassengers.Count != 0)
                {
                    for (int i = 0; i < model.ListOfPassengers.Count; i++)
                    {
                        var passenger = new Passengers();
                        passenger.FirstName = model.ListOfPassengers[i].FirstName;
                        passenger.LastName = model.ListOfPassengers[i].LastName;
                        passenger.Email = model.ListOfPassengers[i].Email;
                        passenger.Passport = model.ListOfPassengers[i].Passport;
                        passenger.Seat = model.ListOfPassengers[i].Seat;
                        passenger.InviteUsername = user.Email;
                        passenger.EmailConfirmed = false;

                        var seatPass = new Seat();
                        await Task.Run(() =>
                        {
                            seatPass = _dataBase.Seats.SingleOrDefault(s => s.FullId == model.ListOfPassengers[i].Seat);
                        });

                        if (seatPass.Class == "seat-white")
                        {
                            seatPass.Class = "seat-red";
                        }
                        else
                        {
                            return NotFound();
                        }

                        await Task.Run(() =>
                        {
                            ticket1.ListOfPassengers.Add(passenger);
                            if (model.IdFlight2 != 0)
                                ticket2.ListOfPassengers.Add(passenger);
                        });
                    }
                }

                await Task.Run(() =>
                {
                    flight1.ListOfTickets.Add(ticket1);

                    if (model.IdFlight2 != 0)
                    {
                        flight2.ListOfTickets.Add(ticket2);
                    }

                    _dataBase.SaveChanges();
                    transaction.Commit();

                    List<Passengers> passengerList = ticket1.ListOfPassengers.ToList();

                    if (passengerList.Count != 0)
                    {
                        for (int i = 0; i < passengerList.Count; i++)
                        {
                            sendMail(passengerList[i].Email, user.UserName, passengerList[i].Seat,
                                ticket1.DestinationFrom, ticket1.DestinationTo, ticket1.Id, passengerList[i].Id);

                        }
                    }
                });
                return Ok();
            }
        }

        [HttpGet("{IdTicket},{IdPassenger}")]
        [Route("ConfirmReservation/{IdTicket}/{IdPassenger}")]
        public async Task<object> ConfirmReservation(int IdTicket, int IdPassenger)
        {
            var passenger = new Passengers();
            await Task.Run(() =>
            {
                passenger = _dataBase.Passengers.SingleOrDefault(p => p.TicketId == IdTicket && p.Id == IdPassenger);
                passenger.EmailConfirmed = true;
                _dataBase.SaveChanges();
            });
            
            return Ok();
        }

        private async void sendMail(String mailAdress, string username, string seatNum, string destFrom, string destTo, int idTicket, int idPassenger)
        {
            await Task.Run(() =>
            {

                string senderEmail = "gobookyourselfservice@gmail.com";
                string senderpassword = "gobookyourself8";
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.Timeout = 100000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(senderEmail, senderpassword);

                var body = "\r\n" +
                            "\r\n" +
                            "A friend has invited you to join them on a flight!\r\n" +
                            "<br />\r\n" +
                            "<table> \r\n" +
                            "<tr> \r\n" +
                            "<td><label>Friend username: </label></td> \r\n" +
                            "<td><label>" + username + "</label></td> \r\n" +
                            "</tr> \r\n" +
                            "<tr> \r\n" +
                            "<td><label>Flight: </label></td> \r\n" +
                            "<td><label>" + destFrom + " - " + destTo + "</label></td> \r\n" +
                            "</tr> \r\n" +
                            "<tr> \r\n" +
                            "<td><label>Seat NO: </label></td> \r\n" +
                            "<td><label>" + seatNum + "</label></td> \r\n" +
                            "</tr> \r\n" +
                            "</table> \r\n" +
                            "<br /> \r\n" +
                            "\r\n" +
                            "<a href=\"http://localhost:40000/api/Airline/ConfirmReservation/" + idTicket + "/" + idPassenger + "\"> Click here to confirm reservation </a>";

                MailMessage mailMessage = new MailMessage(senderEmail, mailAdress, "Confirm email", body);
                mailMessage.IsBodyHtml = true;
                mailMessage.BodyEncoding = UTF8Encoding.UTF8;
                client.Send(mailMessage);

            });
        }

        [HttpPost]
        [Route("FindTicket")]
        public async Task<object> FindTicket(IdModel model)
        {
            var ticket = await _dataBase.Ticket.FindAsync(model.Id);

            if(ticket.Type == "Super")
            {
                await Task.Run(() =>
                {
                    ticket.TotalPrice = ticket.OriginalPrice - (ticket.OriginalPrice * ticket.Sale / 100);
                    _dataBase.SaveChanges();
                });
            }

            return Ok(ticket);
        }

        [HttpPost]
        [Route("ReserveSuperTicket")]
        public async Task<object> ReserveSuperTicket(ReserveFlightModel model)
        {
            var ticket = await _dataBase.Ticket.FindAsync(model.IdTicket);
            ticket.User = model.UserEmail;
            ticket.Passport = model.Passport;
            ticket.DateOfReservation = DateTime.Now;
            ticket.Type = "SuperRes";

            await Task.Run(() =>
            {
                var seat = _dataBase.Seats.SingleOrDefault(s => s.FullId == ticket.Seat);
                seat.Class = "seat-red";

                _dataBase.SaveChanges();
            });
            return Ok();
        }

        [HttpPost]
        [Route("GetReservedTickets")]
        public async Task<object> GetReservedTickets(IdModel model)
        {
            var tickets = new List<Ticket>();

            await Task.Run(() =>
            {
                foreach (Ticket t in _dataBase.Ticket)
                {
                    if (t.User == model.Email)
                    {
                        tickets.Add(t);
                    }
                }
            });
            return Ok(tickets);
        }

        [HttpPost]
        [Route("RateAll")]
        public async Task<object> RateAll(IdModel model)
        {

            var ticket = await _dataBase.Ticket.FindAsync(model.Id);
            var flight = await _dataBase.Flights.FindAsync(ticket.FlightID);

            var rate1 = new Rating();
            rate1.Rated = model.Rate1;

            flight.AllRatings.Add(rate1);

            var airline = _dataBase.Airlines.SingleOrDefault(a => a.NameOfAirline == ticket.NameOfCompany);

            var rate2 = new Rating();
            rate2.Rated = model.Rate2;
            airline.AllRatings.Add(rate2);

            _dataBase.SaveChanges();
            return Ok();
        }

        [HttpPost]
        [Route("GetAirlineAverageRate")]
        public async Task<object> GetAirlineAverageRate(IdModel model)
        {
            var airline = await _dataBase.Airlines.FindAsync(model.Id);
            double rates = 0;
            int counter = 0;

            foreach(Rating r in _dataBase.Rating)
            {
                if(r.AirlineID == model.Id)
                {
                    rates += r.Rated;
                    counter++;
                }
            }

            airline.RatingOfService = Math.Round(rates / counter);
            _dataBase.SaveChanges();
            return Ok(airline.RatingOfService);
        }

        [HttpPost]
        [Route("GetFlightAverageRate")]
        public async Task<object> GetFlightAverageRate(IdModel model)
        {
            var flight = await _dataBase.Flights.FindAsync(model.Id);
            double rates = 0;
            int counter = 0;

            foreach (Rating r in _dataBase.Rating)
            {
                if (r.FlightID == model.Id)
                {
                    rates += r.Rated;
                    counter++;
                }
            }

            flight.AverageRating = Math.Round(rates / counter);
            _dataBase.SaveChanges();
            return Ok(flight.AverageRating);
        }

        #region Discount Groups
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

            var air = _dataBase.Airlines.Include(r => r.DiscountGroups).First(r => r.Id == int.Parse(group.ServiceId));

            if (air == null)
                return BadRequest("No rentacar found");

            if (air.DiscountGroups.ToList().Find(g => g.MinPoints == group.MinPoints && g.DiscountPercentage == group.DiscountPercentage) != null)
                return BadRequest("Can't have groups with same Minimum points and Percengate values!");

            if (air.DiscountGroups.ToList().Find(g => g.GroupName == group.GroupName) != null)
                return BadRequest("Group with same name exist!");

            var disc = new DiscountGroup() { DiscountPercentage = group.DiscountPercentage, GroupName = group.GroupName, MinPoints = group.MinPoints };

            air.DiscountGroups.Add(disc);
            _dataBase.SaveChanges();
            var ret = air.DiscountGroups.ToList();
            ret.Sort(new DiscountGroupListSort());
            return Ok(ret);
        }

        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("DeleteDiscountGroup")]
        public async Task<Object> DeleteDiscountGroup(string aid, string did)
        {
            var airId = int.Parse(aid);
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
            var air = _dataBase.Airlines.Include(r => r.DiscountGroups).ToList().Find(r => r.Id == airId);
            var ret = air.DiscountGroups.ToList();
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

            var air = _dataBase.Airlines.Include(r => r.DiscountGroups).First(r => r.Id == int.Parse(group.ServiceId));

            if (air == null)
                return BadRequest("No rentacar found");

            var discGr = air.DiscountGroups.ToList().Find(g => g.Id == group.Id);

            if (discGr == null)
                return BadRequest("Group doesn't exist!");

            discGr.GroupName = group.GroupName;
            discGr.MinPoints = group.MinPoints;
            discGr.DiscountPercentage = group.DiscountPercentage;

            _dataBase.SaveChanges();

            var ret = air.DiscountGroups.ToList();
            ret.Sort(new DiscountGroupListSort());
            return Ok(ret);
        }

        //[HttpGet]
        //[Route("GetAllArilinesDiscGroups")]
        //public async Task<Object> GetAllRentacarsDiscGroups()
        //{
        //    var airlines = _dataBase.Airlines.Include(r => r.DiscountGroups);

        //    return Ok(airlines);
        //}
        #endregion
        
        [HttpPost]
        [Route("CancelTicket")]
        public async Task<object> CancelTicket(IdModel model)
        {
            var ticket = await _dataBase.Ticket.FindAsync(model.Id);
            DateTime date = DateTime.Now;
            int res = DateTime.Compare(date, ticket.DateAndTime);
            
            if(res < 0 || (res == 0 && (ticket.DateAndTime.Hour - date.Hour) > 3))
            { 
                _dataBase.Ticket.Remove(ticket);
                _dataBase.SaveChanges();
            }

            return Ok();
        }

        [HttpPost]
        [Route("GetTicketsByAirline")]
        public async Task<object> GetTicketsByAirline(IdModel model)
        {
            List<Ticket> tickets = new List<Ticket>();
            int[] ticketDaily = new int[24];
            int[] ticketWeekly = new int[7];
            int[] ticketMonthly = new int[12];

            for (int i = 0; i < 24; i++)
                ticketDaily[i] = 0;

            for (int i = 0; i < 7; i++)
                ticketWeekly[i] = 0;

            for (int i = 0; i < 12; i++)
                ticketMonthly[i] = 0;

            await Task.Run(() =>
            {
                foreach (Ticket t in _dataBase.Ticket)
                {
                    if (t.NameOfCompany == model.Name && t.Type != "Super")
                    {
                        tickets.Add(t);
                    }
                }

                if (model.Id == 1)
                {
                    foreach (Ticket t in tickets)
                    {
                        if (DateTime.Parse(model.Date).Date == t.DateOfReservation.Date)
                        {
                            if (model.Type == "sales")
                                ticketDaily[t.DateOfReservation.Hour]++;
                            else
                                ticketDaily[t.DateOfReservation.Hour] += (int)t.TotalPrice;
                        }
                    }
                }
                else if (model.Id == 2)
                {
                    foreach (Ticket t in tickets)
                    {
                        for (int i = 0; i < 7; i++)
                        {
                            DateTime date = DateTime.Parse(model.Date);
                            date = date.AddDays(i);

                            if (date.Date == t.DateOfReservation.Date)
                            {
                                if (model.Type == "sales")
                                    ticketWeekly[i]++;
                                else
                                    ticketWeekly[i] += (int)t.TotalPrice;
                            }
                        }
                    }
                }
                else if (model.Id == 3)
                {
                    foreach (Ticket t in tickets)
                    {
                        if (Int32.Parse(model.Date) == t.DateOfReservation.Month)
                        {
                            if (model.Type == "sales")
                                ticketMonthly[t.DateOfReservation.Month - 1]++;
                            else
                                ticketMonthly[t.DateOfReservation.Month - 1] += (int)t.TotalPrice;
                        }
                    }
                }
            });

            if (model.Id == 1)
                return Ok(ticketDaily);
            else if (model.Id == 2)
                return Ok(ticketWeekly);
            else if (model.Id == 3)
                return Ok(ticketMonthly);
            else
                return NoContent();
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("ModifyAirlineInfo")]
        public async Task<Object> ModifyAirlineInfo([FromBody] AirlineInfo info)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _dataBase.Users.FindAsync(userId);

            if (user != null)
            {
                if (user.ServiceID != info.Id)
                {
                    return BadRequest("Not allowed to make modifications on given airline service");
                }

                var res = await _dataBase.Airlines.Include(r => r.Location).SingleOrDefaultAsync(obj => obj.Id == info.Id);

                if (res == null)
                {
                    return NotFound("Given rentacar was not found!");
                }

                res.Image = info.Image;
                res.Location.NameOfCity = info.CityName;
                res.Location.NameOfStreet = info.StreetName;
                res.Location.NumberInStreet = info.Number;
                res.NameOfAirline = info.NameOfService;
                res.DescriptionOfAirline = info.Description;
                _dataBase.SaveChanges();

                return Ok();
            }
            else
            {
                return BadRequest("Not allowed to make modifications on airline service!");
            }
        }

        [HttpPost]
        [Route("FilterAirlines")]
        public async Task<object> FilterAirlines(FilterModel model)
        {
            var filterFlights = new List<Flight>();

            await Task.Run(() =>
            {
                if (model.NameOfCompany == "" && model.PriceFrom == 0 && model.PriceTo == 0 && model.Hours == 0 && model.Minutes == 0)
                    filterFlights = model.Flights.ToList();

                if (model.NameOfCompany != "")
                {
                    foreach (var flight in model.Flights)
                    {
                        if (flight.NameOfAirline.ToLower().Contains(model.NameOfCompany.ToLower()))
                        {
                            if (!filterFlights.Contains(flight))
                                filterFlights.Add(flight);
                        }
                    }
                }

                if (model.PriceFrom != 0)
                {
                    foreach (var flight in model.Flights)
                    {
                        if (model.EconomyClass)
                        {
                            if (flight.TicketPrice >= model.PriceFrom)
                            {
                                if (!filterFlights.Contains(flight))
                                    filterFlights.Add(flight);
                            }
                        }
                        else
                        {
                            if (flight.BusinessPrice >= model.PriceFrom)
                            {
                                if (!filterFlights.Contains(flight))
                                    filterFlights.Add(flight);
                            }
                        }
                    }
                }

                if (model.PriceTo != 0)
                {
                    foreach (var flight in model.Flights)
                    {
                        if (model.EconomyClass)
                        {
                            if (flight.TicketPrice <= model.PriceTo)
                            {
                                if (!filterFlights.Contains(flight))
                                    filterFlights.Add(flight);
                            }
                        }
                        else
                        {
                            if (flight.BusinessPrice <= model.PriceTo)
                            {
                                if (!filterFlights.Contains(flight))
                                    filterFlights.Add(flight);
                            }
                        }
                    }
                }

                if (model.Hours != 0)
                {
                    foreach (var flight in model.Flights)
                    {
                        if (flight.Duration.Hour == model.Hours)
                        {
                            if (!filterFlights.Contains(flight))
                                filterFlights.Add(flight);
                        }
                    }
                }

                if (model.Minutes != 0)
                {
                    foreach (var flight in model.Flights)
                    {
                        if (flight.Duration.Minute == model.Minutes)
                        {
                            if (!filterFlights.Contains(flight))
                                filterFlights.Add(flight);
                        }
                    }
                }
            });

            return Ok(filterFlights);
        }

        [HttpPost]
        [Route("DeleteSeat")]
        public async Task<object> DeleteSeat(IdModel model)
        {
            if (model.Name != "")
            {
                await Task.Run(() =>
                {
                    var seat = _dataBase.Seats.SingleOrDefault(s => s.FullId == model.Name);
                    seat.Class = "seat-noseat";
                    seat.Disabled = true;

                    _dataBase.SaveChanges();
                });
            }
            else
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPost]
        [Route("AddNewSeats")]
        public async Task<object> AddNewSeats(IdModel model)
        {
            var plane = await _dataBase.Planes.FindAsync(model.Id);
            List<Seat> seats = new List<Seat>();

            if (model.BusNumber > 0)
            {
                seats = _dataBase.Seats.ToList();
                int busCounter = plane.BusinessSeats;

                foreach (Seat seat in seats)
                {
                    if (seat.Type == "Bussines" && seat.Disabled)
                    {
                        if(model.BusNumber > 0)
                        {
                            seat.Disabled = false;
                            seat.Class = "seat-white";
                            model.BusNumber--;
                        }
                    }
                }

                int busRowsOld = (int)Math.Ceiling(((double)busCounter / 4));
                int busRowsNew = (int)Math.Ceiling(((double)(busCounter + model.BusNumber) / 4));

                if(busCounter % 4 != 0)         // ako nije popunjen citav red
                {
                    BusRow bRow = _dataBase.BusRows.SingleOrDefault(br => br.IdRow == busRowsOld);
                    //busCounter -= (bRow.Id * 3);
                    int modelBusCounter = model.BusNumber;

                    for (int j = 1; j <= 3; j++)
                    {
                        if (model.BusNumber > 0)
                        {
                            if ((busCounter + j) % 4 == 2)
                            {
                                var seat1 = new Seat();
                                seat1.AirlineId = plane.Id;
                                seat1.IdCol = "C";
                                seat1.Class = "seat-white";
                                seat1.RowId = busRowsOld;
                                seat1.Type = "Bussines";
                                seat1.FullId = seat1.RowId.ToString() + seat1.IdCol;

                                _dataBase.Seats.Add(seat1);
                                _dataBase.SaveChanges();
                                plane.BusinessSeats++;
                                bRow.Seat2 = seat1.Id;
                                model.BusNumber--;
                            }
                            else if ((busCounter + j) % 4 == 3)
                            {
                                var seat1 = new Seat();
                                seat1.AirlineId = plane.Id;
                                seat1.IdCol = "D";
                                seat1.Class = "seat-white";
                                seat1.RowId = bRow.IdRow;
                                seat1.Type = "Bussines";
                                seat1.FullId = seat1.RowId.ToString() + seat1.IdCol;

                                _dataBase.Seats.Add(seat1);
                                _dataBase.SaveChanges();
                                plane.BusinessSeats++;
                                bRow.Seat6 = seat1.Id;
                                model.BusNumber--;
                            }
                            else if ((busCounter + j) % 4 == 0)
                            {
                                var seat1 = new Seat();
                                seat1.AirlineId = plane.Id;
                                seat1.IdCol = "F";
                                seat1.Class = "seat-white";
                                seat1.RowId = bRow.IdRow;
                                seat1.Type = "Bussines";
                                seat1.FullId = seat1.RowId.ToString() + seat1.IdCol;

                                _dataBase.Seats.Add(seat1);
                                _dataBase.SaveChanges();
                                plane.BusinessSeats++;
                                bRow.Seat7 = seat1.Id;

                                plane.BusinessRows.Add(bRow);

                                j = 4;
                                model.BusNumber--;
                            }
                        }
                        else
                        {
                            plane.BusinessRows.Add(bRow);
                        }

                    }
                }

                if (model.BusNumber > 0)
                {
                    for (int i = busRowsOld + 1; i <= busRowsNew; i++)
                    {
                        IEnumerable<EcoRow> ecoRows = _dataBase.EcoRows.ToList();
                        ecoRows = ecoRows.OrderByDescending(x => x.IdRow);
                        foreach (EcoRow ecoRow in ecoRows)
                        {
                            var oldSeats = _dataBase.Seats.ToList().Where(s => s.RowId == ecoRow.IdRow);
                            foreach (var seat in oldSeats)
                                seat.RowId++;
                            ++ecoRow.IdRow;
                        }

                        BusRow bRow = new BusRow();
                        plane.CountBusRows++;
                        bRow.IdRow = i;
                        var seat3 = new Seat();
                        seat3.AirlineId = plane.Id;
                        var seat4 = new Seat();
                        seat4.AirlineId = plane.Id;
                        var seat5 = new Seat();
                        seat5.AirlineId = plane.Id;

                        _dataBase.Seats.Add(seat3);
                        _dataBase.Seats.Add(seat4);
                        _dataBase.Seats.Add(seat5);
                        _dataBase.SaveChanges();

                        seat3.RowId = bRow.IdRow;
                        seat3.Type = "Bussines";
                        seat3.FullId = seat3.RowId.ToString() + seat3.IdCol;
                        bRow.Seat3 = seat3.Id;
                        seat4.RowId = bRow.IdRow;
                        seat4.Type = "Bussines";
                        seat4.FullId = seat4.RowId.ToString() + seat4.IdCol;
                        bRow.Seat4 = seat4.Id;
                        seat5.RowId = bRow.IdRow;
                        seat5.Type = "Bussines";
                        seat5.FullId = seat5.RowId.ToString() + seat5.IdCol;
                        bRow.Seat5 = seat5.Id;

                        for (int j = 1; j <= model.BusNumber; j++)
                        {
                            if (j % 4 == 1)
                            {
                                var seat1 = new Seat();
                                seat1.AirlineId = plane.Id;
                                seat1.IdCol = "A";
                                seat1.Class = "seat-white";
                                seat1.RowId = bRow.IdRow;
                                seat1.Type = "Bussines";
                                seat1.FullId = seat1.RowId.ToString() + seat1.IdCol;

                                _dataBase.Seats.Add(seat1);
                                _dataBase.SaveChanges();
                                plane.BusinessSeats++;
                                bRow.Seat1 = seat1.Id;
                            }
                            else if (j % 4 == 2)
                            {
                                var seat1 = new Seat();
                                seat1.AirlineId = plane.Id;
                                seat1.IdCol = "C";
                                seat1.Class = "seat-white";
                                seat1.RowId = bRow.IdRow;
                                seat1.Type = "Bussines";
                                seat1.FullId = seat1.RowId.ToString() + seat1.IdCol;

                                _dataBase.Seats.Add(seat1);
                                _dataBase.SaveChanges();
                                plane.BusinessSeats++;
                                bRow.Seat2 = seat1.Id;
                            }
                            else if (j % 4 == 3)
                            {
                                var seat1 = new Seat();
                                seat1.AirlineId = plane.Id;
                                seat1.IdCol = "D";
                                seat1.Class = "seat-white";
                                seat1.RowId = bRow.IdRow;
                                seat1.Type = "Bussines";
                                seat1.FullId = seat1.RowId.ToString() + seat1.IdCol;

                                _dataBase.Seats.Add(seat1);
                                _dataBase.SaveChanges();
                                plane.BusinessSeats++;
                                bRow.Seat6 = seat1.Id;
                            }
                            else if (j % 4 == 0)
                            {
                                var seat1 = new Seat();
                                seat1.AirlineId = plane.Id;
                                seat1.IdCol = "F";
                                seat1.Class = "seat-white";
                                seat1.RowId = bRow.IdRow;
                                seat1.Type = "Bussines";
                                seat1.FullId = seat1.RowId.ToString() + seat1.IdCol;

                                _dataBase.Seats.Add(seat1);
                                _dataBase.SaveChanges();
                                plane.BusinessSeats++;
                                bRow.Seat7 = seat1.Id;

                                plane.BusinessRows.Add(bRow);
                                j = model.BusNumber + 1;
                                model.BusNumber -= 4;
                            }

                            if (j == model.BusNumber)
                            {
                                plane.BusinessRows.Add(bRow);
                            }

                        }
                    }
                }

            }

            if (model.EcoNumber > 0)
            {
                seats = _dataBase.Seats.ToList();
                int ecoCounter = plane.EconomySeats;

                foreach (Seat seat in seats)
                {
                    if (seat.Type == "Economy" && seat.Disabled)
                    {
                        if (model.EcoNumber > 0)
                        {
                            seat.Disabled = false;
                            seat.Class = "seat-white";
                            model.EcoNumber--;
                        }
                    }
                }

                int ecoRowsOld = (int)Math.Ceiling(((double)ecoCounter / 6)) + plane.CountBusRows;
                int ecoRowsNew = (int)Math.Ceiling(((double)(ecoCounter + model.EcoNumber) / 6)) + plane.CountBusRows;

                if (ecoCounter % 6 != 0)         // ako nije popunjen citav red
                {
                    EcoRow eRow = _dataBase.EcoRows.SingleOrDefault(er => er.IdRow == ecoRowsOld);
                    //ecoCounter -= (eRow.Id - plane.CountBusRows);
                    int modelEcoCounter = model.EcoNumber;

                    for (int j = 1; j <= 5; j++)
                    {
                        if (model.EcoNumber > 0)
                        {
                            if ((ecoCounter + j) % 6 == 2)
                            {
                                var seat1 = new Seat();
                                seat1.AirlineId = plane.Id;
                                seat1.IdCol = "B";
                                seat1.Class = "seat-white";
                                seat1.RowId = eRow.IdRow;
                                seat1.Type = "Economy";
                                seat1.FullId = seat1.RowId.ToString() + seat1.IdCol;

                                _dataBase.Seats.Add(seat1);
                                _dataBase.SaveChanges();
                                plane.EconomySeats++;
                                eRow.Seat2 = seat1.Id;
                                model.EcoNumber--;
                            }
                            else if ((ecoCounter + j) % 6 == 3)
                            {
                                var seat1 = new Seat();
                                seat1.AirlineId = plane.Id;
                                seat1.IdCol = "C";
                                seat1.Class = "seat-white";
                                seat1.RowId = eRow.IdRow;
                                seat1.Type = "Economy";
                                seat1.FullId = seat1.RowId.ToString() + seat1.IdCol;

                                _dataBase.Seats.Add(seat1);
                                _dataBase.SaveChanges();
                                plane.EconomySeats++;
                                eRow.Seat3 = seat1.Id;
                                model.EcoNumber--;
                            }
                            else if ((ecoCounter + j) % 6 == 4)
                            {
                                var seat1 = new Seat();
                                seat1.AirlineId = plane.Id;
                                seat1.IdCol = "D";
                                seat1.Class = "seat-white";
                                seat1.RowId = eRow.IdRow;
                                seat1.Type = "Economy";
                                seat1.FullId = seat1.RowId.ToString() + seat1.IdCol;

                                _dataBase.Seats.Add(seat1);
                                _dataBase.SaveChanges();
                                plane.EconomySeats++;
                                eRow.Seat5 = seat1.Id;
                                model.EcoNumber--;
                            }
                            else if ((ecoCounter + j) % 6 == 5)
                            {
                                var seat1 = new Seat();
                                seat1.AirlineId = plane.Id;
                                seat1.IdCol = "E";
                                seat1.Class = "seat-white";
                                seat1.RowId = eRow.IdRow;
                                seat1.Type = "Economy";
                                seat1.FullId = seat1.RowId.ToString() + seat1.IdCol;

                                _dataBase.Seats.Add(seat1);
                                _dataBase.SaveChanges();
                                plane.EconomySeats++;
                                eRow.Seat6 = seat1.Id;
                                model.EcoNumber--;
                            }
                            else if ((ecoCounter + j) % 6 == 0)
                            {
                                var seat1 = new Seat();
                                seat1.AirlineId = plane.Id;
                                seat1.IdCol = "F";
                                seat1.Class = "seat-white";
                                seat1.RowId = eRow.IdRow;
                                seat1.Type = "Economy";
                                seat1.FullId = seat1.RowId.ToString() + seat1.IdCol;

                                _dataBase.Seats.Add(seat1);
                                _dataBase.SaveChanges();
                                plane.EconomySeats++;
                                eRow.Seat7 = seat1.Id;

                                plane.EconomyRows.Add(eRow);
                                j = 6;
                                model.EcoNumber--;
                            }

                        }
                        else
                        {
                            plane.EconomyRows.Add(eRow);
                        }

                    }
                }

                if (model.EcoNumber > 0)
                {
                    for (int i = ecoRowsOld + 1; i <= ecoRowsNew; i++)
                    {
                        EcoRow eRow = new EcoRow();
                        plane.CountEcoRows++;
                        eRow.IdRow = i;
                        var seat4 = new Seat();
                        seat4.AirlineId = plane.Id;
                        seat4.RowId = eRow.IdRow;
                        seat4.Type = "Economy";
                        seat4.FullId = seat4.RowId.ToString() + seat4.IdCol;

                        _dataBase.Seats.Add(seat4);
                        _dataBase.SaveChanges();

                        eRow.Seat4 = seat4.Id;

                        for (int j = 1; j <= model.EcoNumber; j++)
                        {
                            if (j % 6 == 1)
                            {
                                var seat1 = new Seat();
                                seat1.AirlineId = plane.Id;
                                seat1.IdCol = "A";
                                seat1.Class = "seat-white";
                                seat1.RowId = eRow.IdRow;
                                seat1.Type = "Economy";
                                seat1.FullId = seat1.RowId.ToString() + seat1.IdCol;

                                _dataBase.Seats.Add(seat1);
                                _dataBase.SaveChanges();
                                plane.EconomySeats++;
                                eRow.Seat1 = seat1.Id;
                            }
                            else if (j % 6 == 2)
                            {
                                var seat1 = new Seat();
                                seat1.AirlineId = plane.Id;
                                seat1.IdCol = "B";
                                seat1.Class = "seat-white";
                                seat1.RowId = eRow.IdRow;
                                seat1.Type = "Economy";
                                seat1.FullId = seat1.RowId.ToString() + seat1.IdCol;

                                _dataBase.Seats.Add(seat1);
                                _dataBase.SaveChanges();
                                plane.EconomySeats++;
                                eRow.Seat2 = seat1.Id;
                            }
                            else if (j % 6 == 3)
                            {
                                var seat1 = new Seat();
                                seat1.AirlineId = plane.Id;
                                seat1.IdCol = "C";
                                seat1.Class = "seat-white";
                                seat1.RowId = eRow.IdRow;
                                seat1.Type = "Economy";
                                seat1.FullId = seat1.RowId.ToString() + seat1.IdCol;

                                _dataBase.Seats.Add(seat1);
                                _dataBase.SaveChanges();
                                plane.EconomySeats++;
                                eRow.Seat3 = seat1.Id;
                            }
                            else if (j % 6 == 4)
                            {
                                var seat1 = new Seat();
                                seat1.AirlineId = plane.Id;
                                seat1.IdCol = "D";
                                seat1.Class = "seat-white";
                                seat1.RowId = eRow.IdRow;
                                seat1.Type = "Economy";
                                seat1.FullId = seat1.RowId.ToString() + seat1.IdCol;

                                _dataBase.Seats.Add(seat1);
                                _dataBase.SaveChanges();
                                plane.EconomySeats++;
                                eRow.Seat5 = seat1.Id;
                            }
                            else if (j % 6 == 5)
                            {
                                var seat1 = new Seat();
                                seat1.AirlineId = plane.Id;
                                seat1.IdCol = "E";
                                seat1.Class = "seat-white";
                                seat1.RowId = eRow.IdRow;
                                seat1.Type = "Economy";
                                seat1.FullId = seat1.RowId.ToString() + seat1.IdCol;

                                _dataBase.Seats.Add(seat1);
                                _dataBase.SaveChanges();
                                plane.EconomySeats++;
                                eRow.Seat6 = seat1.Id;
                            }
                            else if (j % 6 == 0)
                            {
                                var seat1 = new Seat();
                                seat1.AirlineId = plane.Id;
                                seat1.IdCol = "F";
                                seat1.Class = "seat-white";
                                seat1.RowId = eRow.IdRow;
                                seat1.Type = "Economy";
                                seat1.FullId = seat1.RowId.ToString() + seat1.IdCol;

                                _dataBase.Seats.Add(seat1);
                                _dataBase.SaveChanges();
                                plane.EconomySeats++;
                                eRow.Seat7 = seat1.Id;

                                plane.EconomyRows.Add(eRow);
                                j = model.EcoNumber + 1;
                                model.EcoNumber -= 6;
                            }

                            if (j == model.EcoNumber)
                            {
                                plane.EconomyRows.Add(eRow);
                            }
                        }
                    }
                }

            }

            _dataBase.SaveChanges();
            return Ok();
        }

        [HttpPost]
        [Route("AddNewSuperTicket")]
        public async Task<object> AddNewSuperTicket(ReserveFlightModel model)
        {
            Ticket ticket = new Ticket();
            var flight = await _dataBase.Flights.FindAsync(model.IdFlight1);

            ticket.DestinationFrom = flight.DestinationFrom;
            ticket.DestinationTo = flight.DestinationTo;
            ticket.DateAndTime = flight.DepartingDateTime;
            ticket.Seat = model.Seat1;
            ticket.Sale = model.Sale;
            ticket.Type = "Super";

            var seat = _dataBase.Seats.SingleOrDefault(s => s.FullId == model.Seat1);

            if(seat.Class != "seat-white")
            {
                return NotFound();
            }
            else
            {
                seat.Class = "seat-yellow";
            }

            if(seat.Type == "Economy")
            {
                ticket.OriginalPrice = flight.TicketPrice;
            }
            else
            {
                ticket.OriginalPrice = flight.BusinessPrice;
            }

            ticket.TotalPrice = ticket.OriginalPrice - (ticket.OriginalPrice * ticket.Sale / 100);

            flight.ListOfTickets.Add(ticket);
            _dataBase.SaveChanges();


            return Ok();
        }


    }
} 