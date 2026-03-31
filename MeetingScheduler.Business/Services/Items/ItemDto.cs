using AutoMapper;
using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Infrastructure.Common.Mappings;
using System;

namespace MeetingScheduler.Infrastructure.Services.MeetingItems
{
    public class ItemDto : IMapFrom<Item>
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<ItemDto, Item>(MemberList.Destination).ReverseMap();

        }
    }
}
