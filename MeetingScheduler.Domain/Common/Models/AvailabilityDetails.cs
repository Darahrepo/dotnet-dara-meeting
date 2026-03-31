using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Common.Models
{
    public class AvailabilityDetails
    {
        public DateTime Date { get; set; }
        public TimeSpan To { get; set; }
        public TimeSpan From { get; set; }
        public int HostId { get; set; }
        public int Room { get; set; }
        public int MeetingId { get; set; }
        public string LocationType { get; set; }
    }
}
