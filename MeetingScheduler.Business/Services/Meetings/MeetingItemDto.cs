using AutoMapper;
using MeetingScheduler.Domain.Common;
using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Infrastructure.Common.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Infrastructure.Services.Meetings
{
    public class MeetingItemDto : AuditableEntity, IMapFrom<MeetingItem>
    {
        public int Id { get; set; }
        public int MeetingId { get; set; }
        public Meeting Meeting { get; set; }
        public string ItemName { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<MeetingItemDto, MeetingItem>(MemberList.Destination).ReverseMap();

        }
    }
}
