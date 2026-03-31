using MeetingScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Common.Models
{
    public class ZoomWebinarDetails
    {
        public string webinarId { get; set; }
        public int response_status_code { get; set; }
        public string topic { get; set; }
        public string host { get; set; }
        public ZoomMeetingType type { get; set; }
        public string password { get; set; }
        public bool recorded_webinar { get; set; }
        public List<string> panelists { get; set; }
        public DateTime start_time { get; set; }
        public double duration { get; set; }
        public string agenda { get; set; }
        public string starting_url { get; set; }
        public string joining_url { get; set; }
        public string registration_url { get; set; }
        public string approval_type { get; set; }
        public bool registration_required { get; set; }
        public Settings Settings { get; set; } = new Settings();
        public ZoomUserType ZoomAccount{ get; set; }
    }


    public class Settings
    {
        public string auto_recording { get; set; }
        public bool join_before_host { get; set; } = true;
        public bool waiting_room { get; set; } = false;
        public int approval_type { get; set; }
        public LanguageInterpretation language_interpretation { get; set; } = new LanguageInterpretation();
        public bool registrants_email_notification { get; set; } = true;
        public bool registrants_confirmation_email { get; set; } = true;
    }
    public class LanguageInterpretation
    {
        public bool enable { get; set; }
        public List<Interpreters> interpreters { get; set; } = new List<Interpreters>();
    }
    public class Interpreters
    {
        public string email { get; set; }
        public string languages { get; set; }
    }

}
