using Ical.Net;
using MeetingScheduler.Domain.Common.Models;
using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Common.Interfaces
{
    public interface IEmailBuilderService
    {
        //MEETING
        Task<MailRequest> BuildMeetingInvitationEmail(Meeting meeting); 
        Task<MailRequest> BuildMeetingCreationEmail(Meeting meeting); 
         Task<MailRequest> BuildMeetingCancellationEmail(Meeting meeting, ApprovalStatus previousStatus);
         Task<MailRequest> BuildMeetingRejectionEmail(Meeting meeting);


        //WEBINAR
        Task<MailRequest> BuildWebinarInvitationEmail(Webinar webinar);
        Task<MailRequest> BuildWebinarCreationEmail(Webinar webinar);
        Task<MailRequest> BuildWebinarCancellationEmail(Webinar webinar, ApprovalStatus previousStatus);
        Task<MailRequest> BuildWebinarRejectionEmail(Webinar webinar);
    }
}
