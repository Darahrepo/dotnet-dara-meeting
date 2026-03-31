

namespace MeetingScheduler.Domain.Common.Models
{
    public class ZoomSettings
    {
       public ZoomAccessDetails Main { get; set; }
       public ZoomAccessDetails Ceo { get; set; }
       public string ApiUrl { get; set; }
    }

    public class ZoomAccessDetails
    {
        public string AccountUser { get; set; }
        public string AccountId { get; set; }
        public string Client { get; set; } //APIKey
        public string ClientSecret { get; set; }
        public int AccessTokenLifeTimeMinutes { get; set; }
    }
}
