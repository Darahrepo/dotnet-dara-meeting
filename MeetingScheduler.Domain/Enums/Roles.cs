using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Enums
{
    public class Roles
    {
        public const string Admin = "1";
        public const string Employee = "2";
        public const string Ceo = "3";
        public const string CeoAssistant = "4";

        public static string GetRoles(string code)
        {
            foreach (var field in typeof(Roles).GetFields())
            {
                if ((string)field.GetValue(null) == code)
                    return field.Name.ToString();
            }
            return "";
        }
    }
}
