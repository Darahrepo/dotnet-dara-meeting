using System.Linq;
using System.DirectoryServices.AccountManagement;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;

namespace MeetingScheduler.Identity
{
    public class ADUserProcesses
    {
        private static AdUser adUser = new AdUser();
        public static AdUser CastToAdUser(UserPrincipal principal)
        {
            var user = new AdUser();
            user.AccountLockoutTime = principal.AccountLockoutTime;
            user.BadLogonCount = principal.BadLogonCount;
            user.Description = principal.Description;
            user.DisplayName = principal.DisplayName;
            user.DistinguishedName = principal.DistinguishedName;
            user.Email = principal.EmailAddress;
            user.EmployeeId = principal.EmployeeId;
            user.Enabled = principal.Enabled;
            user.GivenName = principal.GivenName;
            user.Guid = principal.Guid;
            //adUser.HomeDirectory = user.HomeDirectory;
            //adUser.HomeDrive = user.HomeDrive;
            //adUser.LastBadPasswordAttempt = user.LastBadPasswordAttempt;
            //adUser.LastLogon = user.LastLogon;
            //adUser.LastPasswordSet = user.LastPasswordSet;
            user.MiddleName = principal.MiddleName;
            user.Name = principal.Name;
            //adUser.PasswordNeverExpires = user.PasswordNeverExpires;
            //adUser.PasswordNotRequired = user.PasswordNotRequired;
            user.SamAccountName = principal.SamAccountName;
            //adUser.ScriptPath = user.ScriptPath;
            //adUser.Sid = user.Sid;
            user.Surname = principal.Surname;
            //adUser.UserCannotChangePassword = user.UserCannotChangePassword;
            user.UserPrincipalName = principal.UserPrincipalName;
            //adUser.VoiceTelephoneNumber = user.VoiceTelephoneNumber;
            adUser = user;
            return user;

        }

        public string GetDomainPrefix() => adUser.DistinguishedName
            .Split(';')
            .FirstOrDefault(x => x.ToLower().Contains("dc"))
            .Split('=')
            .LastOrDefault()
            .ToUpper();
    }

}
