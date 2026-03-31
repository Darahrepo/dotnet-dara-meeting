using AutoMapper;
using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Infrastructure.Common.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingScheduler.UI.Models
{
    public class LanguageVm :IMapFrom<Language>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Language, LanguageVm>(MemberList.Destination).ReverseMap();
        }
    }
}
