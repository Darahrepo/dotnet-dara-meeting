using MeetingScheduler.Identity;
using MeetingScheduler.Infrastructure.Interfaces;
using MeetingScheduler.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using MeetingScheduler.Infrastructure.Services.Employees;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading;
using EmployeeScheduler.Infrastructure.Interfaces;
using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Domain.Enums;
using MeetingScheduler.Domain.Common.Interfaces;
using Microsoft.Extensions.Localization;

namespace MeetingScheduler.UI.Controllers
{

    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IUserProvider _userProvider; 
        private readonly IUserClaimsService _userClaimsService;
        private readonly IEmployeeService _employeeService;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IStringLocalizer<AccountController> _accountControllerResource;

        public AccountController(ILogger<AccountController> logger, IStringLocalizer<AccountController> accountControllerResource,  IHttpContextAccessor httpContext, IUserProvider userProvider,IEmployeeService employeeService, IUserClaimsService userClaimsService)
        {
            _logger = logger;
            _userProvider = userProvider;
            _employeeService = employeeService;
            _httpContext = httpContext;
            _userClaimsService = userClaimsService;
            _accountControllerResource = accountControllerResource;
        }


        public async Task<IActionResult> Index(string returnUrl = null)
        {
            IActionResult result = View();
            var domainUser = await _userProvider.GetAdUser(HttpContext.User.Identity);
            if (domainUser != null && domainUser.Guid != null)
            {
                result = await Login(null, returnUrl, domainUser);
            }
            else
            {
                result = View("Login");
            }

            return result;
        }


        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVm login = null, string returnUrl = null, AdUser domainUser = null)
        {
            Employee userDetailsExist = null;
            if (domainUser.UserId <= 0 && !ModelState.IsValid)
            {
                return View();
            }
            try { 
                CancellationToken cancellation = new CancellationToken();
                if (login != null && ModelState.IsValid)
                {
                    domainUser = _userProvider.ValidateDomainUser(login.Username, login.Password);
                    if (domainUser == null)
                    {
                        ModelState.AddModelError(nameof(LoginVm.RememberMe), _accountControllerResource["LoginInvalid"]);
                        return View();
                    }
                }
                else if (domainUser != null)
                {
                    //var ifTesting = false;
                    userDetailsExist = await _employeeService.GetEmployeeByGuid((Guid)domainUser.Guid.Value);
                    //if (ifTesting)
                    //{
                    //    var getTestUser = await _employeeService.GetEmployeeById(14);
                    //    userDetailsExist = getTestUser;
                    //}
                    var res = 0;
                    if (userDetailsExist == null)
                    {
                        Employee employee = new Employee
                        {
                            EmailAddress = domainUser.Email,
                            Guid = domainUser.Guid.Value,
                            FirstNameEn = domainUser.DisplayName.Split(" ")[0],
                            MiddleNameEn = domainUser.DisplayName.Split(" ").Length > 1 ? domainUser.DisplayName.Split(" ")[1] : "",
                            LastNameEn = domainUser.DisplayName.Split(" ").Length > 2 ? domainUser.DisplayName.Split(" ")[domainUser.DisplayName.Split(" ").Length - 1] : "",
                            DisplayName = domainUser.DisplayName,
                            RoleId = Convert.ToInt32(Roles.Employee)
                        };
                         
                        res = await _employeeService.CreateEmployee(employee, cancellation);
                        if (res > 0)
                        {
                            Employee emp = new Employee
                            {
                                DisplayName = employee.DisplayName,
                                Id = res,
                                RoleId = employee.RoleId
                            };
                            _userProvider.CurrentUser.UserId = res;
                            _userProvider.CurrentUser.UserRole = employee.RoleId.ToString();
                            var claim = await _userClaimsService.CreateClaim(false, Guid.Empty , emp);
                        }

                    }
                    else
                    {
                        _userProvider.CurrentUser.UserId = userDetailsExist.Id;
                        _userProvider.CurrentUser.UserRole = userDetailsExist.RoleId.ToString();
                        var claim = await _userClaimsService.CreateClaim(false, Guid.Empty , userDetailsExist );
                    }
                }
                //else {
                //    return RedirectToAction("Index","Home",null);

                //}
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) && returnUrl!="/")
                {
                    return Redirect(returnUrl);
                }
            
            }
            catch(Exception Ex)
            {
                _logger.LogError(Ex.Message);
                throw;
            }


            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                //var returnUrl = $"~{HttpContext.Request.Path.Value}{HttpContext.Request.QueryString}";
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> AccessDenied(string returnUrl , int statusCode = 0)
        {
            //if (!string.IsNullOrWhiteSpace(HttpContext.Request.Path))
            //{
            //    var returnUrl = $"~{HttpContext.Request.Path.Value}{HttpContext.Request.QueryString}";
            //    return LocalRedirect(returnUrl);
            //}
            //throw new HttpResponseException(HttpStatusCode.Unauthorized);
            //statusCode = HttpContext.Response.StatusCode; 
            var test = User.FindFirst("Authenticated");
            if (User.FindFirst("Authenticated") == null)//.FindFirst("Authenticated") == null)
                return RedirectToAction("Index", "Account", new { returnUrl = returnUrl });

            return View();
        }


        public async Task<ActionResult> SignOut(string returnUrl = null)
        {
            try
            {
                //var returnUrl = string.IsNullOrWhiteSpace(HttpContext.Request.Path) ? "~/" : $"~{HttpContext.Request.Path.Value}{HttpContext.Request.QueryString}";
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Index", "Account", new { returnUrl = returnUrl });

            }
            catch (Exception ex)
            {
                // Info  
                _logger.LogError(ex.Message);
                throw ex;
            }
        }


        //[AllowAnonymous]
        //public async Task<ActionResult> Login(LoginVm loginCredentials)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return View(loginCredentials);
        //        }

        //        var DomainUser = _userProvider.ValidateDomainUser(loginCredentials.Username, loginCredentials.Password);
        //        if (DomainUser.Guid != null)
        //        {

        //            string userName = loginCredentials.Username.ToString().ToLower(new CultureInfo("en-US", false)); //When UserName is entered in uppercase containing "I", the user cannot be found in LDAP

        //            ApplicationUser user = await _userManager.FindByNameAsync(userName); //Asynchronous method

        //            if (user == null) // If the User DOES NOT exists in the ASP.NET Identity table (AspNetUsers)
        //            {
        //                user = new ApplicationUser();
        //                user.UserName = DomainUser.DisplayName;
        //                user.Email = DomainUser.Email;
        //                user.EmailConfirmed = true;
        //                var res = await _userManager.CreateAsync(user);
        //            }

        //            var userDetailsExist = await _employeeService.GetEmployeeByGuid(DomainUser.Guid.Value);

        //            if (userDetailsExist == null)
        //            {
        //                EmployeeDto employee = new EmployeeDto
        //                {
        //                    EmailAddress = DomainUser.Email,
        //                    Guid = DomainUser.Guid.Value,
        //                    FirstNameEn = DomainUser.GivenName,
        //                    DisplayName = DomainUser.DisplayName
        //                };

        //                CancellationToken cancellationToken = new CancellationToken();

        //                var res = await _employeeService.CreateEmployee(employee, cancellationToken);

        //                if (res > 0)
        //                {
        //                    _userProvider.CurrentUser.UserId = res;
        //                }
        //            }

        //            return RedirectToAction("Index", "Home");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError(nameof(LoginVm.Password), "Username or Password incorrect");
        //            return View("Index", loginCredentials);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        ///* Since ASP.NET Identity and OWIN Cookie Authentication are claims-based system, the framework requires the app to generate a ClaimsIdentity for the user. 
        //ClaimsIdentity has information about all the claims for the user, such as what roles the user belongs to. You can also add more claims for the user at this stage.
        //The highlighted code below in the SignInAsync method signs in the user by using the AuthenticationManager from OWIN and calling SignIn and passing in the ClaimsIdentity. */
        //private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        //{

        //    HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
        //    var identity = await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
        //    HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        //}



        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult SignOut()
        //{
        //    AuthenticationManager.SignOut();
        //    FormsAuthentication.SignOut(); //In order to force logout in LDAP authentication
        //    return RedirectToAction("Login", "Account");
        //}
    }
}
