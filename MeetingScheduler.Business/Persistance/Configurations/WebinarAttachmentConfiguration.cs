using MeetingScheduler.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingScheduler.Infrastructure.Persistance.Configurations
{
    public class WebinarAttachmentConfiguration : IEntityTypeConfiguration<WebinarAttachment>
    {
        public void Configure(EntityTypeBuilder<WebinarAttachment> builder)
        {
            builder.Property(t => t.WebinarId)
            .HasMaxLength(100)
            .IsRequired();

            builder.Property(t => t.FileName)
            .IsRequired();

            builder.Property(t => t.FilePath)
            .IsRequired();
        }
    }
}
