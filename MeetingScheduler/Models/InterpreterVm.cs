using AutoMapper;
using FluentValidation;
using MeetingScheduler.Domain.Common.Interfaces;
using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Infrastructure.Common.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingScheduler.UI.Models
{
    public class InterpreterVm :IMapFrom<Interpreter>
    {
        public int Id { get; set; }
        public int WebinarId { get; set; }
        public WebinarVm Webinar { get; set; }
        public string EmailAddress { get; set; }
        public int FromLanguageId { get; set; }
        public LanguageVm FromLanguage { get; set; }
        public int ToLanguageId { get; set; }
        public LanguageVm ToLanguage { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<InterpreterVm, Interpreter>().ReverseMap();
        }

        public class WebinarVmValidator : AbstractValidator<WebinarVm>
        {
            private readonly IApplicationDbContext _context;
            public WebinarVmValidator(IApplicationDbContext context)
            {
                _context = context;

               
            }
        }
    }
}
