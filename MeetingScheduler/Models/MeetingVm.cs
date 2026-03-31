using AutoMapper;
using FluentValidation;
using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Domain.Enums;
using MeetingScheduler.Domain.Common.Interfaces;
using MeetingScheduler.Infrastructure.Common.Extensions;
using MeetingScheduler.Infrastructure.Common.Mappings;
using MeetingScheduler.Infrastructure.Interfaces;
using MeetingScheduler.Infrastructure.Services.MeetingRooms;
using MeetingScheduler.Infrastructure.Services.Meetings;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingScheduler.UI.Models
{
    public class MeetingVm : ToastVm, IMapFrom<Meeting>
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public List<MeetingAttendeeVm> MeetingAttendees { get; set; }
        public DateTime Date { get; set; }

        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan Time_From { get; set; }
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan Time_To { get; set; }
        public List<MeetingAttachmentVm> MeetingAttachments { get; set; }
        public LocationType MeetingLocationType { get; set; }
        public List<MeetingItemVm> MeetingRequirements { get; set; }
        public string MeetingLink { get; set; }
        public string MeetingPassword { get; set; }
        public string MeetingId { get; set; }
        public string WebexMeetingNumber { get; set; }
        public int? MeetingRoomId { get; set; }
        public MeetingRoom MeetingRoom { get; set; }
        public string MeetingAgenda { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;
        public string Reason { get; set; }
        public int HostId { get; set; }
        public EmployeeVm Host { get; set; }
        public string CreatedBy { get; set; }
        public bool IsRecorded { get; set; }
        public bool IsAttachmentShared { get; set; }
        public ZoomUserType ZoomAccount { get; set; }

        //NotMapped
        public List<IFormFile> Files { get; set; }
        public List<string> DeletedImageUrls { get; set; }
        public bool IsWebex { get; set; } = true;
        public bool IsCeo { get; set; }
        public List<string> RequirementsList { get; set; }
        public char MeetingLocation{ get; set; }
        public List<MeetingRoomVm> MeetingRoomslist { get; set; }
        public List<EmployeeVm> EmployeesList { get; set; }
        public List<EmployeeVm> HostList { get; set; }
        public List<ExternalMeetingAttendeesVm> ExternalAttendeesList { get; set; }
        public List<int> MeetingInternalAttendeesId { get; set; }
        public List<EmployeeVm> MeetingInternalAttendees { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<MeetingVm, Meeting>().ReverseMap();


            profile.CreateMap<Meeting, MeetingVm>().ForMember(x => x.Files, opt => opt.Ignore());
            profile.CreateMap<Meeting, MeetingVm>().ForMember(x => x.DeletedImageUrls, opt => opt.Ignore());
            profile.CreateMap<Meeting, MeetingVm>().ForMember(x => x.RequirementsList, opt => opt.Ignore());
            profile.CreateMap<Meeting, MeetingVm>().ForMember(x => x.MeetingLocation, opt => opt.Ignore());
            profile.CreateMap<Meeting, MeetingVm>().ForMember(x => x.MeetingRoomslist, opt => opt.Ignore());
            profile.CreateMap<Meeting, MeetingVm>().ForMember(x => x.ExternalAttendeesList, opt => opt.Ignore());
            profile.CreateMap<Meeting, MeetingVm>().ForMember(x => x.MeetingInternalAttendeesId, opt => opt.Ignore());
            profile.CreateMap<Meeting, MeetingVm>().ForMember(x => x.MeetingInternalAttendees, opt => opt.Ignore());
            profile.CreateMap<Meeting, MeetingVm>().ForMember(x => x.EmployeesList, opt => opt.Ignore());
            profile.CreateMap<Meeting, MeetingVm>().ForMember(x => x.HostList, opt => opt.Ignore());
        }

    }


    public class MeetingVmValidator : AbstractValidator<MeetingVm>
    {
        private readonly IApplicationDbContext _context;
        //private readonly IMeetingRoomService _meetingRoomService;

        public MeetingVmValidator(IApplicationDbContext context)
        {
            _context = context;

            //RuleFor(v => v.MeetingAttendees)
            //    .NotEmpty().WithMessage("Atleast one meeting attendee must be selected.");

            RuleFor(v => v.Date)
               .NotEmpty().WithMessage("Date is required.");

            RuleFor(v => v.Time_From)
            .NotEmpty().WithMessage("From Time is required.");

            RuleFor(v => v.Time_To)
            .NotEmpty().WithMessage("To Time is required.");

            RuleFor(v => v.MeetingLocationType)
            .NotEmpty().WithMessage("Location Type is required.");

            RuleFor(v => v.HostId)
            .NotEmpty().WithMessage("Host is required.");

            RuleFor(v => v.Subject)
            .NotEmpty().WithMessage("Subject is required.");
        }

        //public async Task<bool> BeUnique(string name, CancellationToken cancellationToken)
        //{
        //    return (await _meetingRoomService.GetAll()).All(l => l.NameEn != name && l.NameAr != name);
        //}

        public async Task<bool> BeInArabic(string title, CancellationToken cancellationToken)
        {
            return await title.BeInArabic();
        }
    }
}
