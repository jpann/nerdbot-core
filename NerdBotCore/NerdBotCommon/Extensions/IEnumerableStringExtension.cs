using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NerdBotCommon.Extensions
{
    public static class IEnumerableStringExtension
    {
        public static string OxbridgeAnd(this IEnumerable<String> collection)
        {
            var output = String.Empty;

            var list = collection.ToList();

            if (list.Count > 1)
            {
                var delimited = String.Join(", ", list.Take(list.Count - 1));

                output = String.Concat(delimited, ", and ", list.LastOrDefault());
            }
            else
            {
                output = list.FirstOrDefault();
            }

            return output;
        }

        public static string OxbridgeOr(this IEnumerable<String> collection)
        {
            var output = String.Empty;

            var list = collection.ToList();

            if (list.Count > 1)
            {
                var delimited = String.Join(", ", list.Take(list.Count - 1));

                output = String.Concat(delimited, ", or ", list.LastOrDefault());
            }
            else
            {
                output = list.FirstOrDefault();
            }

            return output;
        }
    }
}
