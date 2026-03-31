using AutoMapper;
using EmployeeScheduler.Infrastructure.Interfaces;
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
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using MeetingScheduler.Domain.Common.Models;
using NToastNotify;
using Ganss.Xss;
using Newtonsoft.Json;

namespace MeetingScheduler.UI.Controllers
{
    [Authorize]
    public class MeetingController : BaseController
    {
        private readonly int currentUserId;
        private readonly string currentUserRole;
        private readonly IMapper _mapper;
        private readonly IFileServices _fileService;
        private readonly IUserProvider _userProvider;
        private readonly IHtmlSanitizer _htmlSanitizer;
        private readonly IMeetingService _meetingService;
        private readonly IEmailService _emailService; 
        private readonly IEmailBuilderService _emailBuilderService;
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<MeetingController> _logger;
        private readonly IMeetingRoomService _meetingRoomService;
        private readonly IMeetingValidationService _meetingValidation;
        private readonly IHtmlLocalizer _localizer;
        private readonly IStringLocalizer<SharedResource> _sharedResource;
        private readonly IStringLocalizer<MeetingController> _meetingControllerResource;
        private readonly IZoomService _zoomService;
        private readonly IWebexService _webexService;
        private readonly ICalendarService _calService;
        private readonly IToastNotification _toastNotification;
        public MeetingController(ILogger<MeetingController> logger, IWebexService webexService, IHtmlLocalizer<MeetingController> localizer, IToastNotification toastNotification, IStringLocalizer<MeetingController> meetingControllerResource, IStringLocalizer<SharedResource> sharedResource, 
        IEmailBuilderService emailBuilderService, IEmailService emailService, IHtmlSanitizer htmlSanitizer, IFileServices fileService, IMeetingService meetingService, 
        IMeetingRoomService meetingRoomService, IUserProvider userProvider, IEmployeeService employeeService, IMapper mapper, ICalendarService calService,
        IZoomService zoomService, IMeetingValidationService meetingValidation)
        {
            currentUserId =  userProvider.CurrentUser.UserId;
            _logger = logger;
            _localizer = localizer;
            _toastNotification = toastNotification;
            _meetingControllerResource = meetingControllerResource;
            _sharedResource = sharedResource;
            _emailBuilderService = emailBuilderService;
            _emailService = emailService;
            _htmlSanitizer = htmlSanitizer;
             _fileService = fileService;
            _webexService = webexService;
            _meetingService = meetingService;
            _meetingRoomService = meetingRoomService;
            currentUserRole = userProvider.CurrentUser.UserRole;
            _userProvider = userProvider;
            _employeeService = employeeService;
            _mapper = mapper;
            _zoomService = zoomService;
            _calService = calService;
            _meetingValidation = meetingValidation;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Inside Index Meeting page");
            List<MeetingVm> allMeetings = new List<MeetingVm>();

            TempData["message"] = TempData["message"];
            TempData["type"] = TempData["type"];

            if(currentUserRole == Roles.Admin)
            {
                allMeetings = _mapper.Map<List<MeetingVm>>((await _meetingService.GetAll()).Where(x => x.ApprovalStatus == ApprovalStatus.Approved && x.Date + x.Time_To > DateTime.Now).OrderByDescending(x => x.CreatedOn).ThenByDescending(x => x.Time_From));
                    
                return View("Approvals", allMeetings);
            }
            else if (currentUserRole == Roles.Employee)
            {
                allMeetings = _mapper.Map<List<MeetingVm>>(await _meetingService.GetScheduledByEmployeeId(currentUserId));
                return View( "Index", allMeetings);
            }
            else if (currentUserRole == Roles.CeoAssistant || currentUserRole == Roles.Ceo)
            {
                allMeetings = _mapper.Map<List<MeetingVm>>((await _meetingService.GetAllCeo(currentUserId)).Where(x => x.ApprovalStatus == ApprovalStatus.Approved && x.Date + x.Time_To > DateTime.Now).OrderByDescending(x => x.CreatedOn).ThenByDescending(x => x.Time_From));
                return View("Index", allMeetings);
            }
            return View();
        }


        /************ Employee Filters *************/
        public async Task<IActionResult> GetMeetingsAsHost()
        {
            var allMeetings = (await _meetingService.GetByEmployeeIdAsHost(currentUserId)).OrderByDescending(x => x.CreatedOn).ThenByDescending(x => x.Time_From);
            var MeetingVm = _mapper.Map<List<MeetingVm>>(allMeetings);
            return PartialView("_indexPartial", MeetingVm);
        }

        public async Task<IActionResult> GetMeetingsAsAttendee()
        {
            var allMeetings = (await _meetingService.GetByEmployeeIdAsAttendee(currentUserId)).OrderByDescending(x => x.CreatedOn).ThenByDescending(x => x.Time_From);
            var MeetingVm = _mapper.Map<List<MeetingVm>>(allMeetings);
            return PartialView("_indexPartial", MeetingVm);
        }

        //public async Task<IActionResult> GetTodaysMeetings_EmployeeFilter()
        //{
        //    var allMeetings = await _meetingService.GetTodaysByEmployeeId(currentUserId);
        //    var MeetingVm = _mapper.Map<List<MeetingVm>>(allMeetings);
        //    return PartialView("_indexPartial", MeetingVm);
        //}
        //public async Task<IActionResult> GetPendingMeetings_EmployeeFilter()
        //{
        //    var allMeetings = (await _meetingService.GetPendingByEmployeeId(currentUserId));
        //    var MeetingVm = _mapper.Map<List<MeetingVm>>(allMeetings);
        //    return PartialView("_indexPartial", MeetingVm);
        //}

        //public async Task<IActionResult> GetScheduledMeetings_EmployeeFilter()
        //{
        //    var allMeetings = (await _meetingService.GetScheduledByEmployeeId(currentUserId));
        //    var MeetingVm = _mapper.Map<List<MeetingVm>>(allMeetings);
        //    return PartialView("_indexPartial", MeetingVm);
        //}

        //public async Task<IActionResult> GetArchived_EmployeeFilter()
        //{
        //    var allMeetings = (await _meetingService.GetArchivedByEmployeeId(currentUserId));
        //    var MeetingVm = _mapper.Map<List<MeetingVm>>(allMeetings);
        //    return PartialView("_indexPartial", MeetingVm);
        //}



        /************ Admin Filters *************/

        //public async Task<IActionResult> GetApprovedMeetingsToday()
        //{
        //    List<MeetingVm> allMeetings = new List<MeetingVm>();
        //    if (currentUserRole == Roles.Admin)
        //    {
        //        allMeetings = _mapper.Map<List<MeetingVm>>((await _meetingService.GetAll()).Where(x => x.ApprovalStatus == ApprovalStatus.Approved && x.Date == DateTime.Now.Date).OrderByDescending(x => x.CreatedOn).ThenByDescending(x => x.Time_From));
        //    }
        //    else if (currentUserRole == Roles.Employee)
        //    {
        //        allMeetings = _mapper.Map<List<MeetingVm>>((await _meetingService.GetAllByEmployeeId(currentUserId)).Where(x => x.ApprovalStatus == ApprovalStatus.Approved && x.Date + x.Time_To > DateTime.Now.Date).OrderByDescending(x => x.CreatedOn).ThenByDescending(x => x.Time_From));

        //    }
        //    else if (currentUserRole == Roles.Ceo || currentUserRole == Roles.CeoAssistant)
        //    {
        //        allMeetings = _mapper.Map<List<MeetingVm>>((await _meetingService.GetAllCeo(currentUserId)).Where(x => x.ApprovalStatus == ApprovalStatus.Approved && x.Date + x.Time_To > DateTime.Now.Date).OrderByDescending(x => x.CreatedOn).ThenByDescending(x => x.Time_From));
        //    }

        //    var allMeetings = (await _meetingService.GetAll()).Where(x => x.ApprovalStatus == ApprovalStatus.Approved && x.Date == DateTime.Now.Date).OrderByDescending(x => x.CreatedOn).ThenByDescending(x => x.Time_From);
        //    var MeetingVm = _mapper.Map<List<MeetingVm>>(allMeetings);
        //    return PartialView("_indexPartial", MeetingVm);
        //}

        //public async Task<IActionResult> GetPendingApprovals()
        //{
        //    var allMeetings = (await _meetingService.GetAll()).Where(x=>x.ApprovalStatus == ApprovalStatus.Pending && (x.Date > DateTime.Now.Date || (x.Date == DateTime.Now.Date && x.Time_From >= DateTime.Now.TimeOfDay))).OrderByDescending(x => x.CreatedOn).ThenByDescending(x => x.Time_From);
        //    var MeetingVm = _mapper.Map<List<MeetingVm>>(allMeetings);
        //    return PartialView("_indexPartial", MeetingVm);
        //}


        public async Task<IActionResult> GetScheduledMeetings()
        {
            List<MeetingVm> allMeetings = new List<MeetingVm>();
            if (currentUserRole == Roles.Admin)
            {
                allMeetings = _mapper.Map<List<MeetingVm>>((await _meetingService.GetAll()).Where(x => x.ApprovalStatus == ApprovalStatus.Approved && x.Date + x.Time_To > DateTime.Now).OrderByDescending(x => x.CreatedOn).ThenByDescending(x => x.Time_From));
            }else if(currentUserRole == Roles.Employee)
            {
                allMeetings = _mapper.Map<List<MeetingVm>>((await _meetingService.GetAllByEmployeeId(currentUserId)).Where(x => x.ApprovalStatus == ApprovalStatus.Approved && x.Date + x.Time_To > DateTime.Now).OrderByDescending(x => x.CreatedOn).ThenByDescending(x => x.Time_From));

            }else if (currentUserRole == Roles.Ceo || currentUserRole == Roles.CeoAssistant)
            {
                allMeetings = _mapper.Map<List<MeetingVm>>((await _meetingService.GetAllCeo(currentUserId)).Where(x => x.ApprovalStatus == ApprovalStatus.Approved && x.Date + x.Time_To > DateTime.Now).OrderByDescending(x => x.CreatedOn).ThenByDescending(x => x.Time_From));
            }
            //MeetingVm = _mapper.Map<List<MeetingVm>>(allMeetings);
            return PartialView("_indexPartial", allMeetings);
        }

        public async Task<IActionResult> GetArchived()
        {
            List<MeetingVm> allMeetings = new List<MeetingVm>();
            if (currentUserRole == Roles.Admin)
            {
                allMeetings = _mapper.Map<List<MeetingVm>>((await _meetingService.GetArchived()));
            }
            else if (currentUserRole == Roles.Employee)
            {
                allMeetings = _mapper.Map<List<MeetingVm>>((await _meetingService.GetArchivedByEmployeeId(currentUserId)));

            }
            else if (currentUserRole == Roles.Ceo || currentUserRole == Roles.CeoAssistant)
            {
                allMeetings = _mapper.Map<List<MeetingVm>>((await _meetingService.GetAllCeo(currentUserId)).Where(x => (x.ApprovalStatus != ApprovalStatus.Pending && x.Date + x.Time_To < DateTime.Now) || x.ApprovalStatus==ApprovalStatus.Cancelled).OrderByDescending(x => x.CreatedOn).ThenByDescending(x => x.Time_From));
            }
            return PartialView("_indexPartial", allMeetings);
        }



        public async Task<IActionResult> View(int id)
        {
            _logger.LogDebug("Inside View page");
            Meeting allMeetings = null;
            MeetingVm meetingVm = null;

            try
            {
                allMeetings = await _meetingService.GetById(id);
                meetingVm = _mapper.Map<MeetingVm>(allMeetings);

                meetingVm.MeetingInternalAttendees = new List<EmployeeVm>();
                meetingVm.ExternalAttendeesList = new List<ExternalMeetingAttendeesVm>();

                var internalAttendees = meetingVm.MeetingAttendees.Where(x => x.AttendeeType == AttendeeType.Internal).ToList();

                if (internalAttendees != null && internalAttendees.Count > 0)
                {
                    foreach (var attendee in internalAttendees)
                    {
                        meetingVm.MeetingInternalAttendees.Add(attendee.Employee);
                    }
                }

                var externalAttendees = meetingVm.MeetingAttendees.Where(x => x.AttendeeType == AttendeeType.External).ToList();

                if (externalAttendees != null && externalAttendees.Count > 0)
                {
                    foreach (var attendee in externalAttendees)
                    {
                        ExternalMeetingAttendeesVm extAtt = new ExternalMeetingAttendeesVm();
                        extAtt.NameAr = attendee.ExternalAttendeeNameAr;
                        extAtt.NameEn = attendee.ExternalAttendeeNameEn;
                        extAtt.EmailAddress = attendee.ExternalAttendeeEmailAddress;
                        meetingVm.ExternalAttendeesList.Add(extAtt);
                    }
                }
                if (!(currentUserId == meetingVm.Host.Id) && !(currentUserRole == Roles.Admin) && !meetingVm.IsAttachmentShared)
                {
                    meetingVm.MeetingAttachments = null;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.InnerException.Message);
                throw;
            }
            
            return PartialView("_detailsPartial", meetingVm);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            _logger.LogDebug("On Create Page");
            MeetingVm meetingInitialDetails = new MeetingVm();

            try
            {
                var employee = await _employeeService.GetEmployeeById(currentUserId);
                meetingInitialDetails.EmployeesList = new List<EmployeeVm>();
                meetingInitialDetails.HostList = new List<EmployeeVm>();
                meetingInitialDetails.HostId = employee.Id;
                var employeeVm = _mapper.Map<EmployeeVm>(employee);
                var allEmployeesVm = _mapper.Map<List<EmployeeVm>>(await _employeeService.GetAllEmployees());

                if(currentUserRole == Roles.Admin)
                {
                    meetingInitialDetails.HostList = allEmployeesVm;
                    meetingInitialDetails.EmployeesList = allEmployeesVm;
                }
                else if (currentUserRole == Roles.Ceo || currentUserRole == Roles.CeoAssistant)
                {
                    //meetingInitialDetails.HostList.Add(employeeVm);
                    meetingInitialDetails.HostList.AddRange(allEmployeesVm.Where(x => x.RoleId == Roles.Ceo || x.RoleId == Roles.CeoAssistant).ToList());
                    meetingInitialDetails.EmployeesList = allEmployeesVm;
                }
                else
                {
                    meetingInitialDetails.HostList.Add(employeeVm);
                    meetingInitialDetails.EmployeesList = allEmployeesVm.Where(x => x.Id != currentUserId).ToList();
                }

                meetingInitialDetails.MeetingRoomslist = _mapper.Map<List<MeetingRoomVm>>(await _meetingRoomService.GetAll());
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex.InnerException.Message);
                throw;
            }

            return View(meetingInitialDetails);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MeetingVm meetingVm)
        {
            CancellationToken cts = new CancellationToken();
            Meeting meeting = new Meeting();
            string message = null;
            ToastVm toast = new ToastVm();
            ToastAlertType type = ToastAlertType.info;
            try
            {
                switch (meetingVm.MeetingLocation)
                {

                    case (char)LocationType.Online:
                        meetingVm.MeetingRoomId = null;
                        break;
                }


                meeting = _mapper.Map<Meeting>(meetingVm);
                meeting.MeetingLocationType = (LocationType)meetingVm.MeetingLocation;

                if (! (meeting.MeetingLocationType == LocationType.Online))
                {

                    meeting.MeetingRoom = await _meetingRoomService.GetById(meeting.MeetingRoomId.Value);
                }
                //Meeting Host
                var employee = await _employeeService.GetEmployeeById(meetingVm.HostId);

                var meetingHost = _mapper.Map<EmployeeVm>(employee);

                meeting.HostId = meetingHost.Id;
                List<Invitee> invitees = new List<Invitee>();
                //ExternalAttendees
                if (meetingVm.ExternalAttendeesList != null && meetingVm.ExternalAttendeesList.Count > 0)
                {
                    foreach (var entity in meetingVm.ExternalAttendeesList)
                    {
                        Invitee invitee = new Invitee();
                        if (!string.IsNullOrWhiteSpace(entity.EmailAddress))
                        {
                            MeetingAttendee meetingObj = new MeetingAttendee();
                            meetingObj.AttendeeType = AttendeeType.External;
                            meetingObj.ExternalAttendeeEmailAddress = entity.EmailAddress;
                            meetingObj.ExternalAttendeeNameEn = entity.NameEn != null ? entity.NameEn : "";
                            meetingObj.ExternalAttendeeNameAr = entity.NameAr != null ? entity.NameAr : "";
                            invitee.DisplayName = entity.NameAr;
                            invitee.Email = entity.EmailAddress;
                            meeting.MeetingAttendees.Add(meetingObj);
                            invitees.Add(invitee);
                        }

                    }
                }

                if (meetingVm.MeetingInternalAttendeesId != null && meetingVm.MeetingInternalAttendeesId.Count > 0)
                {
                    foreach (var entity in meetingVm.MeetingInternalAttendeesId)
                    {
                        MeetingAttendee meetingObj = new MeetingAttendee();
                        meetingObj.AttendeeType = AttendeeType.Internal;
                        meetingObj.EmployeeId = entity;
                        meeting.MeetingAttendees.Add(meetingObj);
                    }
                }

                if (meetingVm.RequirementsList != null && meetingVm.RequirementsList.Count > 0)
                {
                    foreach (var req in meetingVm.RequirementsList)
                    {
                        if (!string.IsNullOrWhiteSpace(req))
                        {
                            MeetingItem item = new MeetingItem();
                            item.ItemName = req;
                            item.MeetingId = meeting.Id;
                            meeting.MeetingRequirements.Add(item);
                        }
                    }
                }

                if (meetingVm.Files != null && meetingVm.Files.Count > 0)
                {
                    foreach (var att in meetingVm.Files)
                    {
                        MeetingAttachment attachment = new MeetingAttachment();
                        var fileNameSanitized = _htmlSanitizer.Sanitize(att.FileName);
                        using var fileStream = att.OpenReadStream();
                        attachment.FilePath = await _fileService.SaveFile(fileNameSanitized, fileStream);
                        attachment.FileName = att.FileName;
                        attachment.MeetingId = meeting.Id;
                        meeting.MeetingAttachments.Add(attachment);
                    }
                }

                meeting.ApprovalStatus = ApprovalStatus.Approved;
                meeting.IsWebex = meetingVm.IsWebex;
                meeting.IsCeo = meetingVm.IsCeo;
                MeetingDetails response = new MeetingDetails();
                MeetingDetails details = new MeetingDetails();
                var result = 0;
                //Automatically assign CEO to ceo meeting or check if option is picked
                if (meetingVm.IsCeo ||  currentUserRole == Roles.Ceo)
                {
                    meeting.ZoomAccount = ZoomUserType.Ceo;
                }
                else
                {
					meeting.ZoomAccount = ZoomUserType.Main;
				}



                if (meeting.MeetingLocationType == LocationType.Both || meeting.MeetingLocationType == LocationType.Online)
                {
                    details.Topic = meeting.Subject;
                    details.Agenda = meeting.MeetingAgenda;
                    details.Location = (meeting.MeetingLocationType == LocationType.Both) ? "Virtual | On Site" : "Virtual";
                    details.StartDateTime = meeting.Date + meeting.Time_From;
                    details.EndDateTime = meeting.Date + meeting.Time_To;
                    details.RecordedMeeting = meeting.IsRecorded;
                    details.ZoomAccount = meeting.ZoomAccount;
                    details.DurationInMinutes = (meeting.Time_To - meeting.Time_From).TotalMinutes;


                    if (meetingVm.IsWebex)
                        response = await _webexService.CreateMeeting(details);
                    else
                        response = await _zoomService.CreateMeeting(details);

                    if (!string.IsNullOrEmpty(response.MeetingId) && !string.IsNullOrEmpty(response.JoiningUrl))
                    {
                        meeting.MeetingId = response.MeetingId;
                        meeting.MeetingPassword = response.Password;
                        meeting.MeetingLink = response.JoiningUrl;
                        meeting.WebexMeetingNumber = response.WebexMeetingNumber;

                        result = await _meetingService.Create(meeting, cts);
                        if (!(result > 0))
                        {
                            await _zoomService.CancelMeeting(meeting.MeetingId);
                        }
                    }
                    else
                    {

                        _logger.LogError("Problem Connecting With Zoom   ====      Error Details:    " + response.ToString() + "=======  Meeting Details:    " + details);
                        toast.Message = _sharedResource["CreateError"];
                        toast.Type = ToastAlertType.error;
                        return Json(new { redirectToUrl = Url.Action("Index", "Meeting") }, toast);
                    }
                }
                else if (meeting.MeetingLocationType == LocationType.FaceToFace)
                {
                    details.Location = "This meeting will on premise in : " + meeting.MeetingRoom.NameAr + " - " + meeting.MeetingRoom.NameEn;
                    result = await _meetingService.Create(meeting, cts);
                }
                else
                {
                    toast.Message = _sharedResource["CreateError"];
                    toast.Type = ToastAlertType.error;
                    return Json(new { redirectToUrl = Url.Action("Index", "Meeting", meetingVm), type = type.ToString(), message = message });
                }

                if(result > 0)
                {
                    var emailDetails = await _meetingService.GetById(meeting.Id);
                    //var calendar = await _calService.CreateICS(details);
                    var mailRequest = await _emailBuilderService.BuildMeetingInvitationEmail(meeting);
                    //mailRequest.Calendar = calendar;
                    if (meeting.MeetingLocationType == LocationType.Both || meeting.MeetingLocationType == LocationType.FaceToFace)
                    {
                        meeting.MeetingRoom = await _meetingRoomService.GetById(meeting.MeetingRoomId.Value);
                    }
                    await _emailService.SendEmail(mailRequest);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.InnerException.Message);
                toast.Message = _sharedResource["CreateError"];
                toast.Type = ToastAlertType.error;
               
                return Json(new { redirectToUrl = Url.Action("Index", "Meeting",meetingVm), type = type.ToString(), message = message } );
            }

            toast.Message = _sharedResource["CreateSuccess"];
            toast.Type = ToastAlertType.success;
            TempData["message"] = toast.Message;
            TempData["type"] = toast.Type.ToString();
            return Json(new { redirectToUrl = Url.Action("Index", "Meeting",toast) , type = type.ToString(), message = message });
        }


        public async Task<IActionResult> Edit(int id)
        {

            MeetingVm meetingVm = new MeetingVm();

            try {
                var meeting = await _meetingService.GetById(id);

                meetingVm = _mapper.Map<MeetingVm>(meeting);

                meetingVm.EmployeesList = _mapper.Map<List<EmployeeVm>>(await _employeeService.GetAllEmployees());
                meetingVm.MeetingRoomslist = _mapper.Map<List<MeetingRoomVm>>(await _meetingRoomService.GetAll());

                meetingVm.MeetingLocation = (char)meeting.MeetingLocationType;

                ExternalMeetingAttendeesVm ext = new ExternalMeetingAttendeesVm();

                //Attendees
                meetingVm.ExternalAttendeesList = new List<ExternalMeetingAttendeesVm>();
                meetingVm.MeetingInternalAttendeesId = new List<int>();
                foreach (var attendee in meeting.MeetingAttendees)
                {
                    if (attendee.AttendeeType == AttendeeType.Internal)
                    {
                        meetingVm.MeetingInternalAttendeesId.Add(attendee.EmployeeId.Value);
                    }
                    else if (attendee.AttendeeType == AttendeeType.External)
                    {
                        ExternalMeetingAttendeesVm extAttendee = new ExternalMeetingAttendeesVm();
                        extAttendee.Id = attendee.Id;
                        extAttendee.EmailAddress = attendee.ExternalAttendeeEmailAddress;
                        extAttendee.NameEn = !string.IsNullOrEmpty(attendee.ExternalAttendeeNameEn) ? attendee.ExternalAttendeeNameEn : "";
                        extAttendee.NameAr = !string.IsNullOrEmpty(attendee.ExternalAttendeeNameAr) ? attendee.ExternalAttendeeNameAr : "";
                        meetingVm.ExternalAttendeesList.Add(extAttendee);
                    }
                }
                if (meetingVm.MeetingRequirements.Count <= 0)
                {
                    meetingVm.MeetingRequirements = new List<MeetingItemVm>();
                    meetingVm.MeetingRequirements.Add(new MeetingItemVm());
                }
                foreach (var attachment in meetingVm.MeetingAttachments)
                {
                    string file;
                    new FileExtensionContentTypeProvider().TryGetContentType(attachment.FileName, out file);
                    attachment.FileType = file;
                    var FileStream = await _fileService.GetFile(attachment.FilePath);
                    attachment.FileSize = FileStream.Length;
                    attachment.Path = attachment.FilePath;
                }
            }
            catch (Exception Ex)
            {

            }

            return View(meetingVm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(MeetingVm meetingVm, List<IFormFile> MeetingFiles, CancellationToken cancellation)
        {
            string message = null;
            int result = 0;
            ToastAlertType type;
            //Basic Details
            try
            {
                switch (meetingVm.MeetingLocation)
                {
                    case (char)LocationType.FaceToFace:
                        meetingVm.MeetingLink = null;
                        break;
                    case (char)LocationType.Online:
                        meetingVm.MeetingRoom = null;
                        break;
                }
                var meeting = await _meetingService.GetById(meetingVm.Id); 
                var meetingToUpdate = _mapper.Map<Meeting>(meetingVm);
                meetingToUpdate.MeetingAttachments = meeting.MeetingAttachments;

                meetingToUpdate.MeetingLocationType = (LocationType)meetingVm.MeetingLocation;

                var externalAttendees = meeting.MeetingAttendees.Where(x => x.IsActive == true && x.AttendeeType == AttendeeType.External).ToList();
                var internalAttendees = meeting.MeetingAttendees.Where(x => x.IsActive == true && x.AttendeeType == AttendeeType.Internal).ToList();

                //Host Attendee
                if (meetingVm.HostId != meeting.HostId)
                {

                    meetingToUpdate.HostId = meetingVm.HostId;
                }

                if (meetingVm.ExternalAttendeesList.Count > 0)
                {
                    var externalToAdd = meetingVm.ExternalAttendeesList.Where(p => externalAttendees.All(p2 => p2.ExternalAttendeeEmailAddress != p.EmailAddress && p2.Id != p.Id)).ToList();
                    var externalToRemove = externalAttendees.Where(p => meetingVm.ExternalAttendeesList.All(p2 => p2.EmailAddress != p.ExternalAttendeeEmailAddress)).ToList();
                    var externalToUpdate = externalAttendees.Where(p => meetingVm.ExternalAttendeesList.Any(p2 => p2.Id == p.Id)).ToList();
                    foreach (var attendee in externalToAdd)
                    {
                        MeetingAttendee meetingObj = new MeetingAttendee();
                        meetingObj.AttendeeType = AttendeeType.External;
                        meetingObj.ExternalAttendeeEmailAddress = attendee.EmailAddress;
                        meetingObj.ExternalAttendeeNameEn = attendee.NameEn != null ? attendee.NameEn : "";
                        meetingObj.ExternalAttendeeNameAr = attendee.NameAr != null ? attendee.NameAr : "";
                        meetingToUpdate.MeetingAttendees.Add(meetingObj);
                    }
                    foreach (var attendeeToRemove in externalToRemove)
                    {
                        meeting.MeetingAttendees.Where(x => x.Id == attendeeToRemove.Id);
                        MeetingAttendee meetingObj = new MeetingAttendee();
                        attendeeToRemove.IsActive = false;
                        meetingToUpdate.MeetingAttendees.Add(attendeeToRemove);
                    }
                    foreach (var attendeeToUpdate in externalToUpdate)
                    {
                        var newMeetingDetails = meetingVm.ExternalAttendeesList.Where(x => x.Id == attendeeToUpdate.Id).FirstOrDefault();
                        attendeeToUpdate.ExternalAttendeeEmailAddress = newMeetingDetails.EmailAddress;
                        attendeeToUpdate.ExternalAttendeeNameAr = newMeetingDetails.NameAr != null ? newMeetingDetails.NameAr : "";
                        attendeeToUpdate.ExternalAttendeeNameEn = newMeetingDetails.NameEn != null ? newMeetingDetails.NameEn : "";
                        meetingToUpdate.MeetingAttendees.Add(attendeeToUpdate);
                    }
                }
                else //if none, Disable all
                {
                    var externalToRemove = externalAttendees.Where(p => meetingVm.ExternalAttendeesList.All(p2 => p2.EmailAddress != p.ExternalAttendeeEmailAddress)).ToList();
                    if (externalToRemove.Count > 0)
                    {
                        foreach (var external in meeting.MeetingAttendees)
                        {
                            if (external.AttendeeType == AttendeeType.External)
                            {
                                external.IsActive = false;
                                meetingToUpdate.MeetingAttendees.Add(external);
                            }
                        }
                    }
                }

                //InternalAttendees
                if (meetingVm.MeetingInternalAttendeesId.Count > 0)
                {
                    var newinternalAtt = meetingVm.MeetingInternalAttendeesId.Where(p => internalAttendees.All(p2 => p2.EmployeeId != p)).ToList();//new list items not in old list items 
                    var internalToRemove = internalAttendees.Where(p => meetingVm.MeetingInternalAttendeesId.All(p2 => p2 != p.EmployeeId)).ToList();
                    var internalToUpdate = internalAttendees.Where(p => meetingVm.MeetingInternalAttendeesId.Any(p2 => p2 == p.EmployeeId)).ToList();
                    foreach (var deactivateInternal in internalToRemove)
                    {
                        deactivateInternal.IsActive = false;
                        meetingToUpdate.MeetingAttendees.Add(deactivateInternal);
                    }
                    foreach (var addNewAtt in newinternalAtt)
                    {
                        MeetingAttendee meetingObj = new MeetingAttendee();
                        meetingObj.AttendeeType = AttendeeType.Internal;
                        meetingObj.EmployeeId = addNewAtt;
                        meetingToUpdate.MeetingAttendees.Add(meetingObj);
                    }
                    foreach (var updateAtt in internalToUpdate)
                    {
                        updateAtt.ExternalAttendeeNameAr = meetingVm.ExternalAttendeesList.Where(x => x.Id == updateAtt.Id).Select(x => x.NameAr).FirstOrDefault();
                        updateAtt.ExternalAttendeeNameEn = meetingVm.ExternalAttendeesList.Where(x => x.Id == updateAtt.Id).Select(x => x.NameEn).FirstOrDefault();
                        updateAtt.ExternalAttendeeEmailAddress = meetingVm.ExternalAttendeesList.Where(x => x.Id == updateAtt.Id).Select(x => x.EmailAddress).FirstOrDefault();
                        meetingToUpdate.MeetingAttendees.Add(updateAtt);
                    }
                }
                else //if none, Disable all
                {
                    if (internalAttendees.Count > 0)
                    {
                        foreach (var intAtt in meeting.MeetingAttendees)
                        {
                            if (intAtt.AttendeeType == AttendeeType.External)
                            {
                                intAtt.IsActive = false;
                                meetingToUpdate.MeetingAttendees.Add(intAtt);
                            }
                        }
                    }
                }

                if (meetingVm.MeetingRequirements.Count > 0)
                {
                    meetingToUpdate.MeetingRequirements = new List<MeetingItem>();
                    var requirementsToRemove = meeting.MeetingRequirements.Where(p => meetingVm.MeetingRequirements.Any(p2 => p.Id == p2.Id && string.IsNullOrEmpty(p2.ItemName))).ToList();
                    var requirementsToAdd = meetingVm.MeetingRequirements.Where(p => p.Id == 0).ToList();
                    var requirementsToUpdate = meeting.MeetingRequirements.Where(p => meetingVm.MeetingRequirements.Any(p2 => p2.Id == p.Id && p2.ItemName != null)).ToList();
                    foreach (var req in requirementsToRemove)
                    {

                        req.IsActive = false;
                        meetingToUpdate.MeetingRequirements.Add(req);
                    }
                    foreach (var req in requirementsToAdd)
                    {
                        MeetingItem item = new MeetingItem();
                        item.ItemName = req.ItemName;
                        meetingToUpdate.MeetingRequirements.Add(item);
                    }
                    foreach (var req in requirementsToUpdate)
                    {
                        req.ItemName = meetingVm.MeetingRequirements.Where(x => x.Id == req.Id).Select(x => x.ItemName).FirstOrDefault();
                        meetingToUpdate.MeetingRequirements.Add(req);
                    }
                }

                if (meetingVm.Files != null && meetingVm.Files.Count > 0)
                {
                    foreach (var att in meetingVm.Files)
                    {
                        MeetingAttachment attachment = new MeetingAttachment();
                        var fileNameSanitized = _htmlSanitizer.Sanitize(att.FileName);
                        using var fileStream = att.OpenReadStream();
                        attachment.FilePath = await _fileService.SaveFile(fileNameSanitized, fileStream);
                        attachment.FileName = att.FileName;
                        attachment.MeetingId = meeting.Id;
                        meetingToUpdate.MeetingAttachments.Add(attachment);
                    }
                }

                if (meetingVm.DeletedImageUrls != null && meetingVm.DeletedImageUrls.Count > 0)
                {
                    foreach (var file_url in meetingVm.DeletedImageUrls)
                    {
                        meetingToUpdate.MeetingAttachments.Where(x => x.FilePath == file_url).FirstOrDefault().IsActive = false ;
                    }
                }

                result = await _meetingService.Update(meetingToUpdate);


                if (result > 0)
                {
                    if (meetingVm.DeletedImageUrls != null && meetingVm.DeletedImageUrls.Count > 0)
                    {
                        foreach (var file_url in meetingVm.DeletedImageUrls)
                        {
                            await _fileService.DeleteFile(file_url);
                        }
                    }
                    message = _sharedResource["EditSuccess"];
                    type = ToastAlertType.success;
                }
                else
                {
                    message = _sharedResource["EditError"];
                    type = ToastAlertType.error;
                }
            }
            catch (Exception ex)
            {
                message = _sharedResource["EditError"];
                type = ToastAlertType.error;
                _logger.LogError(ex.InnerException.Message);
                return View(message != null ? new ToastVm { Type = type, Message = message } : null);
            }


            return View("Index", message != null ? new ToastVm { Type = ToastAlertType.success, Message = message } : null);
        }

        //public async Task<IActionResult> Cancel(int id)
        //{
        //    var message="";
        //    var type = ToastAlertType.error;
        //    int response = 0 ;
        //    CancellationToken cancellation = new CancellationToken();
        //    try
        //    {
        //        var meeting = (await _meetingService.GetById(id));
        //        var zoomAccount = ZoomAccount.Main;
        //        if(meeting.Host.RoleId.ToString() == Roles.Ceo)
        //        {
        //            zoomAccount = ZoomAccount.Ceo;
        //        }
        //        var previousStatus = meeting.ApprovalStatus;
        //        var result = await _meetingService.Cancel(id, cancellation);
        //        if (result > 0)
        //        {
        //            if ((!string.IsNullOrWhiteSpace(meeting.ZoomMeetingId)))
        //            {
        //                response = await _zoomService.CancelMeeting(meeting.ZoomMeetingId, zoomAccount);
        //                if (response.ToString() == StatusCodes.Status204NoContent.ToString() || response.ToString() == StatusCodes.Status200OK.ToString())
        //                {
        //                    message = _sharedResource["CancelSuccess"];
        //                    type = ToastAlertType.success;
        //                }
        //                else
        //                {
        //                    message = _sharedResource["CancelError"];
        //                    type = ToastAlertType.error;
        //                }
        //            }
        //            if (previousStatus == ApprovalStatus.Approved || previousStatus == ApprovalStatus.Pending)
        //            {
        //                var mailRequest = await _emailBuilderService.BuildMeetingCancellationEmail(meeting,previousStatus);
        //                await _emailService.SendEmail(mailRequest);
        //            }

        //            message = _sharedResource["CancelSuccess"];
        //            type = ToastAlertType.success;
        //        }
        //        else
        //        {
        //            message = _sharedResource["CancelError"];
        //            type = ToastAlertType.error;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        message = _sharedResource["CancelError"];
        //        type = ToastAlertType.error;
        //        _logger.LogError(ex.InnerException.Message);
        //    }

        //    return Json( message != null ? new ToastVm { Type = type, Message = message } : null);
        //}


        public async Task<IActionResult> ChangeApprovalStatus(int id, string reason, ApprovalStatus status, CancellationToken cts = default )
        {
            cts = new CancellationToken();
            var meeting = await _meetingService.GetById(id);
            var zoomAccount = meeting.ZoomAccount;

            var previousStatus = meeting.ApprovalStatus;
            var result = 0;
            var meetingRoom = !meeting.MeetingRoomId.HasValue ? 0 : meeting.MeetingRoomId.Value;
            string message = null;
            
            MeetingValidity meetingValidity = new MeetingValidity()
            {
                Date = meeting.Date,
                From = meeting.Time_From,
                To = meeting.Time_To,
                HostId = meeting.HostId,
                RoomId = meetingRoom,
                MeetingId = id,
                LocationType = Convert.ToChar(meeting.MeetingLocationType) 
            };
            ToastAlertType type = ToastAlertType.info;
            try
            {
                if (meeting.HostId == currentUserId || _userProvider.CurrentUser.UserRole == Roles.Admin || (meeting.ZoomAccount == ZoomUserType.Ceo && (currentUserRole == Roles.CeoAssistant || currentUserRole == Roles.Ceo)))
                {
                    meeting.ApprovalStatus = status;

                    meeting.Reason = reason;

					MeetingAvailability isTimeSlotAvailable = await _meetingValidation.CheckIfTimeAvailableForHost(meetingValidity, (meeting.ZoomAccount));
                    
                    if(status == ApprovalStatus.Approved)
                    {
                        if (isTimeSlotAvailable == MeetingAvailability.Available)
                        {
                            if (meeting.MeetingLocationType == LocationType.Both || meeting.MeetingLocationType == LocationType.Online)
                            {
                                MeetingDetails details = new MeetingDetails();
                                details.Topic = meeting.Subject;
                                details.StartDateTime = meeting.Date + meeting.Time_From;
                                details.RecordedMeeting = meeting.IsRecorded;
                                details.DurationInMinutes = (meeting.Time_To - meeting.Time_From).TotalMinutes;
                                var response = await _zoomService.CreateMeeting(details);
                                meeting.MeetingId = response.MeetingId;
                                meeting.MeetingPassword = response.Password;
                                meeting.MeetingLink = response.JoiningUrl;
                            }
                            result = await _meetingService.Update(meeting);
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
                    else if (status == ApprovalStatus.Rejected)
                    {
                        result = await _meetingService.Update(meeting);
                    }
                    else if (status == ApprovalStatus.Cancelled)
                    {
                        result = await _meetingService.Cancel(id, cts);
                        
                    }

                    if (result > 0 )
                    {            
                        if(status == ApprovalStatus.Approved)
                        {
                            var mailRequest = await _emailBuilderService.BuildMeetingInvitationEmail(meeting);
                            await _emailService.SendEmail(mailRequest);
                            message = _meetingControllerResource["StatusUpdated"];
                            type = ToastAlertType.success;
                        }
                        else if (status == ApprovalStatus.Rejected)
                        {
                            var mailRequest = await _emailBuilderService.BuildMeetingRejectionEmail(meeting);
                            await _emailService.SendEmail(mailRequest);
                            message = _meetingControllerResource["StatusUpdated"];
                            type = ToastAlertType.success;
                        }
                        else if(status == ApprovalStatus.Cancelled)
                        {
                            message = _meetingControllerResource["StatusUpdated"];
                            type = ToastAlertType.success;
                            if ((!string.IsNullOrWhiteSpace(meeting.MeetingId)))
                            {
                                int response;
                                if (meeting.IsWebex)
                                {
                                     response = await _webexService.CancelMeeting(meeting.MeetingId, (WebexUserType) meeting.ZoomAccount);
                                }
                                else
                                     response = await _zoomService.CancelMeeting(meeting.MeetingId, meeting.ZoomAccount);
                                
                                if(response.ToString() == StatusCodes.Status204NoContent.ToString() || response.ToString() == StatusCodes.Status200OK.ToString())
                                {
                                    message = _meetingControllerResource["StatusUpdated"];
                                    type = ToastAlertType.success;
                                }
                                else
                                {
                                    message = _sharedResource["CancelError"];
                                    type = ToastAlertType.error;
                                }
                            }
                            var mailRequest = await _emailBuilderService.BuildMeetingCancellationEmail(meeting, previousStatus);
                            await _emailService.SendEmail(mailRequest);
                        }
                    }
                    else
                    {
                        message = _sharedResource["EditError"];
                        type = ToastAlertType.error;
                    }
                }
                else
                {
                    message = _meetingControllerResource["NoPermissionToUpdate"];
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


        [HttpPost]
        public async Task<JsonResult> CheckAvailability(MeetingValidity meeting)
        
        {
            MeetingAvailability meetingCanBeReserved;

            var canReserve = false;

            string message = "";

            var host = await _employeeService.GetEmployeeById(meeting.HostId);

            if (host.RoleId.ToString() == Roles.Ceo || meeting.IsCeo)
            {
                meetingCanBeReserved = await _meetingValidation.CheckIfTimeAvailableForHost(meeting,ZoomUserType.Ceo);
            }
            else
                meetingCanBeReserved = await _meetingValidation.CheckIfTimeAvailableForHost(meeting);

            MeetingAvailability output = (MeetingAvailability)meetingCanBeReserved;
            
            if (output != MeetingAvailability.Available) 
                message = _sharedResource[output.ToString()];
            else 
                canReserve = true;

            var data = new { meetingCanBeReserved = canReserve, message = message };
            return Json(data);
        }


        public async Task<IActionResult> AddRequirementsPartial()
        {
            MeetingVm meeting = new MeetingVm();
            return PartialView("_RequirementsPartial",meeting);
        }


        public async Task<ActionResult> AddAttendeesPartial()
        {
            return PartialView("_ExternalAttendeesPartial");
        }

    }
}
