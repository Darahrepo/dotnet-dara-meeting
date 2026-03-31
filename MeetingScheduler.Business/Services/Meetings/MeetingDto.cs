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
    public class MeetingDto : AuditableEntity, IMapFrom<Meeting>
    {
        public MeetingDto()
        {
            MeetingAttachments = new List<MeetingAttachment>();
            MeetingAttendees = new List<MeetingAttendee>();
            MeetingRequirements = new List<MeetingItem>();
        }

        public int Id { get; set; }
        public List<MeetingAttendee> MeetingAttendees { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan From { get; set; }
        public TimeSpan To { get; set; }
        public List<MeetingAttachment> MeetingAttachments { get; set; }
        public LocationType MeetingLocationType { get; set; }
        public List<MeetingItem> MeetingRequirements { get; set; }
        public string MeetingLink { get; set; }
        public int MeetingRoom { get; set; }
        public string MeetingAgenda { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
        public string Reason { get; set; }



        public void Mapping(Profile profile)
        {
            profile.CreateMap<MeetingDto, Meeting>(MemberList.Destination).ReverseMap();
            profile.CreateMap<MeetingAttendeeDto, MeetingAttendee>(MemberList.Destination).ReverseMap();
            //profile.CreateMap<MeetingAttachmentsDto, MeetingAttendee>(MemberList.Destination).ReverseMap();

        }
    }
}
