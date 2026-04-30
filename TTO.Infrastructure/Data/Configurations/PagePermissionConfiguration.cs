using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TTO.Core.Entities;

namespace TTO.Infrastructure.Data.Configurations;

public class PagePermissionConfiguration : IEntityTypeConfiguration<PagePermission>
{
    public void Configure(EntityTypeBuilder<PagePermission> builder)
    {
        builder.HasIndex(pp => new { pp.PageId, pp.UserId, pp.RoleId });

        // Ya UserId ya RoleId dolu olmalı — check constraint
        builder.ToTable(t => t.HasCheckConstraint(
            "CK_PagePermissions_UserOrRole",
            "(UserId IS NOT NULL AND RoleId IS NULL) OR (UserId IS NULL AND RoleId IS NOT NULL)"
        ));

        builder.HasOne(pp => pp.Page)
            .WithMany(p => p.PagePermissions)
            .HasForeignKey(pp => pp.PageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pp => pp.User)
            .WithMany()
            .HasForeignKey(pp => pp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pp => pp.Role)
            .WithMany(r => r.PagePermissions)
            .HasForeignKey(pp => pp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(pp => pp.DeletedAt == null);
    }
}
