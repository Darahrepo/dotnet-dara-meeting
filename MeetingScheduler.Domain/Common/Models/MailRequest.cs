using Ical.Net;
using System.Collections.Generic;

namespace MeetingScheduler.Domain.Common.Models
{
    public class MailRequest
    {
        public List<string> ToEmailAddresses { get; set; } = new List<string>();
        public List<string> CcEmailAddresses { get; set; } = new List<string>();
        public List<string> BccEmailAddresses { get; set; } = new List<string>();
        public string Subject { get; set; }
        public string HtmlBody { get; set; }
        public List<MailAttachment> Attachments { get; set; } = new List<MailAttachment>();
        public List<MailAttachment> LinkedResources { get; set; } = new List<MailAttachment>();
        public string MessagePriority { get; set; }
        public string Calendar { get; set; }
    }
}