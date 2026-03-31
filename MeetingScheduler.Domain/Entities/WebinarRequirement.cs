using MeetingScheduler.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Entities
{
    public class WebinarRequirement : AuditableEntity
    {
        public int Id { get; set; }
        public int WebinarId{ get; set; }
        public Webinar Webinar{ get; set; }
        public string Details { get; set; }
    }
}
