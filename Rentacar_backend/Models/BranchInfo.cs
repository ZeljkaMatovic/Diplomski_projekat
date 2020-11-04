using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class BranchInfo
    {
        public int BranchId { get; set; }
        public int RentacarId { get; set; }
        public string BranchName { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Number { get; set; }
        public double GeoWidth { get; set; }
        public double GeoHeight { get; set; }
    }

    public class Vehicles
    {
        public ICollection<VehicleRequest> Requests { get; set; }
    }
}
