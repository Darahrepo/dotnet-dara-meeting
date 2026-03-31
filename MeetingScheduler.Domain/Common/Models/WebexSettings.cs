

namespace MeetingScheduler.Domain.Common.Models
{
    public class WebexSettings
    {
       public string ApiUrl { get; set; }
        public WebexAccessDetails Ceo { get; set; }
        public WebexAccessDetails Main { get; set; }
		public GrantTypes GrantTypes { get; set; }
        public WebexAccessDetailsJSON WebexAccessDetailsJSON { get; set; }
    }

    public class WebexAccessDetails
    {
        public string RedirectUri { get; set; }
        public string AccountUser { get; set; }
        public string ClientId { get; set; } //APIKey
        public string ClientSecret { get; set; }
        public string RefreshToken { get; set; }
        public bool IsActive { get; set; }
    }
    public class WebexAccessDetailsJSON
    {
        public string client_id { get; set; } //APIKey
        public string client_secret { get; set; }
        public string refresh_token { get; set; }
        public string grant_type { get; set; }

    }
    public class GrantTypes
    {
        public string Refresh { get; set; }
        public string Authorize { get; set; }
    }
}
