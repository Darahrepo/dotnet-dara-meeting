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
    public class WebinarVm : ToastVm, IMapFrom<Webinar>
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public int HostId { get; set; }
        public Employee Host { get; set; }
        public List<PanelistVm> WebinarPanelists { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time_From { get; set; }
        public TimeSpan Time_To { get; set; }
        public List<WebinarAttachmentVm> WebinarAttachments { get; set; }
        public List<WebinarRequirementVm> WebinarRequirements { get; set; }
        public string WebinarUrl { get; set; }
        public string RegistrationUrl { get; set; }
        public string Agenda { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
        public string Reason { get; set; }
        public string ZoomMeetingId { get; set; }
        public string ZoomWebinarId { get; set; }
        public string ZoomWebinarPassword { get; set; }
        public bool IsRecorded { get; set; }
        public bool IsAttachmentShared { get; set; }
        public bool IsTranslationNeeded { get; set; }
        public bool IsCertificateProvided { get; set; }
        public bool IsRegistrationNeeded { get; set; }
        public List<InterpreterVm> Interpreters { get; set; }
        public ZoomUserType ZoomAccount{ get; set; }



        //Not Mapped
        public List<IFormFile> Files { get; set; }
        public char Location { get; set; }
        public List<MeetingRoomVm> MeetingRoomslist { get; set; }
        public List<PanelistVm> ExternalWebinarPanelists { get; set; }
        public List<EmployeeVm> InternalWebinarPanelists { get; set; }
        public List<WebinarRequirementVm> WebinarRequirementsLists { get; set; }
        public List<int> InternalWebinarPanelistIds { get; set; }
        public List<EmployeeVm> EmployeesList { get; set; }
        public List<EmployeeVm> HostList { get; set; }
        public List<LanguageVm> LanguagesList { get; set; } = new List<LanguageVm>();
        public void Mapping(Profile profile)
        {
            profile.CreateMap<WebinarVm, Webinar>().ReverseMap();
            profile.CreateMap<Webinar, WebinarVm>().ForMember(x => x.Location, opt => opt.Ignore());
            profile.CreateMap<Webinar, WebinarVm>().ForMember(x => x.Files, opt => opt.Ignore());
            profile.CreateMap<Webinar, WebinarVm>().ForMember(x => x.ExternalWebinarPanelists, opt => opt.Ignore());
            profile.CreateMap<Webinar, WebinarVm>().ForMember(x => x.InternalWebinarPanelists, opt => opt.Ignore());
            profile.CreateMap<Webinar, WebinarVm>().ForMember(x => x.WebinarRequirementsLists, opt => opt.Ignore());
            profile.CreateMap<Webinar, WebinarVm>().ForMember(x => x.InternalWebinarPanelistIds, opt => opt.Ignore());
            profile.CreateMap<Webinar, WebinarVm>().ForMember(x => x.MeetingRoomslist, opt => opt.Ignore());
            profile.CreateMap<Webinar, WebinarVm>().ForMember(x => x.EmployeesList, opt => opt.Ignore());
            profile.CreateMap<Webinar, WebinarVm>().ForMember(x => x.HostList, opt => opt.Ignore());
            profile.CreateMap<Webinar, WebinarVm>().ForMember(x => x.LanguagesList, opt => opt.Ignore());
        }
    }


    public class WebinarVmValidator : AbstractValidator<WebinarVm>
    {
        private readonly IApplicationDbContext _context;
        public WebinarVmValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(v => v.Date)
               .NotEmpty().WithMessage("Date is required.");

            RuleFor(v => v.Time_From)
            .NotEmpty().WithMessage("From Time is required.");

            RuleFor(v => v.Time_To)
            .NotEmpty().WithMessage("To Time is required.");

            RuleFor(v => v.HostId)
            .NotEmpty().WithMessage("Host is required.");

            RuleFor(v => v.Subject)
            .NotEmpty().WithMessage("Subject is required.");
        }

        public async Task<bool> BeInArabic(string title, CancellationToken cancellationToken)
        {
            return await title.BeInArabic();
        }
    }
}
