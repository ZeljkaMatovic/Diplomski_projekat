using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class DiscountGroup
    {
        [Key]
        public int Id { get; set; }
        public string GroupName { get; set; }
        public int MinPoints { get; set; }
        public int DiscountPercentage { get; set; }

}
}
