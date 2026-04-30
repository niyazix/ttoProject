using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TTO.Core.Entities;

namespace TTO.Infrastructure.Data.Configurations;

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.HasIndex(m => new { m.MenuGroup, m.DeletedAt });

        builder.Property(m => m.Title).HasMaxLength(200).IsRequired();
        builder.Property(m => m.Url).HasMaxLength(500);
        builder.Property(m => m.Icon).HasMaxLength(100);
        builder.Property(m => m.MenuGroup).HasMaxLength(50).IsRequired();
        builder.Property(m => m.LinkType).HasConversion<string>().HasMaxLength(20);

        builder.HasOne(m => m.Page)
            .WithMany(p => p.MenuItems)
            .HasForeignKey(m => m.PageId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(m => m.Parent)
            .WithMany(m => m.Children)
            .HasForeignKey(m => m.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(m => m.DeletedAt == null);
    }
}
