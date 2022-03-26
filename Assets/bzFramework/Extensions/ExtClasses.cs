using System.Collections.Generic;
using System.Linq;

namespace BZFramework
{
    public static class ListHelper
    {
        //  This class just provides some useful extension routines for generic and integer lists
        public static void Swap<T>(this IList<T> list, int indexA, int indexB)  //  Switch elements at A and B
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

        public static void SendToBack<T>(this IList<T> list, int indexA)  //  Move element at A to the end of the list
        {
            T tmp = list[indexA];
            list.RemoveAt(indexA);
            list.Add(tmp);
        }

        public static void BringToFront<T>(this IList<T> list, int indexA)  //  Move element at A to the front of the list
        {
            T tmp = list[indexA];
            list.RemoveAt(indexA);
            list.Insert(0, tmp);
        }

        public static void MoveTo<T>(this IList<T> list, int indexA, int indexB)  //  Move element at A to position B
        {
            T tmp = list[indexA];
            list.RemoveAt(indexA);
            list.Insert(indexB, tmp);
        }

    }

    public static class IntListHelper
    {
        public static IEnumerable<int> SplitInts(this string list, char separator = ',')
        {
            int result = 0;
            return (from s in list.Split(separator)
                    let isint = int.TryParse(s, out result)
                    let val = result
                    where isint
                    select val);
        }

        public static string ConvertToString(this IEnumerable<int> list, string separator = ",")
        {
            return string.Join(separator, list);
        }

    }

}
