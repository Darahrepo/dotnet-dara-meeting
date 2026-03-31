using System.Linq;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;
using System.Collections.Generic;
using System.Security.Claims;

namespace MeetingScheduler.Identity
{
    public static class IdentityExtensions
    {
        public static IQueryable<UserPrincipal> FilterUsers(this IQueryable<UserPrincipal> principals) =>
            principals.Where(x => x.Guid.HasValue && x.EmailAddress != null);

        public static IQueryable<AdUser> SelectAdUsers(this IQueryable<UserPrincipal> principals) {
            var result = principals.Select(x => ADUserProcesses.CastToAdUser(x));
            return result;
        }

        public static string GetSpecificClaim(this ClaimsIdentity claimsIdentity, string claimType)
        {
            var claim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == claimType);

            return (claim != null) ? claim.Value : string.Empty;
        }
    }
}