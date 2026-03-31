using MeetingScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Common.Models
{
    public class MeetingDetails
    {
        public int ResponseStatusCode { get; set; }
        public string Topic { get; set; }
        public string Agenda { get; set; }
        public string Host { get; set; }
        public bool RecordedMeeting { get; set; }
        public List<string> Attendees { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public double DurationInMinutes { get; set; }
        public string JoiningUrl {get;set;}
        public string MeetingId {get;set;}
        public string Password {get;set; }
        public ZoomUserType ZoomAccount { get;set; }
        public WebexUserType WebexAccount { get;set; }
        public string WebexMeetingNumber { get; set; } = string.Empty;
        public string Timezone { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public List<Invitee> Invitees {  get; set; }
        public string ScheduledType { get; set; } = "meeting";
    }

    public class Invitee
    {
        public string Email { get;set; }
        public string DisplayName { get;set; }
        public bool Cohost { get; set; } = false;
        public bool Panelist { get; set; } = false;
    }
}
