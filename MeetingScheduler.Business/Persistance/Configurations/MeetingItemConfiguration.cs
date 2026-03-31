using MeetingScheduler.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingScheduler.Infrastructure.Persistance.Configurations
{
    public class MeetingItemConfiguration : IEntityTypeConfiguration<MeetingItem>
    {
        public void Configure(EntityTypeBuilder<MeetingItem> builder)
        {
            builder.Property(t => t.ItemName)
            .HasMaxLength(100)
            .IsRequired();
        }
    }
}
