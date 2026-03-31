using MeetingScheduler.Domain.Common;
using MeetingScheduler.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using MeetingScheduler.Infrastructure.Identity;
using MeetingScheduler.Identity;
using Microsoft.AspNetCore.Http;
using System;
using MeetingScheduler.Domain.Common.Interfaces;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.Extensions.Options;


namespace MeetingScheduler.Infrastructure.Persistance
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>, IApplicationDbContext
    {
        private readonly IUserProvider _currentUser;
        private readonly IDateTimeService _dateTime;
        private readonly IHttpContextAccessor _httpAccessor;
        public ApplicationDbContext(
            IDateTimeService dateTime,
            DbContextOptions options,
            IHttpContextAccessor httpAccessor,
            IOptions<OperationalStoreOptions> operationalStoreOptions,
            IUserProvider currentUser
            ) : base(options, operationalStoreOptions)
        {
            _dateTime = dateTime;
            _currentUser = currentUser;
            _httpAccessor = httpAccessor;
        }


        public DbSet<MeetingRoom> MeetingRooms { get; set; }
        public DbSet<SampleUser> SampleUsers { get; set; }
        public DbSet<Employee> Employees{ get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<MeetingItem> MeetingItems { get; set; }
        public DbSet<MeetingAttachment> MeetingAttachments { get; set; }
        public DbSet<MeetingAttendee> MeetingAttendees{ get; set; }
        public DbSet<Webinar> Webinars { get; set; }
        public DbSet<Panelist> Panelists { get; set; }
        public DbSet<WebinarRequirement> WebinarRequirements { get; set; }
        public DbSet<WebinarAttachment> WebinarAttachments { get; set; }
        public DbSet<Interpreter> Interpreters { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Role> Roles { get; set; }


        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {

            foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<AuditableEntity> entry in ChangeTracker.Entries<AuditableEntity>())
            {
                var UserId = _currentUser.CurrentUser.UserId;
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = Convert.ToInt32(UserId) > 0 ? UserId.ToString() : "S";
                        entry.Entity.CreatedOn = _dateTime.Now;
                        entry.Entity.IsActive = true;
                        break;

                    case EntityState.Modified:
                        entry.Entity.ModifiedBy = Convert.ToInt32(UserId) > 0 ? UserId.ToString() : "S";
                        entry.Entity.ModifiedOn = _dateTime.Now;
                        break;
                }
            }

            var result = await base.SaveChangesAsync(cancellationToken);

            //await DispatchEvents();

            return result;

            // Call the original SaveChanges(), which will save both the changes made and the audit records
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }


    }

}
