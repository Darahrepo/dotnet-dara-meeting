using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Infrastructure.Persistance.Configurations
{
    public class MeetingAttendeeConfiguration : IEntityTypeConfiguration<MeetingAttendee>
    {
        public void Configure(EntityTypeBuilder<MeetingAttendee> builder)
        {
            builder.Property(t => t.ExternalAttendeeEmailAddress)
            .HasMaxLength(100);

            builder.Property(t => t.ExternalAttendeeNameEn)
            .HasMaxLength(100);

            builder.Property(t => t.ExternalAttendeeNameAr)
            .HasMaxLength(100);

            builder.Property(t => t.AttendeeType)
            .HasConversion<char>(p => (char)p,
                p => (AttendeeType)(int)p)
            .HasMaxLength(1)
            .IsFixedLength(true)
            .IsUnicode(false);
        }
    }
}
