using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TTO.Core.Entities;

namespace TTO.Infrastructure.Data.Configurations;

public class PageConfiguration : IEntityTypeConfiguration<Page>
{
    public void Configure(EntityTypeBuilder<Page> builder)
    {
        builder.HasIndex(p => p.Slug).IsUnique();
        builder.HasIndex(p => p.DeletedAt);
        builder.HasIndex(p => new { p.IsPublished, p.DeletedAt });

        builder.Property(p => p.Title).HasMaxLength(500).IsRequired();
        builder.Property(p => p.Slug).HasMaxLength(500).IsRequired();
        builder.Property(p => p.MetaTitle).HasMaxLength(500);
        builder.Property(p => p.MetaDescription).HasMaxLength(1000);
        builder.Property(p => p.PageType).HasConversion<string>().HasMaxLength(50);

        builder.HasOne(p => p.Parent)
            .WithMany(p => p.Children)
            .HasForeignKey(p => p.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(p => p.DeletedAt == null);
    }
}
