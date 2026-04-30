using Microsoft.EntityFrameworkCore;
using TTO.Core.Entities;
using TTO.Core.Enums;

namespace TTO.Infrastructure.Data.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await SeedRolesAsync(context);
        await SeedPermissionsAsync(context);
        await SeedAdminRolePermissionsAsync(context);
        await SeedSystemPagesAsync(context);
        await SeedSiteSettingsAsync(context);
        await SeedSuperAdminAsync(context);
        await SeedMainMenuAsync(context);
    }

    private static async Task SeedRolesAsync(AppDbContext context)
    {
        if (await context.Roles.AnyAsync()) return;

        context.Roles.AddRange(
            new Role { Name = "Admin",  Description = "Tam yetkili sistem yöneticisi", IsSystem = true  },
            new Role { Name = "Editor", Description = "İçerik düzenleyici",            IsSystem = false },
            new Role { Name = "Viewer", Description = "Salt okunur erişim",            IsSystem = false }
        );
        await context.SaveChangesAsync();
    }

    private static async Task SeedPermissionsAsync(AppDbContext context)
    {
        if (await context.Permissions.AnyAsync()) return;

        var permissions = new List<Permission>
        {
            new() { Name = "news.view",             Module = "news",          Action = "view",    Description = "Haberleri görüntüle"       },
            new() { Name = "news.create",           Module = "news",          Action = "create",  Description = "Haber oluştur"             },
            new() { Name = "news.edit",             Module = "news",          Action = "edit",    Description = "Haber düzenle"             },
            new() { Name = "news.publish",          Module = "news",          Action = "publish", Description = "Haber yayınla"             },
            new() { Name = "news.delete",           Module = "news",          Action = "delete",  Description = "Haber sil"                 },
            new() { Name = "announcements.view",    Module = "announcements", Action = "view",    Description = "Duyuruları görüntüle"      },
            new() { Name = "announcements.create",  Module = "announcements", Action = "create",  Description = "Duyuru oluştur"            },
            new() { Name = "announcements.edit",    Module = "announcements", Action = "edit",    Description = "Duyuru düzenle"            },
            new() { Name = "announcements.publish", Module = "announcements", Action = "publish", Description = "Duyuru yayınla"            },
            new() { Name = "announcements.delete",  Module = "announcements", Action = "delete",  Description = "Duyuru sil"                },
            new() { Name = "pages.view",            Module = "pages",         Action = "view",    Description = "Sayfaları görüntüle"       },
            new() { Name = "pages.create",          Module = "pages",         Action = "create",  Description = "Sayfa oluştur"             },
            new() { Name = "pages.edit",            Module = "pages",         Action = "edit",    Description = "Sayfa düzenle"             },
            new() { Name = "pages.publish",         Module = "pages",         Action = "publish", Description = "Sayfa yayınla"             },
            new() { Name = "pages.delete",          Module = "pages",         Action = "delete",  Description = "Sayfa sil"                 },
            new() { Name = "media.view",            Module = "media",         Action = "view",    Description = "Medyayı görüntüle"         },
            new() { Name = "media.upload",          Module = "media",         Action = "create",  Description = "Dosya yükle"               },
            new() { Name = "media.delete",          Module = "media",         Action = "delete",  Description = "Dosya sil"                 },
            new() { Name = "contact.view",          Module = "contact",       Action = "view",    Description = "Mesajları görüntüle"       },
            new() { Name = "contact.delete",        Module = "contact",       Action = "delete",  Description = "Mesaj sil"                 },
            new() { Name = "users.manage",          Module = "users",         Action = "manage",  Description = "Kullanıcı yönetimi"        },
            new() { Name = "roles.manage",          Module = "roles",         Action = "manage",  Description = "Rol yönetimi"              },
            new() { Name = "settings.edit",         Module = "settings",      Action = "edit",    Description = "Site ayarlarını düzenle"   },
            new() { Name = "menu.manage",           Module = "menu",          Action = "manage",  Description = "Menü yönetimi"             },
        };

        context.Permissions.AddRange(permissions);
        await context.SaveChangesAsync();
    }

    private static async Task SeedAdminRolePermissionsAsync(AppDbContext context)
    {
        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
        if (adminRole == null) return;

        var hasPermissions = await context.RolePermissions.AnyAsync(rp => rp.RoleId == adminRole.Id);
        if (hasPermissions) return;

        var allPermissions = await context.Permissions.ToListAsync();
        context.RolePermissions.AddRange(allPermissions.Select(p => new RolePermission
        {
            RoleId       = adminRole.Id,
            PermissionId = p.Id
        }));
        await context.SaveChangesAsync();
    }

    private static async Task SeedSystemPagesAsync(AppDbContext context)
    {
        if (await context.Pages.AnyAsync()) return;

        context.Pages.AddRange(
            new Page { Title = "Anasayfa",  Slug = "",          IsPublished = true, IsSystem = true, PageType = PageType.Home,          SortOrder = 1 },
            new Page { Title = "Duyurular", Slug = "duyurular", IsPublished = true, IsSystem = true, PageType = PageType.Announcements, SortOrder = 2 },
            new Page { Title = "Haberler",  Slug = "haberler",  IsPublished = true, IsSystem = true, PageType = PageType.News,           SortOrder = 3 },
            new Page { Title = "İletişim",  Slug = "iletisim",  IsPublished = true, IsSystem = true, PageType = PageType.Contact,        SortOrder = 4 }
        );
        await context.SaveChangesAsync();
    }

    private static async Task SeedSiteSettingsAsync(AppDbContext context)
    {
        if (await context.SiteSettings.AnyAsync()) return;

        context.SiteSettings.AddRange(
            new SiteSetting { SettingKey = "site_title",      SettingValue = "Teknoloji Transfer Ofisi",          SettingGroup = "general", IsPublic = true  },
            new SiteSetting { SettingKey = "site_logo_url",   SettingValue = "/assets/logo.png",                  SettingGroup = "general", IsPublic = true  },
            new SiteSetting { SettingKey = "contact_email",   SettingValue = "info@tto.edu.tr",                   SettingGroup = "contact", IsPublic = true  },
            new SiteSetting { SettingKey = "contact_phone",   SettingValue = "+90 000 000 00 00",                 SettingGroup = "contact", IsPublic = true  },
            new SiteSetting { SettingKey = "contact_address", SettingValue = "Üniversite Mah.",                   SettingGroup = "contact", IsPublic = true  },
            new SiteSetting { SettingKey = "footer_text",     SettingValue = "© 2025 TTO. Tüm hakları saklıdır.", SettingGroup = "general", IsPublic = true  },
            new SiteSetting { SettingKey = "social_twitter",  SettingValue = "",                                  SettingGroup = "social",  IsPublic = true  },
            new SiteSetting { SettingKey = "social_linkedin", SettingValue = "",                                  SettingGroup = "social",  IsPublic = true  }
        );
        await context.SaveChangesAsync();
    }

    private static async Task SeedMainMenuAsync(AppDbContext context)
    {
        if (await context.MenuItems.AnyAsync()) return;

        context.MenuItems.AddRange(
            new MenuItem { Title = "Anasayfa",  LinkType = LinkType.Url, Url = "/",          SortOrder = 1, MenuGroup = "main", IsActive = true },
            new MenuItem { Title = "Haberler",  LinkType = LinkType.Url, Url = "/haberler",  SortOrder = 2, MenuGroup = "main", IsActive = true },
            new MenuItem { Title = "Duyurular", LinkType = LinkType.Url, Url = "/duyurular", SortOrder = 3, MenuGroup = "main", IsActive = true },
            new MenuItem { Title = "İletişim",  LinkType = LinkType.Url, Url = "/iletisim",  SortOrder = 4, MenuGroup = "main", IsActive = true }
        );
        await context.SaveChangesAsync();
    }

    private static async Task SeedSuperAdminAsync(AppDbContext context)
    {
        const string superAdminEmail = "admin@tto.edu.tr";

        if (await context.Users.AnyAsync(u => u.Email == superAdminEmail)) return;

        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
        if (adminRole == null) return;

        var superAdmin = new User
        {
            Email        = superAdminEmail,
            Username     = "superadmin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            FirstName    = "Süper",
            LastName     = "Admin",
            IsActive     = true
        };

        context.Users.Add(superAdmin);
        await context.SaveChangesAsync();

        context.UserRoles.Add(new UserRole
        {
            UserId     = superAdmin.Id,
            RoleId     = adminRole.Id,
            AssignedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();
    }
}
