using BackEnd.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Classes
{
    public class DiscountGroupListSort : IComparer<DiscountGroup>
    {
        public int Compare([AllowNull] DiscountGroup x, [AllowNull] DiscountGroup y)
        {
            if (x.MinPoints == y.MinPoints)
                return 0;
            if (x.MinPoints > y.MinPoints)
                return -1;

            return 1;
        }
    }
}
