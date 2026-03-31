using AutoMapper;
using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Infrastructure.Common.Mappings;

namespace MeetingScheduler.Infrastructure.Services.SampleUsers
{
    public class SampleUserDto :IMapFrom<SampleUser>
    {
        public int Id { get; set; }
        public string NameEn{ get; set; }
        public string NameAr { get; set; }
        public string EmailAddress { get; set; }

        public void Mapping(Profile profile) 
        {
            profile.CreateMap<SampleUser, SampleUserDto>(MemberList.Destination).ReverseMap();

        }
    }
}
