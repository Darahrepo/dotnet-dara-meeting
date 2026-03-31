using AutoMapper;
using EmployeeScheduler.Infrastructure.Interfaces;
using Ganss.Xss;
using MeetingScheduler.Domain.Common.Interfaces;
using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Domain.Enums;
using MeetingScheduler.Identity;
using MeetingScheduler.Infrastructure.Interfaces;
using MeetingScheduler.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using MeetingScheduler.Domain.Common.Models;
using MeetingScheduler.UI.Controllers;
using MeetingScheduler.UI;
using NToastNotify;

namespace WebinarScheduler.UI.Controllers
{
    [Authorize]
    public class WebinarController : BaseController
    {
        private readonly int currentUserId;
        private readonly string currentUserRole;
        private readonly IMapper _mapper;
        private readonly IFileServices _fileService;
        private readonly IUserProvider _userProvider;
        private readonly IHtmlSanitizer _htmlSanitizer;
        private readonly IWebinarService _webinarService;
        private readonly IMeetingValidationService _meetingValidation;
        private readonly IEmailService _emailService;
        private readonly IEmailBuilderService _emailBuilderService;
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<WebinarController> _logger;
        private readonly IMeetingRoomService _meetingRoomService;
        private readonly IHtmlLocalizer _localizer;
        private readonly IStringLocalizer<SharedResource> _sharedResource;
        private readonly IStringLocalizer<WebinarController> _webinarControllerResource;
        private readonly IZoomService _zoomService;
        private readonly ILanguageService _languageService;
        private readonly IToastNotification _toastNotification;
        public WebinarController(ILogger<WebinarController> logger, ILanguageService languageService,  IToastNotification toastNotification, IHtmlLocalizer<WebinarController> localizer, IStringLocalizer<WebinarController> webinarControllerResource, IStringLocalizer<SharedResource> sharedResource,
        IEmailBuilderService emailBuilderService, IEmailService emailService, IHtmlSanitizer htmlSanitizer, IFileServices fileService, IWebinarService webinarService,
        IMeetingRoomService meetingRoomService, IUserProvider userProvider, IEmployeeService employeeService, IMapper mapper,
        IZoomService zoomService, IMeetingValidationService meetingValidation)
        {
            _logger = logger;
            _mapper = mapper;
            _languageService = languageService;
            _toastNotification = toastNotification;
            _userProvider = userProvider;
            _webinarService = webinarService;
            _employeeService = employeeService;
            _meetingRoomService = meetingRoomService;
            currentUserId = userProvider.CurrentUser.UserId;
            currentUserRole = userProvider.CurrentUser.UserRole;
            _fileService = fileService;
            _htmlSanitizer = htmlSanitizer;
            _emailService = emailService;
            _emailBuilderService = emailBuilderService;
            _localizer = localizer;
            _sharedResource = sharedResource;
            _meetingValidation = meetingValidation;
            _zoomService = zoomService;
            _webinarControllerResource = webinarControllerResource;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Inside Index Webinar page");
            List<WebinarVm> allWebinars = new List<WebinarVm>();

            TempData["message"] = TempData["message"];
            TempData["type"] = TempData["type"];
            
            if (currentUserRole == Roles.Admin)
            {
                allWebinars = _mapper.Map<List<WebinarVm>>((await _webinarService.GetAllForAdminApproval()));
                return View("Approvals", allWebinars);
            }
            else if (currentUserRole == Roles.Employee)
            {
                allWebinars = _mapper.Map<List<WebinarVm>>(await _webinarService.GetTodaysWebinarsByEmployeeId(currentUserId));
                return View("Index", allWebinars);
            }
            return View();

        }



        public async Task<IActionResult> GetAllWebinarsById()
        {
            var allWebinars = (await _webinarService.GetAllByEmployeeId(currentUserId)).OrderByDescending(x => x.Date).ThenByDescending(x => x.Time_From);
            var WebinarVm = _mapper.Map<List<WebinarVm>>(allWebinars);
            return PartialView("_indexPartial", WebinarVm);
        }


        public async Task<IActionResult> GetWebinarsAsHost()
        {
            var allWebinars = (await _webinarService.GetByEmployeeIdAsHost(currentUserId)).OrderByDescending(x => x.Date).ThenByDescending(x => x.Time_From);
            var WebinarVm = _mapper.Map<List<WebinarVm>>(allWebinars);
            return PartialView("_indexPartial", WebinarVm);
        }

        public async Task<IActionResult> GetWebinarsAsAttendee()
        {
            var allWebinars = (await _webinarService.GetByEmployeeIdAsAttendee(currentUserId)).OrderByDescending(x => x.Date).ThenByDescending(x => x.Time_From);
            var WebinarVm = _mapper.Map<List<WebinarVm>>(allWebinars);
            return PartialView("_indexPartial", WebinarVm);
        }

        public async Task<IActionResult> GetTodaysWebinars_EmployeeFilter()
        {
            var allWebinars = (await _webinarService.GetTodaysWebinarsByEmployeeId(currentUserId));
            var WebinarVm = _mapper.Map<List<WebinarVm>>(allWebinars);
            return PartialView("_indexPartial", WebinarVm);
        }
        public async Task<IActionResult> GetPendingWebinars_EmployeeFilter()
        {
            var allWebinars = (await _webinarService.GetPendingByEmployeeId(currentUserId));
            var WebinarVm = _mapper.Map<List<WebinarVm>>(allWebinars);
            return PartialView("_indexPartial", WebinarVm);
        }

        public async Task<IActionResult> GetScheduledWebinars_EmployeeFilter()
        {
            var allWebinars = (await _webinarService.GetScheduledByEmployeeId(currentUserId));
            var WebinarVm = _mapper.Map<List<WebinarVm>>(allWebinars);
            return PartialView("_indexPartial", WebinarVm);
        }
        public async Task<IActionResult> GetArchivedWebinars_EmployeeFilter()
        {
            var allWebinars = (await _webinarService.GetArchivedByEmployeeId(currentUserId));
            var WebinarVm = _mapper.Map<List<WebinarVm>>(allWebinars);
            return PartialView("_indexPartial", WebinarVm);
        }

        public async Task<IActionResult> GetAllWebinars_EmployeeFilter()
        {
            var allWebinars = (await _webinarService.GetAllByEmployeeId(currentUserId)).Where(x => x.ApprovalStatus != ApprovalStatus.Pending).OrderByDescending(x => x.Date).ThenByDescending(x => x.Time_From);
            var WebinarVm = _mapper.Map<List<WebinarVm>>(allWebinars);
            return PartialView("_indexPartial", WebinarVm);
        }



        /************ Admin Filters *************/

        public async Task<IActionResult> GetApprovedWebinarsToday()
        {
            var allWebinars = (await _webinarService.GetAll()).Where(x => x.ApprovalStatus == ApprovalStatus.Approved && x.Date == DateTime.Now.Date).OrderByDescending(x => x.Date).ThenByDescending(x => x.Time_From);
            var WebinarVm = _mapper.Map<List<WebinarVm>>(allWebinars);
            return PartialView("_indexPartial", WebinarVm);
        }

        public async Task<IActionResult> GetPendingApprovals()
        {
            var allWebinars = (await _webinarService.GetAll()).Where(x => x.ApprovalStatus == ApprovalStatus.Pending && (x.Date > DateTime.Now.Date || (x.Date == DateTime.Now.Date && x.Time_From >= DateTime.Now.TimeOfDay))).OrderByDescending(x => x.Date).ThenByDescending(x => x.Time_From);
            var WebinarVm = _mapper.Map<List<WebinarVm>>(allWebinars);
            return PartialView("_indexPartial", WebinarVm);
        }


        public async Task<IActionResult> GetScheduledWebinars()
        {
            var allWebinars = (await _webinarService.GetAll()).Where(x => x.ApprovalStatus == ApprovalStatus.Approved && x.Date > DateTime.Now.Date).OrderByDescending(x => x.Date).ThenByDescending(x => x.Time_From);
            var WebinarVm = _mapper.Map<List<WebinarVm>>(allWebinars);
            return PartialView("_indexPartial", WebinarVm);
        }

        public async Task<IActionResult> GetArchived()
        {
            var allWebinars = (await _webinarService.GetArchived()).OrderByDescending(x => x.Date).ThenByDescending(x => x.Time_From);
            var WebinarVm = _mapper.Map<List<WebinarVm>>(allWebinars);
            return PartialView("_indexPartial", WebinarVm);
        }


        public async Task<IActionResult> View(int id)
        {
            Webinar allWebinars = null;
            WebinarVm webinarVm = null;

            try
            {
                allWebinars = await _webinarService.GetById(id);
                webinarVm = _mapper.Map<WebinarVm>(allWebinars);

                webinarVm.InternalWebinarPanelists = new List<EmployeeVm>();
                webinarVm.ExternalWebinarPanelists = new List<PanelistVm>();

                var internalAttendees = webinarVm.WebinarPanelists.Where(x => x.AttendeeType == AttendeeType.Internal).ToList();

                if (internalAttendees != null && internalAttendees.Count > 0)
                {
                    foreach (var attendee in internalAttendees)
                    {
                        webinarVm.InternalWebinarPanelists.Add(attendee.Employee);
                    }
                }

                var externalAttendees = webinarVm.WebinarPanelists.Where(x => x.AttendeeType == AttendeeType.External).ToList();

                if (externalAttendees != null && externalAttendees.Count > 0)
                {
                    foreach (var attendee in externalAttendees)
                    {
                        PanelistVm extAtt = new PanelistVm();
                        extAtt.ExternalPanelistNameAr = attendee.ExternalPanelistNameAr;
                        extAtt.ExternalPanelistNameEn = attendee.ExternalPanelistNameEn;
                        extAtt.ExternalPanelistEmailAddress = attendee.ExternalPanelistEmailAddress;
                        webinarVm.ExternalWebinarPanelists.Add(extAtt);
                    }
                }
                if (!(currentUserId == webinarVm.Host.Id) && !(currentUserRole == Roles.Admin) && !webinarVm.IsAttachmentShared)
                {
                    webinarVm.WebinarAttachments = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.InnerException.Message);//.LogInformation(ex.InnerException.Message);
                throw;
            }

            return PartialView("_detailsPartial", webinarVm);
        }


        public async Task<IActionResult> Create()
        {
            WebinarVm webinarInitialDetails = new WebinarVm();

            try
            {
                var employee = await _employeeService.GetEmployeeById(currentUserId);
                webinarInitialDetails.EmployeesList = new List<EmployeeVm>();
                webinarInitialDetails.HostList = new List<EmployeeVm>();
                webinarInitialDetails.HostId = employee.Id;
                var employeeVm = _mapper.Map<EmployeeVm>(employee);
                var allEmployeesVm = _mapper.Map<List<EmployeeVm>>(await _employeeService.GetAllEmployees());

                webinarInitialDetails.LanguagesList = _mapper.Map<List<LanguageVm>>(await _languageService.GetAll());
                if (currentUserRole == Roles.Admin)
                {
                    webinarInitialDetails.HostList = allEmployeesVm;
                    webinarInitialDetails.EmployeesList = allEmployeesVm;
                }
                else if (currentUserRole == Roles.Ceo || currentUserRole == Roles.CeoAssistant)
                {
                    webinarInitialDetails.HostList.Add(employeeVm);
                    webinarInitialDetails.HostList.AddRange(allEmployeesVm.Where(x => x.RoleId != Roles.Ceo).ToList());
                    webinarInitialDetails.EmployeesList = allEmployeesVm;
                }
                else
                {
                    webinarInitialDetails.HostList.Add(employeeVm);
                    webinarInitialDetails.EmployeesList = allEmployeesVm.Where(x => x.Id != currentUserId).ToList();
                }

                webinarInitialDetails.MeetingRoomslist = _mapper.Map<List<MeetingRoomVm>>(await _meetingRoomService.GetAll());
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex.InnerException.Message);
                throw;
            }

            return View(webinarInitialDetails);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WebinarVm webinarVm)
        {
            CancellationToken cancellation = new CancellationToken();
            Webinar webinar = new Webinar();
            var result = 0;
            ToastVm toast = new ToastVm();
            string message = null;
            ToastAlertType type = ToastAlertType.info;
            try
            {

                webinar = _mapper.Map<Webinar>(webinarVm);

                webinar.ApprovalStatus = ApprovalStatus.Approved;

                //Basic Details 

                //Webinar Host
                var employee = await _employeeService.GetEmployeeById(webinarVm.HostId);

                var webinarHost = _mapper.Map<EmployeeVm>(employee);

                webinar.HostId = webinarHost.Id;
                if (employee.RoleId.ToString() == Roles.Ceo || employee.RoleId.ToString() == Roles.CeoAssistant)
                {
                    webinar.ZoomAccount = ZoomUserType.Ceo;
                }
                //InternalAttendees
                if (webinarVm.InternalWebinarPanelistIds!= null && webinarVm.InternalWebinarPanelistIds.Count > 0)
                {
                    foreach (var entity in webinarVm.InternalWebinarPanelistIds)
                    {
                        Panelist meetingObj = new Panelist();
                        meetingObj.AttendeeType = AttendeeType.Internal;
                        meetingObj.EmployeeId = entity;
                        webinar.WebinarPanelists.Add(meetingObj);
                    }
                }

                //ExternalAttendees
                if (webinarVm.ExternalWebinarPanelists != null && webinarVm.ExternalWebinarPanelists.Count > 0)
                {
                    foreach (var entity in webinarVm.ExternalWebinarPanelists)
                    {
                        if (!string.IsNullOrWhiteSpace(entity.ExternalPanelistEmailAddress))
                        {
                            Panelist webinarObj = new Panelist();
                            webinarObj.AttendeeType = AttendeeType.External;
                            webinarObj.ExternalPanelistEmailAddress = entity.ExternalPanelistEmailAddress;
                            webinarObj.ExternalPanelistNameEn = entity.ExternalPanelistNameEn != null ? entity.ExternalPanelistNameEn : "";
                            webinarObj.ExternalPanelistNameAr = entity.ExternalPanelistNameAr != null ? entity.ExternalPanelistNameAr : "";
                            webinar.WebinarPanelists.Add(webinarObj);
                        }
                    }
                }

                if (webinarVm.WebinarRequirementsLists != null && webinarVm.WebinarRequirementsLists.Count > 0)
                {
                    foreach (var req in webinarVm.WebinarRequirementsLists)
                    {
                        if (!string.IsNullOrWhiteSpace(req.Details))
                        {
                            WebinarRequirement requirement = new WebinarRequirement();
                            requirement.Details = req.Details;
                            requirement.WebinarId = webinar.Id;
                            webinar.WebinarRequirements.Add(requirement);
                            
                        }
                    }
                }

                if (webinarVm.Files != null && webinarVm.Files.Count > 0)
                {
                    foreach (var att in webinarVm.Files)
                    {
                        WebinarAttachment attachment = new WebinarAttachment();
                        var fileNameSanitized = _htmlSanitizer.Sanitize(att.FileName);
                        using var fileStream = att.OpenReadStream();
                        attachment.FilePath = await _fileService.SaveFile(fileNameSanitized, fileStream);
                        attachment.FileName = att.FileName;
                        attachment.WebinarId = webinar.Id;
                        webinar.WebinarAttachments.Add(attachment);
                    }
                }

                if(webinarVm.IsTranslationNeeded == false)
                {
                    webinar.Interpreters = null;
                }

                ZoomWebinarDetails details = new ZoomWebinarDetails();
                details.topic = webinar.Subject;
                details.agenda = webinar.Agenda;
                details.ZoomAccount = webinar.ZoomAccount;
                details.start_time = webinar.Date + webinar.Time_From;
                details.recorded_webinar = webinar.IsRecorded;
                details.registration_required = webinar.IsRegistrationNeeded;
                details.duration = (webinar.Time_To - webinar.Time_From).TotalMinutes;

                var zoomDetails = await _zoomService.CreateWebinar(details);
                if (zoomDetails.response_status_code == Convert.ToInt32(ZoomMeetingStatus.success))
                {
                    webinar.ZoomWebinarId = zoomDetails.webinarId;
                    webinar.ZoomWebinarPassword = zoomDetails.password;
                    webinar.WebinarUrl = zoomDetails.starting_url;
                    webinar.RegistrationUrl = zoomDetails.registration_url;
                }
                else
                {
                    toast.Message = _sharedResource["CreateError"];
                    toast.Type = ToastAlertType.error;
                    _logger.LogError("Problem Connecting With Zoom   ====      Error Details:    " + zoomDetails.ToString() + "=======  Webinar Details:    " + details);
                    return Json(new { redirectToUrl = Url.Action("Index", "Webinar") });
                }


                result = await _webinarService.Create(webinar, cancellation);

                if (result > 0)
                {
                    var emailDetails = await _webinarService.GetById(webinar.Id);
                    var mailRequest = await _emailBuilderService.BuildWebinarInvitationEmail(emailDetails);
                    await _emailService.SendEmail(mailRequest);
                }
                
            }
            catch (Exception ex)
            {
                toast.Message = _sharedResource["CreateError"];
                toast.Type = ToastAlertType.error;
                _logger.LogError(ex.InnerException.Message);
                return Json(new { redirectToUrl = Url.Action("Index", "Webinar") });
            }

            toast.Message = _sharedResource["CreateSuccess"];
            toast.Type = ToastAlertType.success;
            TempData["message"] = toast.Message;
            TempData["type"] = toast.Type.ToString();
            return Json(new { redirectToUrl = Url.Action("Index", "Webinar") });
        }


        public async Task<IActionResult> AddRequirementsPartial()
        {
            WebinarVm webinar = new WebinarVm();
            return PartialView("_RequirementsPartial", webinar);
        }


        public async Task<ActionResult> AddPanelistsPartial()
        {
            return PartialView("_ExternalPanelistsPartial");
        }

        public async Task<ActionResult> AddInterpretersPartial()
        {
            List<LanguageVm> languages = new List<LanguageVm>();
            languages = _mapper.Map<List<LanguageVm>>(await _languageService.GetAll());
            WebinarVm webinar = new WebinarVm();
            webinar.LanguagesList = languages;
            return PartialView("_InterpretersPartial", webinar);
        }

        public async Task<IActionResult> ChangeApprovalStatus(int id, string reason, ApprovalStatus status)
        {
            CancellationToken cancellation = new CancellationToken();
            var webinar = await _webinarService.GetById(id);
            var zoomAccount = ZoomUserType.Main;
            if (webinar.Host.RoleId.ToString() == Roles.Ceo)
            {
                zoomAccount = ZoomUserType.Ceo;
            }
            var previousStatus = webinar.ApprovalStatus;
            var result = 0;
            int response = 0;
            string message = null;
            ToastAlertType type = ToastAlertType.info;
            try
            {
                MeetingValidity meetingValidity = new MeetingValidity()
                {
                    Date = webinar.Date,
                    From = webinar.Time_From,
                    To = webinar.Time_To,
                    HostId = webinar.HostId,
                    WebinarId = id
                };

                if (webinar.HostId == currentUserId || _userProvider.CurrentUser.UserRole == Roles.Admin)
                {
                    webinar.ApprovalStatus = status;
                    webinar.Reason = reason;

                    var isTimeSlotAvailable = await _meetingValidation.CheckIfTimeAvailableForHost(meetingValidity);

                    if (status == ApprovalStatus.Approved)
                    {
                        if (isTimeSlotAvailable == MeetingAvailability.Available)
                        {

                            ZoomWebinarDetails details = new ZoomWebinarDetails();
                            details.topic = webinar.Subject;
                            details.agenda = webinar.Agenda;
                            details.start_time = webinar.Date + webinar.Time_From;
                            details.recorded_webinar = webinar.IsRecorded;
                            details.registration_required = webinar.IsRegistrationNeeded;
                            details.duration = (webinar.Time_To - webinar.Time_From).TotalMinutes;
                            //details.RegistrationRequired = webinar.IsRegistrationNeeded;
                            //if (webinar.Interpreters != null && webinar.Interpreters.Count > 0)
                            //{
                            //    details.settings.language_interpretation.enable = true;
                            //    foreach (var inp in webinar.Interpreters)
                            //    {
                            //        Interpreters interpreter = new Interpreters()
                            //        {
                            //            email = inp.EmailAddress,
                            //            languages = $"{inp.FromLanguage.Code},{inp.ToLanguage.Code}"
                            //        };
                            //        details.settings.language_interpretation.interpreters.Add(interpreter);
                            //    }
                            //}
                            var zoomDetails = await _zoomService.CreateWebinar(details);
                            if(zoomDetails.response_status_code == Convert.ToInt32(ZoomMeetingStatus.success))
                            {
                                webinar.ZoomWebinarId = zoomDetails.webinarId;
                                webinar.ZoomWebinarPassword = zoomDetails.password;
                                webinar.WebinarUrl = zoomDetails.starting_url;
                                webinar.RegistrationUrl = zoomDetails.registration_url;

                                result = await _webinarService.Update(webinar);
                            }
                            else
                            {
                                result = 0;
                            }
                        }
                        else
                        {
                            message = _sharedResource[$"{isTimeSlotAvailable}"];
                            type = ToastAlertType.error;
                            TempData["message"] = message;
                            TempData["type"] = type.ToString();
                            return Json(new { Type = type.ToString(), Message = message });
                        }

                    }
                    else if(status == ApprovalStatus.Cancelled) 
                    {
                        result = await _webinarService.Cancel(id,cancellation);
                    }
                    else if (status == ApprovalStatus.Rejected)
                    {
                        result = await _webinarService.Update(webinar);
                    }

                    if (result > 0)
                    {
                        if (status == ApprovalStatus.Approved)
                        {
                            var mailRequest = await _emailBuilderService.BuildWebinarInvitationEmail(webinar);
                            await _emailService.SendEmail(mailRequest);
                        }
                        else if (status == ApprovalStatus.Rejected)
                        {
                            var mailRequest = await _emailBuilderService.BuildWebinarRejectionEmail(webinar);
                            await _emailService.SendEmail(mailRequest);
                        }
                        else if(status == ApprovalStatus.Cancelled)
                        {
                            if ((!string.IsNullOrWhiteSpace(webinar.ZoomWebinarId)))
                            {
                                response = await _zoomService.CancelWebinar(webinar.ZoomWebinarId, zoomAccount);
                                if (response.ToString() == StatusCodes.Status204NoContent.ToString() || response.ToString() == StatusCodes.Status200OK.ToString())
                                {
                                    message = _sharedResource["CancelSuccess"];
                                    type = ToastAlertType.success;
                                }
                                else
                                {
                                    message = _sharedResource["CancelError"];
                                    type = ToastAlertType.error;
                                }
                            }
                            var mailRequest = await _emailBuilderService.BuildWebinarCancellationEmail(webinar, previousStatus);
                            await _emailService.SendEmail(mailRequest);
                        }
                        message = _sharedResource["StatusUpdated"];
                        type = ToastAlertType.success;
                    }
                    else
                    {
                        message = _sharedResource["EditError"];
                        type = ToastAlertType.error;
                    }
                }
                else
                {
                    message = _sharedResource["NoPermissionToUpdate"];
                    type = ToastAlertType.info;
                }

                TempData["message"] = message;
                TempData["type"] = $"{type}";

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.InnerException.Message);
            }
            return Json(new { Type = type.ToString(), Message = message });
        }


        public async Task<IActionResult> Cancel(int id)
        {
            var message = "";
            var type = ToastAlertType.error;
            int response = 0;
            CancellationToken cancellation = new CancellationToken();
            try
            {
                var webinar = (await _webinarService.GetById(id));
                var previousStatus = webinar.ApprovalStatus;
                var result = await _webinarService.Cancel(id, cancellation);
                if (result > 0)
                {
                    if ((!string.IsNullOrWhiteSpace(webinar.ZoomWebinarId)))
                    {
                        response = await _zoomService.CancelWebinar(webinar.ZoomWebinarId);
                        if (response.ToString() == StatusCodes.Status204NoContent.ToString() || response.ToString() == StatusCodes.Status200OK.ToString())
                        {
                            message = _sharedResource["CancelSuccess"];
                            type = ToastAlertType.success;
                        }
                        else
                        {
                            message = _sharedResource["CancelError"];
                            type = ToastAlertType.error;
                        }
                    }
                    if (previousStatus == ApprovalStatus.Approved)
                    {
                        var mailRequest = await _emailBuilderService.BuildWebinarCancellationEmail(webinar,previousStatus);
                        await _emailService.SendEmail(mailRequest);
                    }

                    message = _sharedResource["CancelSuccess"];
                    type = ToastAlertType.success;
                }
                else
                {
                    message = _sharedResource["CancelError"];
                    type = ToastAlertType.error;
                }
            }
            catch (Exception ex)
            {
                message = _sharedResource["CancelError"];
                type = ToastAlertType.error;
                _logger.LogError(ex.InnerException.Message);
            }

            return Json(message != null ? new ToastVm { Type = type, Message = message } : null);
        }





        [HttpPost]
        public async Task<JsonResult> CheckAvailability(DateTime date, TimeSpan to, TimeSpan from, int hostId, int webinarId = 0)
        {
            MeetingValidity meeting = new MeetingValidity { Date = date, To = to, From = from, HostId = hostId, WebinarId = webinarId };
            MeetingAvailability meetingCanBeReserved;
            var canReserve = false;
            string message = "";
            var data = new { meetingCanBeReserved = true, message = "" };
            var host = await _employeeService.GetEmployeeById(hostId);
            if (host.RoleId.ToString() == Roles.Ceo)
            {
                meetingCanBeReserved = await _meetingValidation.CheckIfTimeAvailableForHost(meeting,ZoomUserType.Ceo);
            }
            else
                meetingCanBeReserved = await _meetingValidation.CheckIfTimeAvailableForHost(meeting);

            switch (meetingCanBeReserved)
            {
                case MeetingAvailability.HostHasConflictingMeeting:
                    message = _sharedResource["HostHasConflictingMeeting"];
                    break;
                case MeetingAvailability.HostHasConflictingWebinar:
                    message = _sharedResource["HostHasConflictingWebinar"];
                    break;
                case MeetingAvailability.TimingNotAvailable:
                    message = _sharedResource["TimingNotAvailable"];
                    break;
                case MeetingAvailability.HostHasConflictingZoomMeeting:
                    message = _sharedResource["HostHasConflictingZoomMeeting"];
                    break;
                case MeetingAvailability.HostHasConflictingZoomWebinar:
                    message = _sharedResource["HostHasConflictingZoomWebinar"];
                    break;
                case MeetingAvailability.IssueAuthorizingUser:
                    message = _sharedResource["IssueAuthorizingUser"];
                    break;
                case MeetingAvailability.Available:
                    canReserve = true;
                    break;
            }


            data = new { meetingCanBeReserved = canReserve, message = message };
            return Json(data);
        }
    }
}
