using MeetingScheduler.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Infrastructure.Common
{
    public class AuthorizeRoles : AuthorizeAttribute
    {
        public AuthorizeRoles(params Roles[] roles)
        {
            
        }

    }
}
