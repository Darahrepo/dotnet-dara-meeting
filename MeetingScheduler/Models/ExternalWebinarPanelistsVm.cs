using FluentValidation;
using MeetingScheduler.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingScheduler.UI.Models
{
    public class ExternalWebinarPanelistsVm
    {
        public int Id { get; set; }
        public int WebinarId { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string EmailAddress { get; set; }
    }
    public class ExternalMeetingpanelistsVmValidator : AbstractValidator<ExternalWebinarPanelistsVm>
    {
        private readonly IApplicationDbContext _context;

        public ExternalMeetingpanelistsVmValidator(IApplicationDbContext context)
        {
            _context = context;
        }
    }
}
