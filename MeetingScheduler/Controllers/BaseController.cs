using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace MeetingScheduler.UI.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            if (userAgent.Contains("MSIE") || userAgent.Contains("Trident"))
            {
                filterContext.Result = base.RedirectToAction("BrowserNotSupported", "Error");
            }
            else
            {
                ViewBag.DisplayName = HttpContext.User.FindFirst(ClaimTypes.GivenName).Value;
            }
            base.OnActionExecuting(filterContext);
        }

    }
}
