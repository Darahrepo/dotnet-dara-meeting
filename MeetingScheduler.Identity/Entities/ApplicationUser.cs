using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MeetingScheduler.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public int UserId { get; set; }
    }
}
