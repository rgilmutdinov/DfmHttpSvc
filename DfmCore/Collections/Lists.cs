using System.Collections.Generic;
using System.Linq;

namespace DfmCore.Collections
{
    public static class Lists
    {
        public static List<T> Of<T>(T firstItem, params T[] otherItems)
        {
            List<T> list = new List<T> { firstItem };

            if (otherItems != null)
            {
                list.AddRange(otherItems);
            }

            return list;
        }

        public static List<T> Empty<T>()
        {
            return Enumerable.Empty<T>().ToList();
        }
    }
}
