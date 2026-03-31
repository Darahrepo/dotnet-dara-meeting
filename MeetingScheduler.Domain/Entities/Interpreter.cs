using MeetingScheduler.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Entities
{
    public class Interpreter : AuditableEntity
    {
        public int Id { get; set; }
        public int WebinarId { get; set; }
        public Webinar Webinar { get; set; }
        public string EmailAddress { get; set; }
        public int FromLanguageId{ get; set; }
        public Language FromLanguage { get; set; }
        public int ToLanguageId { get; set; }
        public Language ToLanguage { get; set; }
    }
}
