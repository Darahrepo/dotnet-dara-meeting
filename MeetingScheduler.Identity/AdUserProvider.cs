using MeetingScheduler.Domain.Common.Interfaces;
using MeetingScheduler.Domain.Common.Models;
using MeetingScheduler.Domain.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace MeetingScheduler.Identity
{
    public class AdUserProvider : IUserProvider
    {
		public AdUser CurrentUser { get; set; } = new AdUser();

		IOptions<ADSettings> adSettings = new OptionsWrapper<ADSettings>(new ADSettings());
		public bool Initialized { get; set; }

        public async Task Create(HttpContext context)
        {
            try
            {

                if (context.User.Identity.IsAuthenticated && context.User.FindFirst(ClaimTypes.Role) != null && context.User.Identity != null)
                {
                    //CurrentUser = await GetAdUser(context.User.Identity);
                    //if(CurrentUser.UserId < 1)
                    //{
                    CurrentUser.UserId = Convert.ToInt32(context.User.FindFirst(ClaimTypes.PrimarySid).Value);
                    //CurrentUser.UserRole = context.User.FindFirst(ClaimTypes.Role).Value;
                    CurrentUser.UserRole = (context.User.Identity as ClaimsIdentity).Claims.Where(x => x.Type == "RoleId").FirstOrDefault().Value.ToString();
                    //}
                }
                else
                {
                    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                }
            }
            catch (Exception ex)
            {
				throw new Exception($"Error Creating user", ex);
			}

			Initialized = true;
        }
        
        public Task<AdUser> GetAdUser(IIdentity identity)
        {
            return Task.Run(() =>
            {
                try
                {
                    var test = ContextType.Domain;

                    PrincipalContext context = new PrincipalContext(ContextType.Domain, adSettings.Value.Domain, adSettings.Value.Container);

                    UserPrincipal principal = new UserPrincipal(context);
                    
                    if (context != null)
                    {
                        principal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, identity.Name);
                    }
                    
                    return ADUserProcesses.CastToAdUser(principal);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error retrieving AD User identity:{identity.Name}", ex);
                }
            });
        }
        
        public Task<AdUser> GetAdUser(string samAccountName)
        {
            return Task.Run(() =>
            {
                try
                {
                    PrincipalContext context = new PrincipalContext(ContextType.Domain, adSettings.Value.Domain, adSettings.Value.Container);
                    UserPrincipal user = new UserPrincipal(context);
                    
                    if (context != null)
                    {
                        user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, samAccountName);
                    }
                    if (user != null)
                    {
                        CurrentUser= ADUserProcesses.CastToAdUser(user);
                    }
                    else{ 
                        CurrentUser = null; 
                    }

                    return CurrentUser;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error retrieving AD User", ex);
                }
            });
        }

        public Task<AdUser> GetAdUser(Guid guid)
        {
            return Task.Run(() =>
            {
                try
                {
                    PrincipalContext context = new PrincipalContext(ContextType.Domain, adSettings.Value.Domain, adSettings.Value.Container);
                    UserPrincipal principal = new UserPrincipal(context);
                    
                    if (context != null)
                    {
                        principal = UserPrincipal.FindByIdentity(context, IdentityType.Guid, guid.ToString());
                    }
                    
                    return ADUserProcesses.CastToAdUser(principal);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error retrieving AD User", ex);
                }
            });
        }

        public Task<List<AdUser>> GetDomainUsers()
        {
            return Task.Run(() =>
            {
                PrincipalContext context = new PrincipalContext(ContextType.Domain, adSettings.Value.Domain, adSettings.Value.Container);
                UserPrincipal principal = new UserPrincipal(context);
                principal.UserPrincipalName = "*@*";
                principal.Enabled = true;
                PrincipalSearcher searcher = new PrincipalSearcher(principal);
                var users = searcher.FindAll().AsQueryable().Cast<UserPrincipal>().FilterUsers().SelectAdUsers().OrderBy(u => u.Surname).ToList();

                //List<AdUser> AdUsers = new List<AdUser>();

                //foreach(var user in users)
                //{
                //    //UserPrincipal cast = user;
                //    //AdUser result = new AdUser();
                //    //result = ;
                //    AdUsers.Add(ADUserProcesses.CastToAdUser(user));
                //}

                //var usersTst = searcher
                //    .FindAll()
                //    .AsQueryable()
                //    .Cast<UserPrincipal>()
                //    .FilterUsers()
                //    .SelectAdUsers()
                //    .OrderBy(x => x.Surname)
                //    .ToList();

                //var users = searcher.FindOne().SamAccountName.Where()

                return users;
            });
        }



        public Task<List<AdUser>> FindDomainUser(string search)
        {
            return Task.Run(() =>
            {
                PrincipalContext context = new PrincipalContext(ContextType.Domain, adSettings.Value.Domain, adSettings.Value.Container);
                UserPrincipal principal = new UserPrincipal(context);
                principal.SamAccountName = $"*{search}*";
                principal.Enabled = true;
                PrincipalSearcher searcher = new PrincipalSearcher(principal);

                List<AdUser> users = searcher
                .FindAll()
                .AsQueryable()
                .Cast<UserPrincipal>()
                .FilterUsers()
                .SelectAdUsers()
                .OrderBy(x => x.Surname)
                .ToList();

                return users;
            });

        }

        public AdUser ValidateDomainUser(string username, string password)
        {

            PrincipalContext context = new PrincipalContext(ContextType.Domain, adSettings.Value.Domain, adSettings.Value.Container);
            UserPrincipal principal = new UserPrincipal(context);
            try
            {
			    bool isValid = context.ValidateCredentials(username, password, ContextOptions.Negotiate | ContextOptions.Signing | ContextOptions.Sealing); //, ContextOptions.Negotiate //Third Parameter // DOMAN\\UserName

                if (isValid)
                {
                    if (context != null)
                    {
                        principal = UserPrincipal.FindByIdentity(context, IdentityType.UserPrincipalName, username);
                    }
                }
                else
                {
                    return null;
                }
            } catch(Exception ex)
            {
				throw new Exception("Error validating AD User", ex);
			}

            return ADUserProcesses.CastToAdUser(principal);
        }


        ////Several solutions presented here lack the ability to differentiate between a wrong user / password, 
        ////and a password that needs to be changed.That can be done in the following way:
        //public void ConnectToLDAP()
        //{

        //    try
        //    {
        //        // CN = MerDarah,DC = org,DC = sa
        //        LdapConnection connection = new LdapConnection("Darah.org.sa");
        //        NetworkCredential credential = new NetworkCredential("merna", "P@ssw0rd1");
        //        connection.Credential = credential;
        //        connection.Bind();
        //    }
        //    catch (System.DirectoryServices.Protocols.LdapException lexc)
        //    {
        //        string error = lexc.ServerErrorMessage;
        //    }
        //    catch (Win32Exception ex)
        //    {
        //        switch (ex.NativeErrorCode)
        //        {
        //            case 1326: // ERROR_LOGON_FAILURE (incorrect user name or password)
        //                       // ...
        //            case 1327: // ERROR_ACCOUNT_RESTRICTION
        //                       // ...
        //            case 1330: // ERROR_PASSWORD_EXPIRED
        //                       // ...
        //            case 1331: // ERROR_ACCOUNT_DISABLED
        //                       // ...
        //            case 1907: // ERROR_PASSWORD_MUST_CHANGE
        //                       // ...
        //            case 1909: // ERROR_ACCOUNT_LOCKED_OUT
        //                       // ...
        //            default: // Other
        //                break;
        //        }
        //    }
        //}


    }
}
