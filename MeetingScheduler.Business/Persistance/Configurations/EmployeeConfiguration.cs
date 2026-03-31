using MeetingScheduler.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Infrastructure.Persistance.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.Property(t => t.DisplayName)
            .HasMaxLength(200)
            .IsRequired();

            builder.Property(t => t.Guid)
            .IsRequired();

            builder.Property(t => t.EmailAddress)
            .HasMaxLength(100)
            .IsRequired();

            builder.Property(t => t.FirstNameEn)
            .HasMaxLength(100)
            .IsRequired();

            builder.Property(t => t.RoleId)
            .HasDefaultValue(2);

        }
    }
}
