using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TTO.Core.Entities;

namespace TTO.Infrastructure.Data.Configurations;

public class AnnouncementConfiguration : IEntityTypeConfiguration<Announcement>
{
    public void Configure(EntityTypeBuilder<Announcement> builder)
    {
        builder.HasIndex(a => a.Slug).IsUnique();
        builder.HasIndex(a => new { a.IsPublished, a.DeletedAt });

        builder.Property(a => a.Title).HasMaxLength(500).IsRequired();
        builder.Property(a => a.Slug).HasMaxLength(500).IsRequired();
        builder.Property(a => a.Summary).HasMaxLength(1000);
        builder.Property(a => a.CoverImageUrl).HasMaxLength(1000);

        builder.HasOne(a => a.Category)
            .WithMany(c => c.Announcements)
            .HasForeignKey(a => a.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(a => a.DeletedAt == null);
    }
}
