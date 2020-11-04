using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class DiscountGroupInfo
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public int MinPoints { get; set; }
        public int DiscountPercentage { get; set; }
        public string ServiceId { get; set; }

        
    }
}
