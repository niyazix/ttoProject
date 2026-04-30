using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TTO.Core.Entities;

namespace TTO.Infrastructure.Data.Configurations;

public class NewsConfiguration : IEntityTypeConfiguration<News>
{
    public void Configure(EntityTypeBuilder<News> builder)
    {
        builder.HasIndex(n => n.Slug).IsUnique();
        builder.HasIndex(n => new { n.IsPublished, n.DeletedAt });

        builder.Property(n => n.Title).HasMaxLength(500).IsRequired();
        builder.Property(n => n.Slug).HasMaxLength(500).IsRequired();
        builder.Property(n => n.Summary).HasMaxLength(1000);
        builder.Property(n => n.CoverImageUrl).HasMaxLength(1000);
        builder.Property(n => n.Author).HasMaxLength(200);

        builder.HasOne(n => n.Category)
            .WithMany(c => c.News)
            .HasForeignKey(n => n.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(n => n.DeletedAt == null);
    }
}
