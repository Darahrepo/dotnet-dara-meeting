using MeetingScheduler.Infrastructure.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingScheduler.UI.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        
        public IActionResult Error([Bind(Prefix = "id")] int statusCode = 0)
        {
            statusCode = HttpContext.Response.StatusCode;
            //var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            //var exception = context.Error; // Your exception
            var page = RedirectToAction("");
            switch (statusCode)
            {
                case 200:
                    return RedirectToAction("Login", "Account");
                case 401:
                    return RedirectToAction("Login", "Account");
                case 403:
                    return RedirectToAction("AccessDenied","Account");
                case 400:
                    return View("Error400");
                case 500:
                    return View("Error500");
            }

            return null;
        }
        public IActionResult Error500()
        {
            
            return View("500");
        }
        public IActionResult Error403()
        {
            
            return View("500");
        }

        public IActionResult BrowserNotSupported()
        {

            return View();
        }
    }
}
