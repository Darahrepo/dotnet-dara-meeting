using MeetingScheduler.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingScheduler.Infrastructure.Persistance.Configurations
{
    public class WebinarRequirementConfiguration : IEntityTypeConfiguration<WebinarRequirement>
    {
        public void Configure(EntityTypeBuilder<WebinarRequirement> builder)
        {
            builder.Property(t => t.Details)
            .HasMaxLength(100)
            .IsRequired();

            builder.Property(t => t.WebinarId)
            .IsRequired();
        }
    }
}
