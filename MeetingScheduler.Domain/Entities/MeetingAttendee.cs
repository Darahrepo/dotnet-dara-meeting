using MeetingScheduler.Domain.Common;
using MeetingScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Entities
{
    public class MeetingAttendee : AuditableEntity
    {
        public int Id { get; set; }
        public int MeetingId { get; set; }
        public Meeting Meeting { get; set; }
        public AttendeeType AttendeeType { get; set; }
        public int? EmployeeId { get; set; }
        public Employee Employee{ get; set; }
        public string ExternalAttendeeNameEn { get; set; }
        public string ExternalAttendeeNameAr { get; set; }
        public string ExternalAttendeeEmailAddress { get; set; }
    }
}
