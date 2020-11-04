using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class User : IdentityUser
    {
        public User(string role)
        {
            if(role == "ru")
            {
                Friends = new HashSet<Friend>();
                FriendRequests = new HashSet<Friend>();
                ReservedTickets = new HashSet<Ticket>();
                TicketHistory = new HashSet<Ticket>();
               // ReservedCars = new HashSet<CarReservation>();
               // ReservedCarsHistory = new HashSet<CarReservation>();
                DiscountPoints = new HashSet<DiscountPoints>();
            }
            Role = role;
        }

        public User()
        {

        }


        [Key]
        public override string UserName { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Role { get; set; }
        public string Image { get; set; }
        public int? RentacarID { get; set; }
       // public virtual Rentacar Rentacar { get; set; }
        public int? ServiceID { get; set; }
        public virtual Airline Airline { get; set; }
        public string PassportNumber { get; set; }
        public string City { get; set; }
        public virtual ICollection<Friend> Friends { get; set; }
        public virtual ICollection<Friend> FriendRequests { get; set; }
        public virtual ICollection<Ticket> ReservedTickets { get; set; }
        public virtual ICollection<Ticket> TicketHistory { get; set; }
      //  public virtual ICollection<CarReservation> ReservedCars { get; set; }
      //  public virtual ICollection<CarReservation> ReservedCarsHistory { get; set; }
        public virtual ICollection<DiscountPoints> DiscountPoints { get; set; }
    }
}
