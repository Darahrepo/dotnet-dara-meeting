using AutoMapper;
using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Infrastructure.Common.Mappings;
using MeetingScheduler.Infrastructure.Services.Meetings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingScheduler.UI.Models
{
    public class MeetingItemVm : IMapFrom<MeetingItem>
    {
        public int Id { get; set; }
        public int MeetingId { get; set; }
        public Meeting Meeting { get; set; }
        public string ItemName { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<MeetingItemVm, MeetingItem>().ReverseMap();
        }
    }
}
