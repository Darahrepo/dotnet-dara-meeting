using MeetingScheduler.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Entities
{
    public class MeetingItem : AuditableEntity
    {
        public int Id { get; set; }
        public int MeetingId{ get; set; }
        public Meeting Meeting{ get; set; }
        public string ItemName { get; set; }
    }
}
