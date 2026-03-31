using System;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Domain.Common.Interfaces;
using EmployeeScheduler.Infrastructure.Interfaces;
using System.Security.Principal;
using System.Linq;
using MeetingScheduler.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using MeetingScheduler.Domain.Enums;

namespace MeetingScheduler.Infrastructure.Common.Services
{
    public class UserClaimsService : IUserClaimsService
    {
        private readonly IHttpContextAccessor _httpContext;
        public UserClaimsService(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public async Task<int> CreateClaim(bool isPersistent, Guid employeeGuid , Employee emp = null)
        {
            var claims = new List<Claim>();
            try
            {
                if (employeeGuid == Guid.Empty && emp == null)
                {
                    return 0;

                }
                var UserIdentity = _httpContext.HttpContext.User.Identity;

                claims = new List<Claim>();

                claims.Add(new Claim("Authenticated", "True"));

                claims.Add(new Claim(ClaimTypes.PrimarySid, emp.Id.ToString()));

                claims.Add(new Claim(ClaimTypes.GivenName, emp.DisplayName));

                claims.Add(new Claim(ClaimTypes.Name, UserIdentity.Name));

                claims.Add(new Claim(ClaimTypes.Role, Roles.GetRoles(emp.RoleId.ToString())));

                claims.Add(new Claim("RoleId", emp.RoleId.ToString()));

                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                await _httpContext.HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties
                    {
                        IsPersistent = false,
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                    });

                return 1;
            }
            catch (Exception ex)
            {
                // Info  
                throw ex;
            }
        }

        public async Task AddUpdateClaim(string key, string value)
        {
            var identity = _httpContext.HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
                return;

            // check for existing claim and remove it
            var existingClaim = identity.FindFirst(key);
            if (existingClaim != null)
                identity.RemoveClaim(existingClaim);

            // add new claim
            identity.AddClaim(new Claim(key, value));
            await _httpContext.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = false,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                });
            //var authenticationManager = _httpContext.HttpContext.Current.GetOwinContext().Authentication;
            //authenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(identity), new AuthenticationProperties() { IsPersistent = true });
        }
    }
    
}
