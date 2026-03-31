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
    public class PanelistVm :IMapFrom<Panelist>
    {
        public int Id { get; set; }
        public int WebinarId { get; set; }
        public WebinarVm Webinar { get; set; }
        public AttendeeType AttendeeType { get; set; }
        public int? EmployeeId { get; set; }
        public EmployeeVm Employee { get; set; }
        public string ExternalPanelistNameEn { get; set; }
        public string ExternalPanelistNameAr { get; set; }
        public string ExternalPanelistEmailAddress { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<PanelistVm, Panelist>().ReverseMap();
        }
    }
}
