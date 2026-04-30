import { Component, computed } from '@angular/core';
import { AuthService } from '../../../../core/services/auth.service';

interface NavItem {
  label: string;
  icon:  string;
  route: string;
  permission?: string;
}

@Component({
  selector: 'app-admin-layout',
  standalone: false,
  templateUrl: './admin-layout.component.html',
  styleUrl: './admin-layout.component.scss'
})
export class AdminLayoutComponent {
  sidebarCollapsed = false;

  readonly navItems: NavItem[] = [
    { label: 'Dashboard',    icon: 'fa-gauge',        route: '/admin'               },
    { label: 'Sayfalar',     icon: 'fa-file-lines',   route: '/admin/pages',        permission: 'pages.view'         },
    { label: 'Haberler',     icon: 'fa-newspaper',    route: '/admin/news',         permission: 'news.view'          },
    { label: 'Duyurular',    icon: 'fa-bullhorn',     route: '/admin/announcements',permission: 'announcements.view' },
    { label: 'İletişim',     icon: 'fa-envelope',     route: '/admin/contact',      permission: 'contact.view'       },
    { label: 'Menü',         icon: 'fa-bars',         route: '/admin/menu',         permission: 'menu.manage'        },
    { label: 'Kullanıcılar', icon: 'fa-users',        route: '/admin/users',        permission: 'users.manage'       },
    { label: 'Roller',       icon: 'fa-shield-halved',route: '/admin/roles',        permission: 'roles.manage'       },
    { label: 'Ayarlar',      icon: 'fa-gear',         route: '/admin/settings',     permission: 'settings.edit'      },
  ];

  visibleItems = computed(() => {
    return this.navItems.filter(item =>
      !item.permission || this.auth.hasPermission(item.permission)
    );
  });

  constructor(public auth: AuthService) {}

  get userInitials(): string {
    const u = this.auth.currentUser();
    if (!u) return '?';
    return (u.firstName?.charAt(0) ?? '') + (u.lastName?.charAt(0) ?? '');
  }

  logout() { this.auth.logout(); }
  toggleSidebar() { this.sidebarCollapsed = !this.sidebarCollapsed; }
}
