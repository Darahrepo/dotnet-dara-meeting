using AutoMapper;
using MeetingScheduler.Domain.Common;
using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Infrastructure.Common.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingScheduler.UI.Models
{
    public class WebinarRequirementVm : IMapFrom<WebinarRequirement>
    {
        public int Id { get; set; }
        public int WebinarId { get; set; }
        public WebinarVm Webinar { get; set; }
        public string Details { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<WebinarRequirementVm, WebinarRequirement>().ReverseMap();
        }
    }
}
