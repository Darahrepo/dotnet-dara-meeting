using MeetingScheduler.Domain.Common;

namespace MeetingScheduler.Domain.Entities
{
    public class SampleUser : AuditableEntity
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string EmailAddress { get; set; }
    }
}
