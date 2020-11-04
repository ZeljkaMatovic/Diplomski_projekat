using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Models
{
    public class AuthenticationContext : DbContext
    {
        public AuthenticationContext(DbContextOptions<AuthenticationContext> options) : base(options)
        {
            this.ChangeTracker.LazyLoadingEnabled = false;
        }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<User> Users { get; set; }
        //public DbSet<RegisteredUser> RuUsers { get; set; }
        //public DbSet<SystemAdmin> SysAdminUsers { get; set; }
        //public DbSet<ARSAdmin> ARSUsers { get; set; }
        //public DbSet<RCSAdmin> RCSUsers { get; set; }
        public DbSet<Airline> Airlines { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<Plane> Planes { get; set; }
        public DbSet<Ticket> Ticket { get; set; }
        public DbSet<Seat> Seats { get; set; } 
        public DbSet<BusRow> BusRows { get; set; }
        public DbSet<EcoRow> EcoRows { get; set; }
        public DbSet<Passengers> Passengers { get; set; }
        public DbSet<Rating> Rating { get; set; }
        public DbSet<DiscountGroup> DiscountGroups { get; set; }



    }
}
