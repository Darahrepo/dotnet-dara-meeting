using AutoMapper;
using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Domain.Enums;
using MeetingScheduler.Infrastructure.Common.Mappings;
using MeetingScheduler.Infrastructure.Services.Employees;
using MeetingScheduler.Infrastructure.Services.Meetings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingScheduler.UI.Models
{
    public class MeetingAttendeeVm :IMapFrom<MeetingAttendee>
    {
        public int Id { get; set; }
        public int MeetingId { get; set; }
        public MeetingVm Meeting { get; set; }
        public AttendeeType AttendeeType { get; set; }
        public int? EmployeeId { get; set; }
        public EmployeeVm Employee { get; set; }
        public string ExternalAttendeeNameEn { get; set; }
        public string ExternalAttendeeNameAr { get; set; }
        public string ExternalAttendeeEmailAddress { get; set; }
        public bool IsHost { get; set; } = false;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<MeetingAttendeeVm, MeetingAttendee>().ReverseMap();
        }
    }
}
