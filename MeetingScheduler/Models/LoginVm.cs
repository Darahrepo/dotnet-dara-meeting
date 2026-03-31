using FluentValidation;
using MeetingScheduler.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingScheduler.UI.Models
{
    public class LoginVm
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

     public class LoginVmValidator : AbstractValidator<LoginVm>
    {
        private readonly IApplicationDbContext _context;

        public LoginVmValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(v => v.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MaximumLength(100).WithMessage("Username must not exceed 100 characters.");

            RuleFor(v => v.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MaximumLength(100).WithMessage("Email Address must not exceed 200 characters.");
        }
    }
}
