using MeetingScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Infrastructure.Services.Meetings
{
    public class MeetingTimingDto
    {
        public DateTime Date { get; set; }
        public TimeSpan From { get; set; }
        public TimeSpan To { get; set; }
        public int MeetingRoom { get; set; }
        public LocationType LocationType { get; set; }
    }
}
