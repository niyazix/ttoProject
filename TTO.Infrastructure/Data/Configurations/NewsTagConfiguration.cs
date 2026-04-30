using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TTO.Core.Entities;

namespace TTO.Infrastructure.Data.Configurations;

public class NewsTagConfiguration : IEntityTypeConfiguration<NewsTag>
{
    public void Configure(EntityTypeBuilder<NewsTag> builder)
    {
        builder.HasKey(nt => new { nt.NewsId, nt.TagId });

        builder.HasOne(nt => nt.News)
            .WithMany(n => n.NewsTags)
            .HasForeignKey(nt => nt.NewsId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(nt => nt.Tag)
            .WithMany(t => t.NewsTags)
            .HasForeignKey(nt => nt.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
