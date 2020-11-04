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

            var locAir = new Location() { GeoHeight = 2.295138, GeoWidth = 48.876965, NameOfCity = "Paris", NameOfStreet = "Rue Bray", NumberInStreet = "12" };
            var airline = new Airline()
            {
                RatingOfService = 0,
                Location = locAir,
                DescriptionOfAirline = "",
                NameOfAirline = "Air France"
            ,
                Image = "Paris.jpg"
            };


            context.Airlines.Add(airline);

            var flight1 = new Flight
            {
                DestinationFrom = "Madrid",
                DestinationTo = "Barcelona",
                DepartingDateTime = new DateTime(2020, 5, 1, 22, 0, 0, DateTimeKind.Utc),
                TimeOfFlight = new DateTime(2020, 1, 1, 23, 20, 0, DateTimeKind.Utc),
                Duration = new DateTime(2020, 1, 1, 1, 20, 0, DateTimeKind.Utc),
                TicketPrice = 200,
                NameOfAirline = "Air France",
                BusinessPrice = 200 + (200 * 80 / 100)
            };

            var flight2 = new Flight     //treci let sa fronta
            {
                DestinationFrom = "Berlin",
                DestinationTo = "Venice",
                DepartingDateTime = new DateTime(2021, 1, 2, 12, 0, 0, DateTimeKind.Utc),
                TimeOfFlight = new DateTime(2020, 1, 1, 14, 0, 0, DateTimeKind.Utc),
                Duration = new DateTime(2020, 1, 1, 2, 0, 0, DateTimeKind.Utc),
                TicketPrice = 380,
                NameOfAirline = "Air France",
                BusinessPrice = 380 + (380 * 80 / 100)
            };

            var flight3 = new Flight     //peti let sa fronta
            {
                DestinationFrom = "Barcelona",
                DestinationTo = "Madrid",
                DepartingDateTime = new DateTime(2022, 6, 10, 12, 0, 0, DateTimeKind.Utc),
                TimeOfFlight = new DateTime(2020, 1, 1, 13, 20, 0, DateTimeKind.Utc),
                Duration = new DateTime(2020, 1, 1, 1, 20, 0, DateTimeKind.Utc),
                TicketPrice = 300,
                NameOfAirline = "Air France",
                BusinessPrice = 300 + (300 * 80 / 100)
            };

            airline.ListOfFlights.Add(flight1);
            airline.ListOfFlights.Add(flight2);
            airline.ListOfFlights.Add(flight3);

            var destination1 = new Destination
            {
                DestinationName = "Barcelona",
                AirlineId = 1
            };

            var destination2 = new Destination
            {
                DestinationName = "Madrid",
                AirlineId = 1
            };

            var destination3 = new Destination
            {
                DestinationName = "Prague",
                AirlineId = 1
            };

            var destination4 = new Destination
            {
                DestinationName = "Berlin",
                AirlineId = 1
            };

            var destination5 = new Destination
            {
                DestinationName = "Venice",
                AirlineId = 1
            };

            airline.Destinations.Add(destination1);
            airline.Destinations.Add(destination2);
            airline.Destinations.Add(destination3);
            airline.Destinations.Add(destination4);
            airline.Destinations.Add(destination5);

            var ticket1 = new Ticket
            {
                DestinationFrom = "Madrid",
                DestinationTo = "Barcelona",
                DateAndTime = new DateTime(2021, 1, 2, 12, 0, 0, DateTimeKind.Utc),
                Seat = "1D",
                OriginalPrice = 200,
                Sale = 10,
                NameOfCompany = "Air France",
                Type = "Super"
            };

            var ticket2 = new Ticket
            {
                DestinationFrom = "Berlin",
                DestinationTo = "Venice",
                DateAndTime = new DateTime(2020, 5, 1, 22, 0, 0, DateTimeKind.Utc),
                Seat = "1A",
                OriginalPrice = 380,
                Sale = 15,
                NameOfCompany = "Air France",
                Type = "Super"
            };

            var ticket3 = new Ticket
            {
                DestinationFrom = "Barcelona",
                DestinationTo = "Madrid",
                DateAndTime = new DateTime(2022, 6, 10, 12, 0, 0, DateTimeKind.Utc),
                Seat = "1C",
                OriginalPrice = 300,
                Sale = 15,
                NameOfCompany = "Air France",
                Type = "Super"
            };

            flight1.ListOfTickets.Add(ticket1);
            flight2.ListOfTickets.Add(ticket2);
            flight3.ListOfTickets.Add(ticket3);

            var plane = new Plane();
            int busSeats = 17;
            int ecoSeats = 40;
            plane.BusinessSeats = busSeats;
            plane.EconomySeats = ecoSeats;
            plane.CountBusRows = (int)Math.Ceiling(((double)plane.BusinessSeats / 4));
            plane.CountEcoRows = (int)Math.Ceiling(((double)plane.EconomySeats / 6));

            for (int i = 1; i <= plane.CountBusRows; i++)
            {
                BusRow bRow = new BusRow();
                bRow.IdRow = i;
                var seat3 = new Seat();
                seat3.AirlineId = airline.Id;
                var seat4 = new Seat();
                seat4.AirlineId = airline.Id;
                var seat5 = new Seat();
                seat5.AirlineId = airline.Id;

                context.Seats.Add(seat3);
                context.Seats.Add(seat4);
                context.Seats.Add(seat5);
                context.SaveChanges();

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

                for (int j = 1; j <= busSeats; j++)
                {
                    if (j % 4 == 1)
                    {
                        var seat1 = new Seat();
                        seat1.AirlineId = airline.Id;
                        seat1.IdCol = "A";
                        seat1.Class = "seat-white";
                        seat1.RowId = bRow.IdRow;
                        seat1.Type = "Bussines";
                        seat1.FullId = seat1.RowId.ToString() + seat1.IdCol;

                        context.Seats.Add(seat1);
                        context.SaveChanges();
                        bRow.Seat1 = seat1.Id;
                    }
                    else if (j % 4 == 2)
                    {
                        var seat1 = new Seat();
                        seat1.AirlineId = airline.Id;
                        seat1.IdCol = "C";
                        seat1.Class = "seat-white";
                        seat1.RowId = bRow.IdRow;
                        seat1.Type = "Bussines";
                        seat1.FullId = seat1.RowId.ToString() + seat1.IdCol;

                        context.Seats.Add(seat1);
                        context.SaveChanges();
                        bRow.Seat2 = seat1.Id;
                    }
                    else if (j % 4 == 3)
                    {
                        var seat1 = new Seat();
                        seat1.AirlineId = airline.Id;
                        seat1.IdCol = "D";
                        seat1.Class = "seat-white";
                        seat1.RowId = bRow.IdRow;
                        seat1.Type = "Bussines";
                        seat1.FullId = seat1.RowId.ToString() + seat1.IdCol;

                        context.Seats.Add(seat1);
                        context.SaveChanges();
                        bRow.Seat6 = seat1.Id;
                    }
                    else if (j % 4 == 0)
                    {
                        var seat1 = new Seat();
                        seat1.AirlineId = airline.Id;
                        seat1.IdCol = "F";
                        seat1.Class = "seat-white";
                        seat1.RowId = bRow.IdRow;
                        seat1.Type = "Bussines";
                        seat1.FullId = seat1.RowId.ToString() + seat1.IdCol;

                        context.Seats.Add(seat1);
                        context.SaveChanges();
                        bRow.Seat7 = seat1.Id;

                        plane.BusinessRows.Add(bRow);
                        context.SaveChanges();
                        j = plane.BusinessSeats + 1;
                        busSeats -= 4;
                    }

                    if (j == busSeats)
                    {
                        plane.BusinessRows.Add(bRow);
                        context.SaveChanges();
                    }

                }
            }

            // eco seats

            for (int i = 1; i <= plane.CountEcoRows; i++)
            {
                EcoRow eRow = new EcoRow();
                eRow.IdRow = plane.CountBusRows + i;
                var seat4 = new Seat();
                seat4.AirlineId = airline.Id;
                seat4.RowId = eRow.IdRow;
                seat4.Type = "Economy";
                seat4.FullId = seat4.RowId.ToString() + seat4.IdCol;

                context.Seats.Add(seat4);
                context.SaveChanges();

                eRow.Seat4 = seat4.Id;

                for (int j = 1; j <= ecoSeats; j++)
                {
                    if (j % 6 == 1)
                    {
                        var seat1 = new Seat();
                        seat1.AirlineId = airline.Id;
                        seat1.IdCol = "A";
                        seat1.Class = "seat-white";
                        seat1.RowId = eRow.IdRow;
                        seat1.Type = "Economy";
                        seat1.FullId = seat1.RowId.ToString() + seat1.IdCol;

                        context.Seats.Add(seat1);
                        context.SaveChanges();
                        eRow.Seat1 = seat1.Id;
                    }
                    else if (j % 6 == 2)
                    {
                        var seat1 = new Seat();
                        seat1.AirlineId = airline.Id;
                        seat1.IdCol = "B";
                        seat1.Class = "seat-white";
                        seat1.RowId = eRow.IdRow;
                        seat1.Type = "Economy";
                        seat1.FullId = seat1.RowId.ToString() + seat1.IdCol;

                        context.Seats.Add(seat1);
                        context.SaveChanges();
                        eRow.Seat2 = seat1.Id;
                    }
                    else if (j % 6 == 3)
                    {
                        var seat1 = new Seat();
                        seat1.AirlineId = airline.Id;
                        seat1.IdCol = "C";
                        seat1.Class = "seat-white";
                        seat1.RowId = eRow.IdRow;
                        seat1.Type = "Economy";
                        seat1.FullId = seat1.RowId.ToString() + seat1.IdCol;

                        context.Seats.Add(seat1);
                        context.SaveChanges();
                        eRow.Seat3 = seat1.Id;
                    }
                    else if (j % 6 == 4)
                    {
                        var seat1 = new Seat();
                        seat1.AirlineId = airline.Id;
                        seat1.IdCol = "D";
                        seat1.Class = "seat-white";
                        seat1.RowId = eRow.IdRow;
                        seat1.Type = "Economy";
                        seat1.FullId = seat1.RowId.ToString() + seat1.IdCol;

                        context.Seats.Add(seat1);
                        context.SaveChanges();
                        eRow.Seat5 = seat1.Id;
                    }
                    else if (j % 6 == 5)
                    {
                        var seat1 = new Seat();
                        seat1.AirlineId = airline.Id;
                        seat1.IdCol = "E";
                        seat1.Class = "seat-white";
                        seat1.RowId = eRow.IdRow;
                        seat1.Type = "Economy";
                        seat1.FullId = seat1.RowId.ToString() + seat1.IdCol;

                        context.Seats.Add(seat1);
                        context.SaveChanges();
                        eRow.Seat6 = seat1.Id;
                    }
                    else if (j % 6 == 0)
                    {
                        var seat1 = new Seat();
                        seat1.AirlineId = airline.Id;
                        seat1.IdCol = "F";
                        seat1.Class = "seat-white";
                        seat1.RowId = eRow.IdRow;
                        seat1.Type = "Economy";
                        seat1.FullId = seat1.RowId.ToString() + seat1.IdCol;

                        context.Seats.Add(seat1);
                        context.SaveChanges();
                        eRow.Seat7 = seat1.Id;

                        plane.EconomyRows.Add(eRow);
                        j = plane.EconomySeats + 1;
                        ecoSeats -= 6;
                    }

                    if (j == ecoSeats)
                    {
                        plane.EconomyRows.Add(eRow);
                    }

                }
            }

            var allBusRows = context.BusRows.Reverse();

            plane.AirlineId = airline.Id;
            context.Planes.Add(plane);

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
                //Rentacar = rentcar,
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
                Airline = airline,
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

            var seat10 = context.Seats.SingleOrDefault(s => s.IdCol == "A" && s.RowId == 1);
            seat10.Class = "seat-yellow";

            var seat20 = context.Seats.SingleOrDefault(s => s.IdCol == "C" && s.RowId == 1);
            seat20.Class = "seat-yellow";

            var seat30 = context.Seats.SingleOrDefault(s => s.IdCol == "D" && s.RowId == 1);
            seat30.Class = "seat-yellow";

            context.SaveChanges();
        }
    }
}
