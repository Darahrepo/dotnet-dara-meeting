using System;
using System.Threading.Tasks;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace MeetingScheduler.Identity
{
    public interface IUserProvider
    {
        AdUser CurrentUser { get; set; }
        bool Initialized { get; set; }
        Task Create(HttpContext context);
        Task<AdUser> GetAdUser(IIdentity identity);
        Task<AdUser> GetAdUser(string samAccountName);
        Task<AdUser> GetAdUser(Guid guid);
        Task<List<AdUser>> GetDomainUsers();
        Task<List<AdUser>> FindDomainUser(string search);
        AdUser ValidateDomainUser(string username, string Password);
    }
}
