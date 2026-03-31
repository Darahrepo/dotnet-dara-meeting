using MeetingScheduler.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Common.Interfaces
{
    public interface IUserClaimsService
    {
        Task<int> CreateClaim(bool isPersistent, Guid employeeGuid, Employee emp = null);
        Task AddUpdateClaim(string key, string value);
    }
}
