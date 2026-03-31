using MeetingScheduler.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MeetingScheduler.Domain.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Item> Items { get; set; }
        DbSet<Meeting> Meetings { get; set; }
        DbSet<Webinar> Webinars { get; set; }
        DbSet<Employee> Employees { get; set; }
        DbSet<Panelist> Panelists { get; set; }
        DbSet<SampleUser> SampleUsers { get; set; }
        DbSet<MeetingRoom> MeetingRooms { get; set; }
        DbSet<MeetingItem> MeetingItems { get; set; }
        DbSet<MeetingAttendee> MeetingAttendees { get; set; }
        DbSet<MeetingAttachment> MeetingAttachments { get; set; }
        DbSet<WebinarAttachment> WebinarAttachments { get; set; }
        DbSet<WebinarRequirement> WebinarRequirements { get; set; }
        DbSet<Interpreter> Interpreters { get; set; }
        DbSet<Language> Languages { get; set; }
        DbSet<Role> Roles { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
