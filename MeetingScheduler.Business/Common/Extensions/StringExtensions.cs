using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MeetingScheduler.Infrastructure.Common.Extensions
{
    public static class StringExtensions
    {
        public static async Task<bool> BeInArabic(this String str)
        {
            Regex regex = new Regex("[\u0600 - \06ff\\s0-9]*");

            return regex.IsMatch(str);
        }

        public static async Task<bool> BeInEnglish(this String str)
        {
            Regex regex = new Regex("^[A-Za-z ]+$");

            return regex.IsMatch(str);
        }
    }
}
