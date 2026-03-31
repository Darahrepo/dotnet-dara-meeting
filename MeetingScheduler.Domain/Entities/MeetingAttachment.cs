using MeetingScheduler.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Entities
{
    public class MeetingAttachment : AuditableEntity
    {
        public int Id { get; set; }
        public int MeetingId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
    }
}
