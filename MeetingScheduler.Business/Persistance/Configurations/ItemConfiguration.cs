using MeetingScheduler.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingScheduler.Infrastructure.Persistance.Configurations
{
    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.Property(t => t.NameEn)
            .HasMaxLength(200)
            .IsRequired();

            builder.Property(t => t.NameAr)
            .HasMaxLength(200)
            .IsRequired();
        }
    }
}
