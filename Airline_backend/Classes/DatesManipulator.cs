using BackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Classes
{
    public static class DatesManipulator
    {
        public static bool ExistsInList(List<Date> toCheck, DateTime toComapreTo)
        {
            return toCheck.Find(d => d.DateTime == toComapreTo) != null ? true : false;
        }

        public static bool CompareLists(List<Date> toCheck, List<DateTime> toCompare)
        {
            bool ret = true;
            toCheck.ForEach(d =>
            {
                toCompare.ForEach(c =>
                {
                    if (d.DateTime == c)
                    {
                        ret = false;
                        return;
                    }
                });
                if (!ret)
                    return;
            });

            return ret;
        }

        public static List<DateTime> GiveMeDateList(DateTime startDate, DateTime endDate)
        {
            var ret = new List<DateTime>();
            for(var d = startDate; d <= endDate; d = d.AddDays(1))
            {
                ret.Add(d);
            }

            return ret;
        }

        public static bool ContainsAllDates(List<Date> dates, List<DateTime> toCompare)
        {
            var data = new List<DateTime>();

            dates.ForEach(d =>
            {
                toCompare.ForEach(t =>
                {
                    if (d.DateTime == t)
                        data.Add(t);
                });
            });

            if (data.Count == dates.Count)
                return true;
            return false;

        }
    }
}
