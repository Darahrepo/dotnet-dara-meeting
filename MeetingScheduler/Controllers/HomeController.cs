using EmployeeScheduler.Infrastructure.Interfaces;
using MeetingScheduler.Domain.Common.Interfaces;
using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Domain.Enums;
using MeetingScheduler.Identity;
using MeetingScheduler.Infrastructure.Interfaces;
using MeetingScheduler.UI.Controllers;
using MeetingScheduler.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;


namespace MeetingScheduler.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmployeeService _employeeService;
        private readonly IMeetingService _meetingService;
        private readonly IUserProvider _userProvider;
        private readonly IDateTimeService _dateTimeService;
        private readonly IWebinarService _webinarService;
        public static DashboardVm _dashboard;
        public HomeController(ILogger<HomeController> logger, IWebinarService webinarService, IDateTimeService dateTimeService, IMeetingService meetingService, IEmployeeService employeeService, IUserProvider userProvider)
        {
            _logger = logger;
            _employeeService = employeeService;
            _userProvider = userProvider;
            _meetingService = meetingService;
            _webinarService = webinarService;
            _dateTimeService = dateTimeService;
            _dashboard = new DashboardVm();
        }

        public async Task<IActionResult> Index()
        {
            var dashboardDetails = await DashboardDetails();
            return View(dashboardDetails);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public async Task<DashboardVm> DashboardDetails()
        {
            dynamic calenderEvents = new List<ExpandoObject>();

            if (_userProvider.CurrentUser.UserRole == Roles.Admin)
            {
                await getAdminDashboardCalculations();
            }
            else if(_userProvider.CurrentUser.UserRole == Roles.Ceo|| _userProvider.CurrentUser.UserRole == Roles.CeoAssistant)
            {
                await getCeoDashboardCalculations();
            }
            else
            {
                await getEmployeeDashboardCalculations();
            }

            foreach (var meeting in _dashboard.Meetings)
            {
                //CalenderEvent MeetingDetail = new CalenderEvent();
                dynamic MeetingDetail = new ExpandoObject();
               //// Pending ////
                if((meeting.Date + meeting.Time_From) >= _dateTimeService.Now && meeting.ApprovalStatus == ApprovalStatus.Pending)
                {
                    MeetingDetail.id = meeting.Id;
                    MeetingDetail.backgroundColor = "#ffc107";
                    MeetingDetail.borderColor = "#ffc107";
                    MeetingDetail.start = (meeting.Date + meeting.Time_From).ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    MeetingDetail.end = (meeting.Date + meeting.Time_To).ToString("yyyy-MM-dd HH:mm",CultureInfo.InvariantCulture);
                    MeetingDetail.title = meeting.Subject;
                    MeetingDetail.type = "Meeting";
                    if (meeting.MeetingLink != null)
                    {
                        MeetingDetail.url = meeting.MeetingLink;
                    }
                }
                //// Today ////
                else if ((meeting.Date == _dateTimeService.Now.Date && meeting.Time_To >= _dateTimeService.Now.TimeOfDay) && meeting.ApprovalStatus == ApprovalStatus.Approved)
                {
                    MeetingDetail.id = meeting.Id;
                    MeetingDetail.backgroundColor = "#28a745";
                    MeetingDetail.borderColor = "#28a745";
                    MeetingDetail.start = (meeting.Date + meeting.Time_From).ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    MeetingDetail.end = (meeting.Date + meeting.Time_To).ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    MeetingDetail.title = meeting.Subject;
                    MeetingDetail.type = "Meeting";
                    if (meeting.MeetingLink != null)
                    {
                        MeetingDetail.url = meeting.MeetingLink;
                    }
                }
                //// Scheduled ////
                else if (meeting.ApprovalStatus == ApprovalStatus.Approved && (meeting.Date) > _dateTimeService.Now.Date)
                {
                    MeetingDetail.id = meeting.Id;
                    MeetingDetail.backgroundColor = "#17a2b8";
                    MeetingDetail.borderColor = "#17a2b8";
                    MeetingDetail.start = (meeting.Date + meeting.Time_From).ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    MeetingDetail.end = (meeting.Date + meeting.Time_To).ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    MeetingDetail.title = meeting.Subject;
                    MeetingDetail.type = "Meeting";
                    if (meeting.MeetingLink != null)
                    {
                        MeetingDetail.url = meeting.MeetingLink;
                    }
                }
                //// Meeting Passed ////
                else if ((meeting.Date + meeting.Time_To) < _dateTimeService.Now && meeting.ApprovalStatus == ApprovalStatus.Approved)
                {
                    MeetingDetail.id = meeting.Id;
                    MeetingDetail.backgroundColor = "#6c757d";
                    MeetingDetail.borderColor = "#6c757d";
                    MeetingDetail.start = (meeting.Date + meeting.Time_From).ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    MeetingDetail.end = (meeting.Date + meeting.Time_To).ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    MeetingDetail.title = meeting.Subject;
                    MeetingDetail.type = "Meeting";
                    if (meeting.MeetingLink != null)
                    {
                        MeetingDetail.url = meeting.MeetingLink;
                    }
                }
                
                calenderEvents.Add(MeetingDetail);
            }

            foreach (var webinar in _dashboard.Webinars)
            {
                //CalenderEvent MeetingDetail = new CalenderEvent();
                dynamic webinarDetails = new ExpandoObject();
                //// Pending ////
                if ((webinar.Date + webinar.Time_From) >= _dateTimeService.Now && webinar.ApprovalStatus == ApprovalStatus.Pending)
                {
                    webinarDetails.id = webinar.Id;
                    webinarDetails.backgroundColor = "#ffc107";
                    webinarDetails.borderColor = "#ffc107";
                    webinarDetails.textColor = "#28a745";
                    webinarDetails.start = (webinar.Date + webinar.Time_From).ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    webinarDetails.end = (webinar.Date + webinar.Time_To).ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    webinarDetails.title = webinar.Subject;
                    webinarDetails.type = "Webinar";
                    if (webinar.WebinarUrl != null)
                    {
                        webinarDetails.url = webinar.WebinarUrl;
                    }
                }
                //// Today ////
                else if ((webinar.Date == _dateTimeService.Now.Date && webinar.Time_To >= _dateTimeService.Now.TimeOfDay) && webinar.ApprovalStatus == ApprovalStatus.Approved)
                {
                    webinarDetails.id = webinar.Id;
                    webinarDetails.backgroundColor = "#28a745";
                    webinarDetails.borderColor = "#28a745";
                    webinarDetails.textColor = "#28a745";
                    webinarDetails.start = (webinar.Date + webinar.Time_From).ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    webinarDetails.end = (webinar.Date + webinar.Time_To).ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    webinarDetails.title = webinar.Subject;
                    webinarDetails.type = "Webinar";
                    if (webinar.WebinarUrl != null)
                    {
                        webinarDetails.url = webinar.WebinarUrl;
                    }
                }
                //// Scheduled ////
                else if (webinar.ApprovalStatus == ApprovalStatus.Approved && (webinar.Date) > _dateTimeService.Now.Date)
                {
                    webinarDetails.id = webinar.Id;
                    webinarDetails.backgroundColor = "#17a2b8";
                    webinarDetails.borderColor = "#17a2b8";
                    webinarDetails.textColor = "#28a745";
                    webinarDetails.start = (webinar.Date + webinar.Time_From).ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    webinarDetails.end = (webinar.Date + webinar.Time_To).ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    webinarDetails.title = webinar.Subject;
                    webinarDetails.type = "Webinar";
                    if (webinar.WebinarUrl != null)
                    {
                        webinarDetails.url = webinar.WebinarUrl;
                    }
                }
                //// Meeting Passed ////
                else if ((webinar.Date + webinar.Time_To) < _dateTimeService.Now && webinar.ApprovalStatus == ApprovalStatus.Approved)
                {
                    webinarDetails.id = webinar.Id;
                    webinarDetails.backgroundColor = "#6c757d";
                    webinarDetails.borderColor = "#6c757d";
                    webinarDetails.textColor = "#28a745";
                    webinarDetails.start = (webinar.Date + webinar.Time_From).ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    webinarDetails.end = (webinar.Date + webinar.Time_To).ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    webinarDetails.title = webinar.Subject;
                    webinarDetails.type = "Webinar";
                    if (webinar.WebinarUrl != null)
                    {
                        webinarDetails.url = webinar.WebinarUrl;
                    }
                }

                calenderEvents.Add(webinarDetails);
            }
            _dashboard.CalenderEvents = JsonSerializer.Serialize(calenderEvents);

            return _dashboard;
        }
        public async Task getAdminDashboardCalculations()
        {
            _dashboard.Meetings = (await _meetingService.GetAll()).OrderByDescending(x => x.ApprovalStatus == ApprovalStatus.Pending).ToList();
            _dashboard.Webinars = (await _webinarService.GetAll()).OrderByDescending(x => x.ApprovalStatus == ApprovalStatus.Pending).ToList();

            int MeetingsPending = _dashboard.Meetings.Where(x => (x.Date + x.Time_From) >= _dateTimeService.Now && x.ApprovalStatus == ApprovalStatus.Pending).Count();
            int MeetingsToday = _dashboard.Meetings.Where(x => (x.Date == _dateTimeService.Now.Date && x.Time_To >= _dateTimeService.Now.TimeOfDay) && x.ApprovalStatus == ApprovalStatus.Approved).Count();
            int MeetingsScheduled = _dashboard.Meetings.Where(x => x.Date > _dateTimeService.Now.Date && x.ApprovalStatus == ApprovalStatus.Approved).Count();
            int MeetingsDone = _dashboard.Meetings.Where(x => (x.Date + x.Time_To) < _dateTimeService.Now && x.ApprovalStatus == ApprovalStatus.Approved).Count();

            int WebinarsPending = _dashboard.Webinars.Where(x => (x.Date + x.Time_From) >= _dateTimeService.Now && x.ApprovalStatus == ApprovalStatus.Pending).Count();
            int WebinarsToday = _dashboard.Webinars.Where(x => (x.Date == _dateTimeService.Now.Date && x.Time_To >= _dateTimeService.Now.TimeOfDay) && x.ApprovalStatus == ApprovalStatus.Approved).Count();
            int WebinarsScheduled = _dashboard.Webinars.Where(x => x.Date > _dateTimeService.Now.Date && x.ApprovalStatus == ApprovalStatus.Approved).Count();
            int WebinarsDone = _dashboard.Webinars.Where(x => (x.Date + x.Time_To) < _dateTimeService.Now && x.ApprovalStatus == ApprovalStatus.Approved).Count();

            _dashboard.MeetingsDone = MeetingsDone + WebinarsDone;
            _dashboard.MeetingsToday = MeetingsToday + WebinarsToday;
            _dashboard.MeetingsScheduled = MeetingsScheduled + WebinarsScheduled;
            _dashboard.MeetingsPending = MeetingsPending + WebinarsPending;
        }

        public async Task getCeoDashboardCalculations()
        {
            _dashboard.Meetings = ((await _meetingService.GetAllCeo(_userProvider.CurrentUser.UserId))).OrderByDescending(x => x.ApprovalStatus == ApprovalStatus.Pending).ToList();
            _dashboard.Webinars = (await _webinarService.GetAllCeo(_userProvider.CurrentUser.UserId)).OrderByDescending(x => x.ApprovalStatus == ApprovalStatus.Pending).ToList();

            int MeetingsPending = _dashboard.Meetings.Where(x => (x.Date + x.Time_From) >= _dateTimeService.Now && x.ApprovalStatus == ApprovalStatus.Pending).Count();
            int MeetingsToday = _dashboard.Meetings.Where(x => (x.Date == _dateTimeService.Now.Date && x.Time_To >= _dateTimeService.Now.TimeOfDay) && x.ApprovalStatus == ApprovalStatus.Approved).Count();
            int MeetingsScheduled = _dashboard.Meetings.Where(x => x.Date > _dateTimeService.Now.Date && x.ApprovalStatus == ApprovalStatus.Approved).Count();
            int MeetingsDone = _dashboard.Meetings.Where(x => (x.Date + x.Time_To) < _dateTimeService.Now && x.ApprovalStatus == ApprovalStatus.Approved).Count();

            int WebinarsPending = _dashboard.Webinars.Where(x => (x.Date + x.Time_From) >= _dateTimeService.Now && x.ApprovalStatus == ApprovalStatus.Pending).Count();
            int WebinarsToday = _dashboard.Webinars.Where(x => (x.Date == _dateTimeService.Now.Date && x.Time_To >= _dateTimeService.Now.TimeOfDay) && x.ApprovalStatus == ApprovalStatus.Approved).Count();
            int WebinarsScheduled = _dashboard.Webinars.Where(x => x.Date > _dateTimeService.Now.Date && x.ApprovalStatus == ApprovalStatus.Approved).Count();
            int WebinarsDone = _dashboard.Webinars.Where(x => (x.Date + x.Time_To) < _dateTimeService.Now && x.ApprovalStatus == ApprovalStatus.Approved).Count();

            _dashboard.MeetingsDone = MeetingsDone + WebinarsDone;
            _dashboard.MeetingsToday = MeetingsToday + WebinarsToday;
            _dashboard.MeetingsScheduled = MeetingsScheduled + WebinarsScheduled;
            _dashboard.MeetingsPending = MeetingsPending + WebinarsPending;
        }

        public async Task getEmployeeDashboardCalculations()
        {

            _dashboard.Meetings = (await _meetingService.GetAllByEmployeeId(_userProvider.CurrentUser.UserId)).OrderByDescending(x => x.ApprovalStatus == ApprovalStatus.Pending).ToList();
            _dashboard.Webinars = (await _webinarService.GetAllByEmployeeId(_userProvider.CurrentUser.UserId)).OrderByDescending(x => x.ApprovalStatus == ApprovalStatus.Pending).ToList();

            int MeetingsPending = _dashboard.Meetings.Where(x => (x.Date + x.Time_From) >= _dateTimeService.Now && x.ApprovalStatus == ApprovalStatus.Pending).Count();
            int MeetingsToday = _dashboard.Meetings.Where(x => (x.Date == _dateTimeService.Now.Date && x.Time_To >= _dateTimeService.Now.TimeOfDay) && x.ApprovalStatus == ApprovalStatus.Approved).Count();
            int MeetingsScheduled = _dashboard.Meetings.Where(x => x.Date > _dateTimeService.Now.Date && x.ApprovalStatus == ApprovalStatus.Approved).Count();
            int MeetingsDone = _dashboard.Meetings.Where(x => (x.Date + x.Time_To) < _dateTimeService.Now && x.ApprovalStatus == ApprovalStatus.Approved).Count();

            int WebinarsPending = _dashboard.Webinars.Where(x => (x.Date + x.Time_From) >= _dateTimeService.Now && x.ApprovalStatus == ApprovalStatus.Pending).Count();
            int WebinarsToday = _dashboard.Webinars.Where(x => (x.Date == _dateTimeService.Now.Date && x.Time_To >= _dateTimeService.Now.TimeOfDay) && x.ApprovalStatus == ApprovalStatus.Approved).Count();
            int WebinarsScheduled = _dashboard.Webinars.Where(x => x.Date > _dateTimeService.Now.Date && x.ApprovalStatus == ApprovalStatus.Approved).Count();
            int WebinarsDone = _dashboard.Webinars.Where(x => (x.Date + x.Time_To) < _dateTimeService.Now && x.ApprovalStatus == ApprovalStatus.Approved).Count();

            _dashboard.MeetingsDone = MeetingsDone + WebinarsDone;
            _dashboard.MeetingsToday = MeetingsToday + WebinarsToday;
            _dashboard.MeetingsScheduled = MeetingsScheduled + WebinarsScheduled;
            _dashboard.MeetingsPending = MeetingsPending + WebinarsPending;
        }

        public IActionResult ChangeLanguage(string returnUrl)
        {
                var culture = HttpContext.Features.Get<IRequestCultureFeature>();
                if (culture.RequestCulture.Culture.Name == "en")
                {
                    CultureInfo cInfo = new CultureInfo("ar");
                    CultureInfo cInfoInvarian = new CultureInfo("ar");
                    cInfoInvarian.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy"; //To avoid datetime conversion issues on systems with AR as default lang
                    cInfo.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
                    cInfoInvarian.DateTimeFormat.Calendar = new GregorianCalendar();
                    cInfo.DateTimeFormat.Calendar = new GregorianCalendar();
                    Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(cInfoInvarian, cInfo)),
                                    new CookieOptions { Expires = DateTimeOffset.Now.AddDays(60) });
                }
                else
                {
                    Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture("en")),
                                    new CookieOptions { Expires = DateTimeOffset.Now.AddDays(60) });
                }


                return LocalRedirect(returnUrl);
        }
    }
}

