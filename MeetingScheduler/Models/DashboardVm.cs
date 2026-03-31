using MeetingScheduler.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingScheduler.UI.Models
{
    public class DashboardVm
    {
        public int MeetingsToday { get; set; }
        public int MeetingsPending { get; set; }
        public int MeetingsScheduled { get; set; }
        public int MeetingsDone{ get; set; }
        public string CalenderEvents { get; set; }
        public List<Meeting> Meetings { get; set; }
        public List<Webinar> Webinars { get; set; }
    }

    public class CalenderEvent 
    {
        public string title { get; set; }
        public string id { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string backgroundColor { get; set; }
        public string borderColor { get; set; }
    }
}
