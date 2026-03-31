using FluentValidation;
using MeetingScheduler.Domain.Common.Interfaces;
using MeetingScheduler.Infrastructure.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingScheduler.UI.Models
{
    public class ExternalMeetingAttendeesVm
    {
        public int Id { get; set; }
        public int MeetingId { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string EmailAddress { get; set; }
    }
    public class ExternalMeetingAttendeesVmValidator : AbstractValidator<ExternalMeetingAttendeesVm>
    {
        private readonly IApplicationDbContext _context;
        private readonly CancellationToken  _cancellation;
        //private readonly IMeetingRoomService _meetingRoomService;

        public ExternalMeetingAttendeesVmValidator(IApplicationDbContext context)
        {

            _context = context;

            RuleFor(v => v.NameEn)
               .MustAsync(BeInArabic).WithMessage("Must be in arabic")
               .NotEmpty().WithMessage("Date is required.");

        }


        public async Task<bool> BeInArabic(string title, CancellationToken cancellation)
        {
            return await title.BeInArabic();
        }
    }

}
