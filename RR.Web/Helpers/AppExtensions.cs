using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Web
{
    public static class AppExtensions
    {
        /// <summary>
        /// Get Time Ago  
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string GetTimeAgo(this DateTime date)
        {
            if (date != null)
            {
                var date_ = DateTime.Now;
                TimeSpan diff = date_.Subtract(Convert.ToDateTime(date));

                if (diff.Days > 0)
                    return $"{diff.Days} days ago";

                if (diff.Hours > 0)
                    return $"{diff.Hours} hours ago";

                if (diff.Minutes > 0)
                    return $"{diff.Minutes} Minutes ago";

                if (diff.Seconds > 0)
                    return $"{diff.Seconds} seconds ago";

            }

            return "";
        }

        public static string Truncate(this string input, int length)
        {
            if (input == null || input.Length < length)
                return input;
            int iNextSpace = input.LastIndexOf(" ", length, StringComparison.Ordinal);
            return string.Format("{0}…", input.Substring(0, (iNextSpace > 0) ? iNextSpace : length).Trim());
        }

    }
}
