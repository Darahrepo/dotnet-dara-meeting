using AutoMapper;
using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Infrastructure.Common.Mappings;
using System;

namespace MeetingScheduler.Infrastructure.Services.MeetingRooms
{
    public class MeetingRoomDto : IMapFrom<MeetingRoom>
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<MeetingRoomDto, MeetingRoom>(MemberList.Destination).ReverseMap();

        }
    }
}
