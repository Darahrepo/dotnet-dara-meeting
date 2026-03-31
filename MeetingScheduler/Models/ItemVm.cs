using AutoMapper;
using FluentValidation;
using MeetingScheduler.Domain.Common.Interfaces;
using MeetingScheduler.Infrastructure.Common.Extensions;
using MeetingScheduler.Infrastructure.Common.Mappings;
using MeetingScheduler.Infrastructure.Services.MeetingItems;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingScheduler.UI.Models
{
    public class ItemVm : IMapFrom<ItemDto>
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string EmailAddress { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ItemDto, ItemVm>(MemberList.Destination).ReverseMap();
        }

    }


    public class ItemVmValidator : AbstractValidator<ItemVm>
    {
        private readonly IApplicationDbContext _context;

        public ItemVmValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(v => v.NameEn)
                .NotEmpty().WithMessage("Name in English is required.")
                .MaximumLength(100).WithMessage("Name in English must not exceed 100 characters.")
                .MustAsync(BeUnique).WithMessage(" Item Already Exists");

            RuleFor(v => v.NameAr)
                .NotEmpty().WithMessage("Name in Arabic is required.")
                .MaximumLength(100).WithMessage("Name in Arabic must not exceed 100 characters.")
                .MustAsync(BeUnique).WithMessage(" Item Already Exists")
                .MustAsync(BeInArabic).WithMessage("Name must be in Arabic");
        }

        public async Task<bool> BeUnique(string name, CancellationToken cancellationToken)
        {
            return await _context.Items
                .AllAsync(l => l.NameEn != name && l.NameAr != name);
        }

        public async Task<bool> BeInArabic(string title, CancellationToken cancellationToken)
        {
            return await title.BeInArabic();
        }
    }
}
