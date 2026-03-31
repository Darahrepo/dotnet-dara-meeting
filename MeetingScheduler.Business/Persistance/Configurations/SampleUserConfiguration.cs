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
    public class MeetingRoomConfiguration : IEntityTypeConfiguration<MeetingRoom>
    {
        public void Configure(EntityTypeBuilder<MeetingRoom> builder)
        {
            builder.Property(t => t.NameEn)
            .HasMaxLength(100)
            .IsRequired();

            builder.Property(t => t.NameAr)
            .HasMaxLength(100)
            .IsRequired(); 
        }
    }
}
