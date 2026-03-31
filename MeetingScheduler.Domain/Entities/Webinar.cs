using MeetingScheduler.Domain.Common;
using MeetingScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Entities
{
    public class Webinar : AuditableEntity
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public int HostId { get; set; }
        public Employee Host { get; set; }
        public List<Panelist> WebinarPanelists { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time_From { get; set; }
        public TimeSpan Time_To { get; set; }
        public List<WebinarAttachment> WebinarAttachments { get; set; }
        public List<WebinarRequirement> WebinarRequirements { get; set; }
        public string WebinarUrl { get; set; }
        public string RegistrationUrl { get; set; }
        public string Agenda { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
        public string Reason { get; set; }
        public string ZoomWebinarId { get; set; }
        public string ZoomWebinarPassword { get; set; }
        public bool IsRecorded { get; set; }
        public bool IsAttachmentShared { get; set; }
        public bool IsTranslationNeeded { get; set; }
        public List<Interpreter> Interpreters { get; set; } 
        public bool IsCertificateProvided { get; set; }
        public bool IsRegistrationNeeded { get; set; }
        public ZoomUserType ZoomAccount { get; set; }
    }
}
