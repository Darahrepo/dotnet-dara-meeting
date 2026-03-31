using EmployeeScheduler.Infrastructure.Interfaces;
using MeetingScheduler.Domain.Common.Interfaces;
using MeetingScheduler.Domain.Common.Models;
using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Domain.Enums;
using MeetingScheduler.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Infrastructure.Common.Services
{
    public class EmailBuilderService : IEmailBuilderService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileServices _fileServices;
        private readonly IMeetingRoomService _meetingRoomService;
        private readonly IEmployeeService _employeeService;
        private readonly IConfiguration _configuration;
        public EmailBuilderService(IWebHostEnvironment webHostEnvironment, IEmployeeService employeeService, IMeetingRoomService meetingRoomService, IConfiguration configuration, IFileServices fileServices)
        {
            _webHostEnvironment = webHostEnvironment;
            _fileServices = fileServices;
            _configuration = configuration;
            _meetingRoomService = meetingRoomService;
            _employeeService = employeeService;
        }

        private async Task<StringBuilder> GetEmailTemplateBody(string templatePath)
        {
            var bodyPath = Path.Combine(_webHostEnvironment.WebRootPath, "Resources/EmailTemplates", templatePath);

            var bodyString = new StringBuilder(await File.ReadAllTextAsync(bodyPath));
            return bodyString;
        }

        //Meeting Emails


        public async Task<MailRequest> BuildMeetingCreationEmail(Meeting meeting)
        {
            var bodyString = await GetEmailTemplateBody("PendingMeetingApproval.html");

            var mailRequest = new MailRequest
            {
                Subject = "Meeting Approval Pending  اجتماع قيد الانتظار"
            };
            var systemAdmins = await _employeeService.GetSystemAdmins();
            if (systemAdmins != null)
            {
                foreach (var admin in systemAdmins)
                {
                    mailRequest.ToEmailAddresses.Add(admin.EmailAddress);
                }

            }

            //replace words in template
            var replaceTextPairs = new Dictionary<string, string>();
            replaceTextPairs.Add("[[Subject]]", meeting.Subject);

            replaceTextPairs.Add("[[EmployeeName]]", meeting.Host.DisplayName);

            if (!string.IsNullOrEmpty(meeting.MeetingId))
            {
                replaceTextPairs.Add("[[MeetingId]]", meeting.IsWebex ? meeting.WebexMeetingNumber : meeting.MeetingId);
                replaceTextPairs.Add("[[MeetingPassword]]", meeting.MeetingPassword);
            }
            else
            {
                replaceTextPairs.Add("[[MeetingId]]", "Not Specified غير محدد");
                replaceTextPairs.Add("[[MeetingPassword]]", "Not Specified غير محدد");
            }

          


            replaceTextPairs.Add("[[DayInWords]]", meeting.Date.ToString("dddd, dd MMMM yyyy", CultureInfo.InvariantCulture));
            DateTime time_from = DateTime.Today.Add(meeting.Time_From);
            replaceTextPairs.Add("[[Time_From]]", time_from.ToString("hh:mm tt"));
            DateTime time_to = DateTime.Today.Add(meeting.Time_To);
            replaceTextPairs.Add("[[Time_To]]", time_to.ToString("hh:mm tt"));
            double duration = (meeting.Time_To - meeting.Time_From).TotalMinutes;
            replaceTextPairs.Add("[[DurationInMinutes]]", duration.ToString());

            replaceTextPairs.Add("[[DayInWords_AR]]", meeting.Date.ToString("dddd, dd MMMM yyyy", CultureInfo.GetCultureInfo("ar")));
            DateTime time_from_AR = DateTime.Today.Add(meeting.Time_From);
            replaceTextPairs.Add("[[Time_From_AR]]", time_from.ToString("hh:mm tt", CultureInfo.GetCultureInfo("ar")));
            DateTime time_to_AR = DateTime.Today.Add(meeting.Time_To);
            replaceTextPairs.Add("[[Time_To_AR]]", time_to.ToString("hh:mm tt", CultureInfo.GetCultureInfo("ar")));
            DateTime Duration_AR = DateTime.Today.Add(meeting.Time_To - meeting.Time_From);


            if (bodyString.Length > 0)
            {
                mailRequest.HtmlBody = FormatMessage(bodyString, replaceTextPairs);
            }

            return mailRequest;
        }


        public async Task<MailRequest> BuildMeetingInvitationEmail(Meeting meeting)
        {
            //var systemAdmins = await _employeeService.GetSystemAdmins();
            var bodyString = await GetEmailTemplateBody("MeetingInvitation.html");

            var mailRequest = new MailRequest
            {
                Subject = "Darah Meeting Invitation دعوة اجتماع دارة الملك عبدالعزيز"
            };

            mailRequest.ToEmailAddresses.Add(meeting.Host.EmailAddress);
            foreach (var attendee in meeting.MeetingAttendees)
            {
                if (attendee.AttendeeType == Domain.Enums.AttendeeType.External)
                {
                    mailRequest.ToEmailAddresses.Add(attendee.ExternalAttendeeEmailAddress);
                }
                else
                {
                    mailRequest.ToEmailAddresses.Add(attendee.Employee.EmailAddress);
                }

            }

            //if (systemAdmins != null)
            //{
            //    foreach (var admin in systemAdmins)
            //    {
            //        mailRequest.BccEmailAddresses.Add(admin.EmailAddress);
            //    }

            //}
            if (meeting.IsAttachmentShared)
            {
                foreach(var attachment in meeting.MeetingAttachments)
                {
                    var fileStream = await _fileServices.GetFile(attachment.FilePath);
                    MailAttachment file = new MailAttachment(fileStream, attachment.FileName);
                    mailRequest.Attachments.Add(file);
                }
            }
            //replace words in template
            var replaceTextPairs = new Dictionary<string, string>();
            replaceTextPairs.Add("[[Subject]]", meeting.Subject );

            replaceTextPairs.Add("[[DayInWords]]", meeting.Date.ToString("dddd, dd MMMM yyyy", CultureInfo.InvariantCulture));
            DateTime time_from = DateTime.Today.Add(meeting.Time_From);
            replaceTextPairs.Add("[[Time_From]]", time_from.ToString("hh:mm tt"));
            DateTime time_to = DateTime.Today.Add(meeting.Time_To);
            replaceTextPairs.Add("[[Time_To]]", time_to.ToString("hh:mm tt"));

            if (!string.IsNullOrEmpty(meeting.MeetingId))
            {

                replaceTextPairs.Add("[[MeetingId]]", meeting.IsWebex ? meeting.WebexMeetingNumber : meeting.MeetingId);
                replaceTextPairs.Add("[[MeetingPassword]]", meeting.MeetingPassword);

            }
            else
            {
                replaceTextPairs.Add("[[MeetingId]]", "Not Specified غير محدد");
                replaceTextPairs.Add("[[MeetingPassword]]", "Not Specified غير محدد");
            }



            replaceTextPairs.Add("[[DayInWords_AR]]", meeting.Date.ToString("dddd, dd MMMM yyyy", CultureInfo.GetCultureInfo("ar")));
            DateTime time_from_AR = DateTime.Today.Add(meeting.Time_From);
            replaceTextPairs.Add("[[Time_From_AR]]", time_from.ToString("hh:mm tt", CultureInfo.GetCultureInfo("ar")));
            DateTime time_to_AR = DateTime.Today.Add(meeting.Time_To);
            replaceTextPairs.Add("[[Time_To_AR]]", time_to.ToString("hh:mm tt", CultureInfo.GetCultureInfo("ar")));

            if (meeting.MeetingRoomId != null)
            {
                if (meeting.MeetingRoom == null)
                {
                    meeting.MeetingRoom = await _meetingRoomService.GetById(meeting.MeetingRoomId.Value);
                }
                var OnPremise = $"<p style='margin: 0; font-size: 16px; line-height: 1.2; word-break: break-word; mso-line-height-alt: 19px; margin-top: 0; margin-bottom: 0;'><span style='font-size: 16px;font-family: 'Cairo', Georgia, Times, 'Times New Roman', serif;'><strong>Location  الموقع&nbsp;</strong>&nbsp;</span></p><p style = 'margin: 0; font-size: 14px; line-height: 1.2; word-break: break-word; mso-line-height-alt: 17px; margin-top: 0; margin-bottom: 0;font-family: 'Cairo', Georgia, Times, 'Times New Roman', serif;' >{meeting.MeetingRoom.NameEn} - {meeting.MeetingRoom.NameAr}</p>";
                replaceTextPairs.Add("[[LocationDetails-OnPremise]]", OnPremise);
            }
            else
            {
                replaceTextPairs.Add("[[LocationDetails-OnPremise]]", "");
            }
            if (meeting.MeetingLink != null)
            {
                var meetingType = meeting.IsWebex ? "Webex" : "Zoom";

                var link = $"<div style='line-height: 1.5; font-size: 15px; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: start; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 255); text-decoration-thickness: initial; text-decoration-style: initial; text-decoration-color: initial; color: rgb(108, 108, 130);font-family: 'Cairo', Georgia, Times, 'Times New Roman', serif; padding: 5px;'><div class='txtTinyMce-wrapper' style='line-height: 1.5; font-family: Lato, Tahoma, Verdana, Segoe, sans-serif; font-size: 12px; color: rgb(108, 108, 130);'><p style='line-height: 1.5; margin: 0px; word-break: break-word; text-align: center; font-family: Lato, Tahoma, Verdana, Segoe, sans-serif; font-size: 16px;'><span style='line-height: inherit; font-size: 16px;'><span style='line-height: inherit;'>To go to the meeting, just click the button below <br/> للذهاب الى الإجتماع، الرجاء الضغط على الرابط التالي</span></span></p></div></div>" +
                    $"<div align='center' class='button-container' style='line-height: inherit; color: rgb(33, 33, 33); font-family: 'Cairo', Georgia, Times, 'Times New Roman', serif; font-size: 15px; font-style: normal; font-variant-ligatures: normal; font-variant-caps: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-indent: 0px; text-transform: none; white-space: normal; widows: 2; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 255); text-decoration-thickness: initial; text-decoration-style: initial; text-decoration-color: initial; padding: 10px 10px 10px 20px;'><a href='{ meeting.MeetingLink}' rel='noopener noreferrer' style='line-height: inherit; text-decoration: none; display: inline-block; color: rgb(255, 255, 255); background-color: rgb(168, 156, 89); width: auto; border-width: 3px; border-style: solid; padding-top: 5px; padding-bottom: 5px; font-family: Lato, Tahoma, Verdana, Segoe, sans-serif; text-align: center; word-break: keep-all;' target='_blank'><span style='line-height: inherit; padding-left: 50px; padding-right: 50px; font-size: 18px; display: inline-block;'><span style='line-height: 2; font-size: 16px; word-break: break-word; font-family: Lato, Tahoma, Verdana, Segoe, sans-serif;'><span style='line-height: 36px; font-size: 18px;'><strong style='line-height: inherit;'>OPEN IN {meetingType} <br/>  {meetingType} الذهاب الى</strong></span></span></span></a></div>";
                replaceTextPairs.Add("[[LocationDetails-Online]]", link);
            }
            else
            {
                replaceTextPairs.Add("[[LocationDetails-Online]]", "");
            }
            var guestCount = meeting.MeetingAttendees.Count + 1;
            replaceTextPairs.Add("[[GuestCount]]", guestCount.ToString());

            //replaceTextPairs.Add("[[HostName]]", meeting.Host.DisplayName);
            //replaceTextPairs.Add("[[HostEmail]]", meeting.Host.EmailAddress);

            var guestsData = "";
            guestsData+= ReplaceAttendeeData(meeting.Host.DisplayName, meeting.Host.EmailAddress,true);

            foreach (var attendee in meeting.MeetingAttendees)
            {
                if( attendee != null && attendee.AttendeeType == Domain.Enums.AttendeeType.External)
                {
                    guestsData += ReplaceAttendeeData(attendee.ExternalAttendeeNameEn, attendee.ExternalAttendeeEmailAddress);
                }
                else if(attendee != null && attendee.AttendeeType == Domain.Enums.AttendeeType.Internal)
                {
                    guestsData += ReplaceAttendeeData(attendee.Employee.DisplayName, attendee.Employee.EmailAddress);
                }

            }



            replaceTextPairs.Add("[[Guests]]", guestsData);

            replaceTextPairs.Add("[[Name]]", meeting.Subject );
            string requirements = "";




            if(meeting.MeetingRequirements != null && meeting.MeetingRequirements.Count > 0)
            {
                var count = 1;
                foreach (var req in meeting.MeetingRequirements)
                {
                    if (!string.IsNullOrWhiteSpace(req.ItemName))
                    {
                        requirements += $"<tr><td style = 'min-width: 5px; border: none; user-select: text;' ><p style='margin-bottom: 10px !important; font-size: 16px; font-family: 'Cairo'; color: rgb(59, 56, 56);'><span style='font-size: 14px;'>{count}</span></p>" +
                   $"</td> <td style='min-width: 5px;border: none; user-select: text;'>" +
                   $"<p style='margin-bottom: 10px !important; font-size: 16px; font-family: 'Cairo'; color: rgb(59, 56, 56);'><span style='font-size: 14px;'> {req.ItemName} </span></p>" +
                    "</td></tr>";
                    }
                }
            }
            else
            {
                requirements = " <p style='margin-bottom: 10px !important; font-family: 'Cairo'; color: #ff7070;'><span style='font-size: 14px;  color: #ff7070;'> None لا يوجد</span></p> ";
            }
            replaceTextPairs.Add("[[Requirements]]", requirements);  




            if (!string.IsNullOrEmpty(meeting.MeetingAgenda))
            {

                replaceTextPairs.Add("[[Agenda]]", meeting.MeetingAgenda);
            }
            else
            {
                replaceTextPairs.Add("[[Agenda]]", "Not Specified غير محدد");
            }


            if (bodyString.Length > 0)
            {
                mailRequest.HtmlBody = FormatMessage(bodyString, replaceTextPairs);
            }

            return mailRequest;
        }
       
        public async Task<MailRequest> BuildMeetingCancellationEmail(Meeting meeting,ApprovalStatus previousStatus)
        {
            var bodyString = await GetEmailTemplateBody("MeetingCancellation.html");

            var mailRequest = new MailRequest
            {
                Subject = "Darah Meeting Cancelled    إالغاء اجتماع دارة الملك عبد العزيز"
            };

            mailRequest.ToEmailAddresses.Add(meeting.Host.EmailAddress);
            if(previousStatus == ApprovalStatus.Approved)
            {
                foreach (var attendee in meeting.MeetingAttendees)
                {
                    if (attendee.AttendeeType == Domain.Enums.AttendeeType.External)
                    {
                        mailRequest.ToEmailAddresses.Add(attendee.ExternalAttendeeEmailAddress);
                    }
                    else
                    {
                        mailRequest.ToEmailAddresses.Add(attendee.Employee.EmailAddress);
                    }

                }
            }
            //replace words in template
            var replaceTextPairs = new Dictionary<string, string>();
            replaceTextPairs.Add("[[Subject]]", meeting.Subject);

            replaceTextPairs.Add("[[DayInWords]]", meeting.Date.ToString("dddd, dd MMMM yyyy", CultureInfo.InvariantCulture));
            DateTime time_from = DateTime.Today.Add(meeting.Time_From);
            replaceTextPairs.Add("[[Time_From]]", time_from.ToString("hh:mm tt"));
            DateTime time_to = DateTime.Today.Add(meeting.Time_To);
            replaceTextPairs.Add("[[Time_To]]", time_to.ToString("hh:mm tt"));

            replaceTextPairs.Add("[[DayInWords_AR]]", meeting.Date.ToString("dddd, dd MMMM yyyy", CultureInfo.GetCultureInfo("ar")));
            DateTime time_from_AR = DateTime.Today.Add(meeting.Time_From);
            replaceTextPairs.Add("[[Time_From_AR]]", time_from.ToString("hh:mm tt", CultureInfo.GetCultureInfo("ar")));
            DateTime time_to_AR = DateTime.Today.Add(meeting.Time_To);
            replaceTextPairs.Add("[[Time_To_AR]]", time_to.ToString("hh:mm tt", CultureInfo.GetCultureInfo("ar")));

            var guestCount = meeting.MeetingAttendees.Count + 1;
            replaceTextPairs.Add("[[GuestCount]]", guestCount.ToString());

            var guestsData = "";
            guestsData += ReplaceAttendeeData(meeting.Host.DisplayName, meeting.Host.EmailAddress, true);

            foreach (var attendee in meeting.MeetingAttendees)
            {
                if (attendee != null && attendee.AttendeeType == Domain.Enums.AttendeeType.External)
                {
                    guestsData += ReplaceAttendeeData(attendee.ExternalAttendeeNameEn, attendee.ExternalAttendeeEmailAddress);
                }
                else if (attendee != null && attendee.AttendeeType == Domain.Enums.AttendeeType.Internal)
                {
                    guestsData += ReplaceAttendeeData(attendee.Employee.DisplayName, attendee.Employee.EmailAddress);
                }

            }

            replaceTextPairs.Add("[[Guests]]", guestsData);

            replaceTextPairs.Add("[[Name]]", meeting.Subject);

            if (bodyString.Length > 0)
            {
                mailRequest.HtmlBody = FormatMessage(bodyString, replaceTextPairs);
            }

            return mailRequest;
        }

        public async Task<MailRequest> BuildMeetingRejectionEmail(Meeting meeting)
        {
            var bodyString = await GetEmailTemplateBody("MeetingRejection.html");

            var mailRequest = new MailRequest
            {
                Subject = "Darah Meeting Rejected    رفض اجتماع دارة الملك عبدالعزيز"
            };

            mailRequest.ToEmailAddresses.Add(meeting.Host.EmailAddress);

            //replace words in template
            var replaceTextPairs = new Dictionary<string, string>();
            replaceTextPairs.Add("[[Subject]]", meeting.Subject);

            replaceTextPairs.Add("[[DayInWords]]", meeting.Date.ToString("dddd, dd MMMM yyyy", CultureInfo.InvariantCulture));
            DateTime time_from = DateTime.Today.Add(meeting.Time_From);
            replaceTextPairs.Add("[[Time_From]]", time_from.ToString("hh:mm tt"));
            DateTime time_to = DateTime.Today.Add(meeting.Time_To);
            replaceTextPairs.Add("[[Time_To]]", time_to.ToString("hh:mm tt"));

            replaceTextPairs.Add("[[DayInWords_AR]]", meeting.Date.ToString("dddd, dd MMMM yyyy", CultureInfo.GetCultureInfo("ar")));
            DateTime time_from_AR = DateTime.Today.Add(meeting.Time_From);
            replaceTextPairs.Add("[[Time_From_AR]]", time_from.ToString("hh:mm tt", CultureInfo.GetCultureInfo("ar")));
            DateTime time_to_AR = DateTime.Today.Add(meeting.Time_To);
            replaceTextPairs.Add("[[Time_To_AR]]", time_to.ToString("hh:mm tt", CultureInfo.GetCultureInfo("ar")));

            var guestCount = meeting.MeetingAttendees.Count + 1;
            replaceTextPairs.Add("[[GuestCount]]", guestCount.ToString());

            var guestsData = "";
            guestsData += ReplaceAttendeeData(meeting.Host.DisplayName, meeting.Host.EmailAddress, true);


            replaceTextPairs.Add("[[Reason]]", meeting.Reason);

            if (bodyString.Length > 0)
            {
                mailRequest.HtmlBody = FormatMessage(bodyString, replaceTextPairs);
            }

            return mailRequest;
        }


        //Webinar Emails 

        public async Task<MailRequest> BuildWebinarCreationEmail(Webinar webinar)
        {
            var bodyString = await GetEmailTemplateBody("PendingWebinarApproval.html");
            var systemAdmins = await _employeeService.GetSystemAdmins();
            var mailRequest = new MailRequest
            {
                Subject = "Webinar Approval Pending  ندوة قيد الانتظار"
            };
            if (systemAdmins != null)
            {
                foreach (var admin in systemAdmins)
                {
                    mailRequest.ToEmailAddresses.Add(admin.EmailAddress);
                }
            }
            //replace words in template
            var replaceTextPairs = new Dictionary<string, string>();
            replaceTextPairs.Add("[[Subject]]", webinar.Subject);
            replaceTextPairs.Add("[[EmployeeName]]", webinar.Host.DisplayName);
            replaceTextPairs.Add("[[DayInWords]]", webinar.Date.ToString("dddd, dd MMMM yyyy", CultureInfo.InvariantCulture));
            DateTime time_from = DateTime.Today.Add(webinar.Time_From);
            replaceTextPairs.Add("[[Time_From]]", time_from.ToString("hh:mm tt"));
            DateTime time_to = DateTime.Today.Add(webinar.Time_To);
            replaceTextPairs.Add("[[Time_To]]", time_to.ToString("hh:mm tt"));
            double duration = (webinar.Time_To - webinar.Time_From).TotalMinutes;
            replaceTextPairs.Add("[[DurationInMinutes]]", duration.ToString());
            replaceTextPairs.Add("[[DayInWords_AR]]", webinar.Date.ToString("dddd, dd MMMM yyyy", CultureInfo.GetCultureInfo("ar")));
            DateTime time_from_AR = DateTime.Today.Add(webinar.Time_From);
            replaceTextPairs.Add("[[Time_From_AR]]", time_from.ToString("hh:mm tt", CultureInfo.GetCultureInfo("ar")));
            DateTime time_to_AR = DateTime.Today.Add(webinar.Time_To);
            replaceTextPairs.Add("[[Time_To_AR]]", time_to.ToString("hh:mm tt", CultureInfo.GetCultureInfo("ar")));
            DateTime Duration_AR = DateTime.Today.Add(webinar.Time_To - webinar.Time_From);


            if (bodyString.Length > 0)
            {
                mailRequest.HtmlBody = FormatMessage(bodyString, replaceTextPairs);
            }
            return mailRequest;
        }


        public async Task<MailRequest> BuildWebinarInvitationEmail(Webinar webinar) //The details with reg url and webinar url
        {
            var bodyString = await GetEmailTemplateBody("WebinarInvitation.html");
            //var systemAdmins = await _employeeService.GetSystemAdmins();
            var mailRequest = new MailRequest
            {
                Subject = "Darah Webinar Invitation دعوة ندوة دارة الملك عبدالعزيز"
            };

            mailRequest.ToEmailAddresses.Add(webinar.Host.EmailAddress);
            foreach (var attendee in webinar.WebinarPanelists)
            {
                if (attendee.AttendeeType == Domain.Enums.AttendeeType.External)
                {
                    mailRequest.ToEmailAddresses.Add(attendee.ExternalPanelistEmailAddress);
                }
                else
                {
                    mailRequest.ToEmailAddresses.Add(attendee.Employee.EmailAddress);
                }
            }
            //if (systemAdmins != null)
            //{
            //    foreach (var admin in systemAdmins)
            //    {
            //        mailRequest.BccEmailAddresses.Add(admin.EmailAddress);
            //    }

            //}

            if (webinar.IsAttachmentShared)
            {
                foreach (var attachment in webinar.WebinarAttachments)
                {
                    var fileStream = await _fileServices.GetFile(attachment.FilePath);
                    MailAttachment file = new MailAttachment(fileStream, attachment.FileName);
                    mailRequest.Attachments.Add(file);
                }
            }
            //replace words in template
            var replaceTextPairs = new Dictionary<string, string>();
            replaceTextPairs.Add("[[Subject]]", webinar.Subject);
            replaceTextPairs.Add("[[DayInWords]]", webinar.Date.ToString("dddd, dd MMMM yyyy", CultureInfo.InvariantCulture));
            DateTime time_from = DateTime.Today.Add(webinar.Time_From);
            replaceTextPairs.Add("[[Time_From]]", time_from.ToString("hh:mm tt"));
            DateTime time_to = DateTime.Today.Add(webinar.Time_To);
            replaceTextPairs.Add("[[Time_To]]", time_to.ToString("hh:mm tt"));
            replaceTextPairs.Add("[[DayInWords_AR]]", webinar.Date.ToString("dddd, dd MMMM yyyy", CultureInfo.GetCultureInfo("ar")));
            DateTime time_from_AR = DateTime.Today.Add(webinar.Time_From);
            replaceTextPairs.Add("[[Time_From_AR]]", time_from.ToString("hh:mm tt", CultureInfo.GetCultureInfo("ar")));
            DateTime time_to_AR = DateTime.Today.Add(webinar.Time_To);
            replaceTextPairs.Add("[[Time_To_AR]]", time_to.ToString("hh:mm tt", CultureInfo.GetCultureInfo("ar")));
           replaceTextPairs.Add("[[WebinarURL]]", webinar.WebinarUrl);

            if(webinar.RegistrationUrl == "" || webinar.RegistrationUrl == null)
            {
                replaceTextPairs.Add("[[RegistrationURL]]", "<p style='margin-bottom: 10px !important;font-family: 'Cairo'; color: #ff7070;'><span style='font-size: 14px;  color: #ff7070;'> None لا يوجد</span></p>");
                
            }
            else
            {
                replaceTextPairs.Add("[[RegistrationURL]]" , $"<a href='{webinar.RegistrationUrl}'>{webinar.RegistrationUrl}</a>");
            }

            var panelistCount = webinar.WebinarPanelists.Count + 1;
            replaceTextPairs.Add("[[PanelistCount]]", panelistCount.ToString());

            var panelists = "";
            panelists += ReplaceAttendeeData(webinar.Host.DisplayName, webinar.Host.EmailAddress, true);

            foreach (var attendee in webinar.WebinarPanelists)
            {
                if (attendee != null && attendee.AttendeeType == Domain.Enums.AttendeeType.External)
                {
                    panelists += ReplaceAttendeeData(attendee.ExternalPanelistNameEn, attendee.ExternalPanelistEmailAddress);
                }
                else if (attendee != null && attendee.AttendeeType == Domain.Enums.AttendeeType.Internal)
                {
                    panelists += ReplaceAttendeeData(attendee.Employee.DisplayName, attendee.Employee.EmailAddress);
                }
            }

            replaceTextPairs.Add("[[Panelists]]", panelists);




            var interpreters = "";

            if (webinar.Interpreters != null && webinar.Interpreters.Count>0)
            {
                foreach (var interpreter in webinar.Interpreters)
                {
                        interpreters += ReplacInterpreterData(interpreter.EmailAddress , interpreter.FromLanguage.Name, interpreter.ToLanguage.Name);
                }
            }
            else
            {
                interpreters += $"<tr><td style = 'min-width: 5px; border: none; user-select: text;' > " +
                      $"<p style='margin-bottom: 10px !important;  font-family: 'Cairo'; color: #ff7070;'><span style='font-size: 14px;  color: #ff7070;'> None لا يوجد</span></p>" +
                      $"</td>" +
                       $"<td style='min-width: 5px;border: none; user-select: text;'>" +
                           "<p style='margin-bottom: 10px !important;font-family: 'Cairo'; color: rgb(59, 56, 56);'><span style='font-size: 14px; '>&nbsp; &nbsp;</span></p>" +
                       "</td></tr>";
            }

            replaceTextPairs.Add("[[Interpreters]]", interpreters);



            replaceTextPairs.Add("[[Name]]", webinar.Subject);
            string requirements = "";
            if (webinar.WebinarRequirements != null && webinar.WebinarRequirements.Count>0)
            {
                var count = 1;
                foreach (var req in webinar.WebinarRequirements)
                {
                    if (!string.IsNullOrWhiteSpace(req.Details))
                    {
                        requirements += $"<tr><td style = 'min-width: 5px; border: none; user-select: text;' ><p style='margin-bottom: 10px !important; font-size: 16px; font-family: 'Cairo'; color: rgb(59, 56, 56);'><span style='font-size: 14px;'>{count}</span></p>" +
                                        $"</td> <td style='min-width: 5px;border: none; user-select: text;'>" +
                                        $"<p style='margin-bottom: 10px !important; font-size: 16px; font-family: 'Cairo'; color: rgb(59, 56, 56);'><span style='font-size: 14px;'> {req.Details} </span></p>" +
                                        "</td></tr>";
                    }
                }
            }
            else
            {
                requirements = "<tr><td> <p style='margin-bottom: 10px !important;  font-family: 'Cairo'; color: #ff7070;'><span style='font-size: 14px;  color: #ff7070;'> None لا يوجد</span></p> </td></tr>";
            }
            replaceTextPairs.Add("[[Requirements]]", requirements);

            if (!string.IsNullOrEmpty(webinar.Agenda))
            {
                replaceTextPairs.Add("[[Agenda]]", webinar.Agenda);
            }
            else
            {
                replaceTextPairs.Add("[[Agenda]]", "Not Specified غير محدد");
            }

            if (bodyString.Length > 0)
            {
                mailRequest.HtmlBody = FormatMessage(bodyString, replaceTextPairs);
            }

            return mailRequest;
        }


        public async Task<MailRequest> BuildWebinarCancellationEmail(Webinar webinar, ApprovalStatus previousStatus)
        {
            var bodyString = await GetEmailTemplateBody("WebinarCancellation.html");
            var mailRequest = new MailRequest
            {
                Subject = "Webinar Cancelled   إالغاء ندوة"
            };
            mailRequest.ToEmailAddresses.Add(webinar.Host.EmailAddress);
            if (previousStatus == ApprovalStatus.Approved)
            {
                if (webinar.WebinarPanelists != null && webinar.WebinarPanelists.Count > 0)
                {
                    foreach (var attendee in webinar.WebinarPanelists)
                    {
                        if (attendee.AttendeeType == Domain.Enums.AttendeeType.External)
                        {
                            mailRequest.ToEmailAddresses.Add(attendee.ExternalPanelistEmailAddress);
                        }
                        else
                        {
                            mailRequest.ToEmailAddresses.Add(attendee.Employee.EmailAddress);
                        }
                    }
                }
                if (webinar.Interpreters != null && webinar.Interpreters.Count > 0)
                {

                    foreach (var interpreter in webinar.Interpreters)
                    {
                        
                        mailRequest.ToEmailAddresses.Add(interpreter.EmailAddress);
                    }
                }
            }
            //replace words in template
            var replaceTextPairs = new Dictionary<string, string>();
            replaceTextPairs.Add("[[Subject]]", webinar.Subject);
            replaceTextPairs.Add("[[DayInWords]]", webinar.Date.ToString("dddd, dd MMMM yyyy", CultureInfo.InvariantCulture));
            DateTime time_from = DateTime.Today.Add(webinar.Time_From);
            replaceTextPairs.Add("[[Time_From]]", time_from.ToString("hh:mm tt"));
            DateTime time_to = DateTime.Today.Add(webinar.Time_To);
            replaceTextPairs.Add("[[Time_To]]", time_to.ToString("hh:mm tt"));
            replaceTextPairs.Add("[[DayInWords_AR]]", webinar.Date.ToString("dddd, dd MMMM yyyy", CultureInfo.GetCultureInfo("ar")));
            DateTime time_from_AR = DateTime.Today.Add(webinar.Time_From);
            replaceTextPairs.Add("[[Time_From_AR]]", time_from.ToString("hh:mm tt", CultureInfo.GetCultureInfo("ar")));
            DateTime time_to_AR = DateTime.Today.Add(webinar.Time_To);
            replaceTextPairs.Add("[[Time_To_AR]]", time_to.ToString("hh:mm tt", CultureInfo.GetCultureInfo("ar")));
            var panelistCount = webinar.WebinarPanelists.Count + 1;
            replaceTextPairs.Add("[[PanelistCount]]", panelistCount.ToString());
            var panelists = "";
            panelists += ReplaceAttendeeData(webinar.Host.DisplayName, webinar.Host.EmailAddress, true);

            foreach (var attendee in webinar.WebinarPanelists)
            {
                if (attendee != null && attendee.AttendeeType == Domain.Enums.AttendeeType.External)
                {
                    panelists += ReplaceAttendeeData(attendee.ExternalPanelistNameEn, attendee.ExternalPanelistEmailAddress);
                }
                else if (attendee != null && attendee.AttendeeType == Domain.Enums.AttendeeType.Internal)
                {
                    panelists += ReplaceAttendeeData(attendee.Employee.DisplayName, attendee.Employee.EmailAddress);
                }
            }
            replaceTextPairs.Add("[[Panelists]]", panelists);




            var interpreters = "";

            if (webinar.Interpreters != null && webinar.Interpreters.Count > 0)
            {
                foreach (var interpreter in webinar.Interpreters)
                {
                    interpreters += ReplacInterpreterData(interpreter.EmailAddress, interpreter.FromLanguage.Name, interpreter.ToLanguage.Name);
                }
            }
            else
            {
                interpreters += $"<tr><td style = 'min-width: 5px; border: none; user-select: text;' > " +
                      $"<p style='margin-bottom: 10px !important; font-size: 16px; font-family: 'Cairo'; color: #ff7070;'><span style='font-size: 14px;  color: #ff7070;'> None لا يوجد</span></p>" +
                      $"</td>" +
                       $"<td style='min-width: 5px;border: none; user-select: text;'>" +
                           "<p style='margin-bottom: 10px !important; font-family: 'Cairo'; color: rgb(59, 56, 56);'><span style='font-size: 14px;'>&nbsp; &nbsp;</span></p>" +
                       "</td></tr>";
            }

            replaceTextPairs.Add("[[Interpreters]]", interpreters);



            if (bodyString.Length > 0)
            {
                mailRequest.HtmlBody = FormatMessage(bodyString, replaceTextPairs);
            }
            return mailRequest;
        }



        public async Task<MailRequest> BuildWebinarRejectionEmail(Webinar webinar)
        {
            var bodyString = await GetEmailTemplateBody("WebinarRejection.html");

            var mailRequest = new MailRequest
            {
                Subject = "Darah Webinar Rejected    رفض ندوة دارة الملك عبدالعزيز"
            };

            mailRequest.ToEmailAddresses.Add(webinar.Host.EmailAddress);

            //replace words in template
            var replaceTextPairs = new Dictionary<string, string>();
            replaceTextPairs.Add("[[Subject]]", webinar.Subject);

            replaceTextPairs.Add("[[DayInWords]]", webinar.Date.ToString("dddd, dd MMMM yyyy", CultureInfo.InvariantCulture));
            DateTime time_from = DateTime.Today.Add(webinar.Time_From);
            replaceTextPairs.Add("[[Time_From]]", time_from.ToString("hh:mm tt"));
            DateTime time_to = DateTime.Today.Add(webinar.Time_To);
            replaceTextPairs.Add("[[Time_To]]", time_to.ToString("hh:mm tt"));

            replaceTextPairs.Add("[[DayInWords_AR]]", webinar.Date.ToString("dddd, dd MMMM yyyy", CultureInfo.GetCultureInfo("ar")));
            DateTime time_from_AR = DateTime.Today.Add(webinar.Time_From);
            replaceTextPairs.Add("[[Time_From_AR]]", time_from.ToString("hh:mm tt", CultureInfo.GetCultureInfo("ar")));
            DateTime time_to_AR = DateTime.Today.Add(webinar.Time_To);
            replaceTextPairs.Add("[[Time_To_AR]]", time_to.ToString("hh:mm tt", CultureInfo.GetCultureInfo("ar")));

            var panelists = webinar.WebinarPanelists.Count + 1;
            replaceTextPairs.Add("[[PanelistCount]]", panelists.ToString());

            var guestsData = "";
            guestsData += ReplaceAttendeeData(webinar.Host.DisplayName, webinar.Host.EmailAddress, true);


            replaceTextPairs.Add("[[Reason]]", webinar.Reason);

            if (bodyString.Length > 0)
            {
                mailRequest.HtmlBody = FormatMessage(bodyString, replaceTextPairs);
            }

            return mailRequest;
        }




        private string ReplaceAttendeeData(string attendeeName,string attendeeEmail, bool isHost=false)
        {
            string attendeeData = "<tr>";
            attendeeData += $"<td style = 'min-width: 5px; border: none; user-select: text;' > " +
                   $"<p style='margin-bottom: 10px !important; font-size: 16px; font-family: 'Cairo'; color: rgb(59, 56, 56);'><span style='font-size: 14px;'>{attendeeName}</span></p>" +
                   $"</td>" +
                    $"<td style='min-width: 5px;border: none; user-select: text;'>"+
                        "<p style='margin-bottom: 10px !important; font-size: 16px; font-family: 'Cairo'; color: rgb(59, 56, 56);'><span style='font-size: 14px;'>&nbsp; &nbsp;</span></p>"+
                    "</td>"
                   +
                   $"<td style = 'min-width: 5px; border: none; user-select: text;'>" +
                   $"<p style = 'margin-bottom: 10px !important; font-size: 16px; font-family: 'Cairo'; color: rgb(59, 56, 56);'><span style='font-size: 14px;'>{attendeeEmail}</span></p>" +
                    $"<td style='min-width: 5px;border: none; user-select: text;'>" +
                        "<p style='margin-bottom: 10px !important; font-size: 16px; font-family: 'Cairo'; color: rgb(59, 56, 56);'><span style='font-size: 14px;'>&nbsp; &nbsp;</span></p>" +
                    "</td>"
                   +
                   $"</td>";
            if (isHost)
            {
                attendeeData += $"<td style = 'min-width: 5px; border: none; user-select: text;' > " +
                   $"<p style='margin-bottom: 10px !important; font-size: 16px; font-family: 'Cairo';'><span style='font-size: 14px; color: #ff7070;'>Host مضيف</span></p>" +
                   $"</td>";
            }
            else
            {
                attendeeData += $"<td style = 'min-width: 5px; border: none; user-select: text;' > " +
                 $"<p style='margin-bottom: 10px !important; font-size: 16px; font-family: 'Cairo'; color: #ff7070;'><span style='font-size: 14px;'></span></p>" +
                 $"</td>";
            }
            attendeeData += "</tr>";
            return attendeeData;
        }

        private string ReplacInterpreterData( string interpreterEmail,string langFrom, string LangTo)
        {
            string attendeeData = "<tr>";
            attendeeData += $"<td style = 'min-width: 5px; border: none; user-select: text;' > " +
                   $"<p style='margin-bottom: 10px !important; font-size: 16px; font-family: 'Cairo'; color: rgb(59, 56, 56);'><span style='font-size: 14px;'>{interpreterEmail}</span></p>" +
                   $"</td>" +
                   $"<td style = 'min-width: 5px; border: none; user-select: text;'>" +
                   $"<p style = 'margin-bottom: 10px !important; font-size: 16px; font-family: 'Cairo'; color: rgb(59, 56, 56);'><span style='font-size: 14px;'>{langFrom} &#8594; {LangTo}</span></p>" +
                    $"<td style='min-width: 5px;border: none; user-select: text;'>" +
                        "<p style='margin-bottom: 10px !important; font-size: 16px; font-family: 'Cairo'; color: rgb(59, 56, 56);'><span style='font-size: 14px;'>&nbsp; &nbsp;</span></p>" +
                    "</td>"
                   +
                   $"</td>";
                attendeeData += $"<td style = 'min-width: 5px; border: none; user-select: text;' > " +
                 $"<p style='margin-bottom: 10px !important; font-size: 16px; font-family: 'Cairo'; color: #ff7070;'><span style='font-size: 14px;'></span></p>" +
                 $"</td>";
            attendeeData += "</tr>";
            return attendeeData;
        }


        private string FormatMessage(StringBuilder mailBody, Dictionary<string, string> replaceTextPairs)
        {
            foreach (var keyValue in replaceTextPairs)
            {
                mailBody = mailBody.Replace(keyValue.Key, keyValue.Value);
            }

            return mailBody.ToString();
        }


    }
}
