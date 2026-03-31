using AutoMapper;
using MeetingScheduler.Domain.Common;
using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Domain.Enums;
using MeetingScheduler.Infrastructure.Common.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Infrastructure.Services.Meetings
{
    public class MeetingAttendeeDto : AuditableEntity, IMapFrom<MeetingAttendee>
    {
        public int Id { get; set; }
        public int MeetingId { get; set; }
        public Meeting Meeting { get; set; }
        public AttendeeType AttendeeType { get; set; }
        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public string ExternalAttendeeNameEn { get; set; }
        public string ExternalAttendeeNameAr { get; set; }
        public string ExternalAttendeeEmailAddress { get; set; }
        public bool IsHost { get; set; }



        public void Mapping(Profile profile)
        {
            profile.CreateMap<MeetingAttendeeDto, MeetingAttendee>(MemberList.Destination).ReverseMap();

        }
    }
}
