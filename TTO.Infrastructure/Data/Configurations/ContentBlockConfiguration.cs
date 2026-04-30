using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TTO.Core.Entities;

namespace TTO.Infrastructure.Data.Configurations;

public class ContentBlockConfiguration : IEntityTypeConfiguration<ContentBlock>
{
    public void Configure(EntityTypeBuilder<ContentBlock> builder)
    {
        builder.HasIndex(cb => new { cb.PageId, cb.DeletedAt });

        builder.Property(cb => cb.BlockType).HasMaxLength(50).IsRequired();

        builder.HasOne(cb => cb.Page)
            .WithMany(p => p.ContentBlocks)
            .HasForeignKey(cb => cb.PageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(cb => cb.DeletedAt == null);
    }
}
