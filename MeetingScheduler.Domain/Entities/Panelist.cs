using MeetingScheduler.Domain.Common;
using MeetingScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Entities
{
    public class Panelist : AuditableEntity
    {
        public int Id { get; set; }
        public int WebinarId { get; set; }
        public Webinar Webinar { get; set; }
        public AttendeeType AttendeeType { get; set; }
        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public string ExternalPanelistNameEn { get; set; }
        public string ExternalPanelistNameAr { get; set; }
        public string ExternalPanelistEmailAddress { get; set; }
    }
}
