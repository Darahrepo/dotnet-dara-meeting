using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Common.Models
{
    public class ZoomMeetingDetails
    {
        public DateTime start_time { get; set; }
        public int duration { get; set; }
        public DateTime id { get; set; }
        public string join_url { get; set; }
        public string topic { get; set; }
        public string agenda { get; set; }
    }
}
