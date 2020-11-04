using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Classes
{
    public static class CalculateRating
    {
        public static double Calculate(List<BackEnd.Models.Rating> list)
        {
            double sum = 0;

            list.ForEach(l => sum += l.Rated);

            return sum / list.Count();
        }
    }
}
