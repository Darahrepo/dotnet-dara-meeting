using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Common.Models
{
    public class WebexMeetingDetails
    {
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }
        public int duration { get; set; }
        public DateTime id { get; set; }
        public string join_url { get; set; }
        public string topic { get; set; }
        public string agenda { get; set; }
    }
}
