using MeetingScheduler.Domain.Common;

namespace MeetingScheduler.Domain.Entities
{
    public class Role : AuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
