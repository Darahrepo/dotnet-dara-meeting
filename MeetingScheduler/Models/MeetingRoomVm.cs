using AutoMapper;
using FluentValidation;
using MeetingScheduler.Domain.Common.Interfaces;
using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Infrastructure.Common.Extensions;
using MeetingScheduler.Infrastructure.Common.Mappings;
using MeetingScheduler.Infrastructure.Interfaces;
using MeetingScheduler.Infrastructure.Services.MeetingRooms;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingScheduler.UI.Models
{
    public class MeetingRoomVm : IMapFrom<MeetingRoom>
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string EmailAddress { get; set; }
        public string Name
        {
            get
            {
                return $"{this.NameAr} - {this.NameEn}";
            }
            set
            {
                this.NameAr = value;
            }
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<MeetingRoom, MeetingRoomVm>(MemberList.Destination).ReverseMap();
        }

    }


    //public class MeetingRoomVmValidator : AbstractValidator<MeetingRoomVm>
    //{
    //    //private readonly IApplicationDbContext _context;
    //    //private readonly IMeetingRoomService _meetingRoomService;

    //    //public MeetingRoomVmValidator(IApplicationDbContext context, IMeetingRoomService meetingRoomService)
    //    //{
    //    //    _context = context;
    //    //    _meetingRoomService = meetingRoomService;

    //    //    //RuleFor(v => v.NameEn)
    //    //    //    .NotEmpty().WithMessage("Name in English is required.")
    //    //    //    .MaximumLength(100).WithMessage("Name in English must not exceed 100 characters.")
    //    //    //    .MustAsync(BeUnique).WithMessage("Meeting Room Already Exists");

    //    //    //RuleFor(v => v.NameAr)
    //    //    //    .NotEmpty().WithMessage("Name in Arabic is required.")
    //    //    //    .MaximumLength(100).WithMessage("Name in Arabic must not exceed 100 characters.")
    //    //    //    .MustAsync(BeUnique).WithMessage("Meeting Room Already Exists")
    //    //    //    .MustAsync(BeInArabic).WithMessage("Name must be in Arabic");
    //    //}

    //    //public async Task<bool> BeUnique(string name, CancellationToken cancellationToken)
    //    //{
    //    //    var result = false;
    //    //    var rooms = await _meetingRoomService.GetAll();
    //    //    if (rooms != null)
    //    //    {
    //    //       result = rooms.All(l => l.NameEn != name && l.NameAr != name);
    //    //    }
    //    //    return result;
    //    //}

    //    //public async Task<bool> BeInArabic(string title, CancellationToken cancellationToken)
    //    //{
    //    //    return await title.BeInArabic();
    //    //}
    //}
}
