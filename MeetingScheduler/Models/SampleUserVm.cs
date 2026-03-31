using AutoMapper;
using FluentValidation;
using MeetingScheduler.Domain.Common.Interfaces;
using MeetingScheduler.Infrastructure.Common.Extensions;
using MeetingScheduler.Infrastructure.Common.Mappings;
using MeetingScheduler.Infrastructure.Services.SampleUsers;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingScheduler.UI.Models
{
    public class SampleUserVm : IMapFrom<SampleUserDto> 
    {

        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string EmailAddress { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<SampleUserDto , SampleUserVm>(MemberList.Destination).ReverseMap();
        }

    }


    public class SampleUserVmValidator : AbstractValidator<SampleUserVm>
    {
        private readonly IApplicationDbContext _context;

        public SampleUserVmValidator(IApplicationDbContext context)
        {
            _context = context;


            RuleFor(v => v.NameEn)
                .NotEmpty().WithMessage("Name in English is required.")
                .MustAsync(NoSpaces).WithMessage("Name in English must not be empty.")
                .MinimumLength(3).WithMessage("Name in English must be of atleast 3 characters.")
                .MaximumLength(100).WithMessage("Name in English must not exceed 100 characters.");
            
              
            RuleFor(v => v.NameAr)
                .NotEmpty().WithMessage("Name in Arabic is required.")
                .MustAsync(NoSpaces).WithMessage("Name in Arabic is required.")
                .MinimumLength(3).WithMessage("Name in Arabic must be of atleast 3 characters.")
                .MaximumLength(100).WithMessage("Name in Arabic must not exceed 100 characters.");


            RuleFor(v => v.EmailAddress)
                .NotEmpty().WithMessage("Email Address is required.")
                .MaximumLength(100).WithMessage("Email Address must not exceed 200 characters.")
                .MinimumLength(3).WithMessage("Email Address must be of atleast 3 characters.")
                .MustAsync(BeUniqueTitle).WithMessage("The specified title already exists.");
        }

        public async Task<bool> BeUniqueTitle(string value, CancellationToken cancellationToken)
        {
            return await _context.SampleUsers.AllAsync(l => l.EmailAddress != value);
        }

        public async Task<bool> NoSpaces(string value, CancellationToken cancellationToken)
        {
            if (value == null) return false;
            if (string.IsNullOrWhiteSpace(value.ToString())) return false;
            return true;
        }

        public async Task<bool> BeInArabic(string title, CancellationToken cancellationToken)
        {
            return await title.BeInArabic();
        }
    }
}
