using MeetingScheduler.Domain.Common;
using MeetingScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Entities
{
    public class Employee : AuditableEntity
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string DisplayName { get; set; }
        public string FirstNameEn { get; set; }
        public string MiddleNameEn { get; set; }
        public string LastNameEn { get; set; }
        public string FirstNameAr { get; set; }
        public string MiddleNameAr { get; set; }
        public string LastNameAr { get; set; }
        public string EmailAddress{ get; set; }
        public string DepartmentId { get; set; }
        public int RoleId{ get; set; }
        public Role Role{ get; set; }
    }
}
