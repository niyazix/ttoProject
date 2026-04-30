import { Component, HostListener, OnInit } from '@angular/core';
import { ApiService } from '../../../../core/services/api.service';

interface MenuItem {
  id: number;
  title: string;
  linkType: string;
  pageId?: number;
  url?: string;
  level: number;
  sortOrder: number;
  openInNewTab: boolean;
  icon?: string;
  children: MenuItem[];
}

@Component({
  selector: 'app-public-layout',
  standalone: false,
  templateUrl: './public-layout.component.html',
  styleUrl: './public-layout.component.scss'
})
export class PublicLayoutComponent implements OnInit {
  menuItems: MenuItem[] = [];
  mobileMenuOpen = false;
  scrolled = false;
  year = new Date().getFullYear();

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.api.get<MenuItem[]>('menu/main').subscribe({
      next: items => this.menuItems = items?.length ? items : this.fallbackMenu(),
      error: () => this.menuItems = this.fallbackMenu()
    });
  }

  getLink(item: MenuItem): string {
    const type = item.linkType?.toLowerCase();
    if (type === 'none' || !item.url) return '#';
    return item.url;
  }

  isExternal(url?: string | null): boolean {
    return !!url && /^https?:\/\//.test(url);
  }

  routerLinkFor(item: MenuItem): string | null {
    const url = this.getLink(item);
    if (url === '' || this.isExternal(url)) return null;
    return url;
  }

  @HostListener('window:scroll')
  onScroll() {
    this.scrolled = window.scrollY > 40;
  }

  toggleMobile() { this.mobileMenuOpen = !this.mobileMenuOpen; }
  closeMobile()  { this.mobileMenuOpen = false; }

  private fallbackMenu(): MenuItem[] {
    return [
      { id: 1, title: 'Anasayfa',  linkType: 'url', url: '/',          level: 0, sortOrder: 1, openInNewTab: false, children: [] },
      { id: 2, title: 'Duyurular', linkType: 'url', url: '/duyurular', level: 0, sortOrder: 2, openInNewTab: false, children: [] },
      { id: 3, title: 'Haberler',  linkType: 'url', url: '/haberler',  level: 0, sortOrder: 3, openInNewTab: false, children: [] },
      { id: 4, title: 'İletişim',  linkType: 'url', url: '/iletisim',  level: 0, sortOrder: 4, openInNewTab: false, children: [] },
    ];
  }
}
