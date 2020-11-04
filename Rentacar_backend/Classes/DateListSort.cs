using BackEnd.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Classes
{
    public class DateListSort : IComparer<Date>
    {
        public int Compare([AllowNull] Date x, [AllowNull] Date y)
        {
            if (x.DateTime == y.DateTime)
                return 0;
            if (x.DateTime > y.DateTime)
                return 1;
            return 0;
        }
    }
}
