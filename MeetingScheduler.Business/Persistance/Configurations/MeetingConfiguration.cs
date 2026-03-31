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
    public class MeetingConfiguration : IEntityTypeConfiguration<Meeting>
    {
        public void Configure(EntityTypeBuilder<Meeting> builder)
        {
            builder.Property(t => t.Subject)
            .HasMaxLength(250)
            .IsRequired();

            builder.Property(t => t.Date)
            .IsRequired();

            builder.Property(t => t.Time_From)
            .IsRequired();

            builder.Property(t => t.Time_To)
            .IsRequired();

            builder.Property(t => t.IsWebex)
            .HasDefaultValue(false);

            builder.Property(t => t.IsCeo)
           .HasDefaultValue(false);

            builder.Property(t => t.MeetingPassword)
            .HasMaxLength(11);

            builder.Property(t => t.ApprovalStatus)
           .HasConversion<char>(p => (char)p,
                p => (ApprovalStatus)(int)p)
           .HasMaxLength(1)
           .IsFixedLength(true)
           .IsUnicode(false);

            builder.Property(t => t.MeetingLocationType)
            .HasConversion<char>(p => (char)p,
                p => (LocationType)(int)p)
            .HasMaxLength(1)
            .IsFixedLength(true)
            .IsUnicode(false);

            builder.Property(t => t.ZoomAccount)
            .HasConversion<char>(p => (char)p,
                p => (ZoomUserType)(int)p)
            .HasMaxLength(1)
            .HasDefaultValue(ZoomUserType.Main)
            .IsFixedLength(true)
            .IsUnicode(false);

        }
    }
}
