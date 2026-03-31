using MeetingScheduler.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingScheduler.Infrastructure.Persistance.Configurations
{
    public class MeetingAttachmentConfiguration : IEntityTypeConfiguration<MeetingAttachment>
    {
        public void Configure(EntityTypeBuilder<MeetingAttachment> builder)
        {
            builder.Property(t => t.FilePath)
            .IsRequired();

        }
    }
}
