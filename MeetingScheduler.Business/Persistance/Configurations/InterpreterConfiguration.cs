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
    public class InterpreterConfiguration : IEntityTypeConfiguration<Interpreter>
    {
        public void Configure(EntityTypeBuilder<Interpreter> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(t => t.EmailAddress)
            .HasMaxLength(250)
            .IsRequired();

            builder
            .HasOne(m => m.FromLanguage)
            .WithMany()
            .HasForeignKey(m => m.FromLanguageId)
            .OnDelete(DeleteBehavior.NoAction);


            builder
            .HasOne(m => m.ToLanguage)
            .WithMany()
            .HasForeignKey(m => m.ToLanguageId)
            .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
