using MeetingScheduler.Domain.Common;
using MeetingScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Entities
{
    public class Meeting : AuditableEntity
    {
        public Meeting()
        {
            //MeetingAttendees = new List<MeetingAttendee>();
            //MeetingAttachments = new List<MeetingAttachment>();
            //MeetingRequirements = new List<MeetingItem>();
        }
        public int Id { get; set; }
        public string Subject { get; set; }
        public List<MeetingAttendee> MeetingAttendees { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time_From { get; set; }
        public TimeSpan Time_To { get; set; }
        public List<MeetingAttachment> MeetingAttachments { get; set; }
        public LocationType MeetingLocationType { get; set; }
        public List<MeetingItem> MeetingRequirements { get; set; }
        public string MeetingLink { get; set; }
        public int? MeetingRoomId { get; set; }
        public MeetingRoom MeetingRoom { get; set; }
        public int HostId { get; set; }
        public Employee Host { get; set; }
        public string MeetingAgenda { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
        public string Reason { get; set; }
        public string MeetingId { get; set; }
        public string MeetingPassword { get; set; }
        public string WebexMeetingNumber { get; set; }
        public bool IsRecorded { get; set; }
        public bool IsAttachmentShared { get; set; }
        public bool IsCeo { get; set; }
        public bool IsWebex { get; set; }
        public ZoomUserType ZoomAccount { get; set; }
    }
}
