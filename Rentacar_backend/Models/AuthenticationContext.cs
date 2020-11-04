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
        public AuthenticationContext(DbContextOptions options) : base(options)
        {
            this.ChangeTracker.LazyLoadingEnabled = false;
        }
        public DbSet<CarReservation> CarReservations { get; set; }
        public DbSet<User> Users { get; set; }
        //public DbSet<RegisteredUser> RuUsers { get; set; }
        //public DbSet<SystemAdmin> SysAdminUsers { get; set; }
        //public DbSet<ARSAdmin> ARSUsers { get; set; }
        //public DbSet<RCSAdmin> RCSUsers { get; set; }
        public DbSet<Rentacar> Rentacars { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Profit> Profits { get; set; }
        public DbSet<Friend> Friends { get; set; }
      //  public DbSet<Passengers> Passengers { get; set; }
        public DbSet<Rating> Rating { get; set; }
        public DbSet<DiscountGroup> DiscountGroups { get; set; }



    }
}
