using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class RegisteredUser : IdentityUser
    {
        [Key]
        public override string UserName { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Lastname { get; set; }
        public string Role { get; set; }
        public string Image { get; set; }
        public string PassportNumber { get; set; }
        public ICollection<Friend> Friends { get; set; }
        public ICollection<Friend> FriendRequests { get; set; }
        public ICollection<Ticket> ReservedTickets { get; set; }
        public ICollection<Ticket> TicketHistory { get; set; }
       // public ICollection<CarReservation> ReservedCars { get; set; }
       // public ICollection<CarReservation> ReservedCarsHistory { get; set; }
        public ICollection<DiscountPoints> DiscountPoints { get; set; }
    }

    public class RegisteredUserModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string City { get; set; }
        public string Passport { get; set; }
    }

    public class DiscountPoints
    {
        [Key]
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public int Points { get; set; }
    }
}
