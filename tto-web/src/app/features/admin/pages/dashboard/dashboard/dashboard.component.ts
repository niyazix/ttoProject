import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../../../../core/services/api.service';
import { AuthService } from '../../../../../core/services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: false,
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {

  today = new Date().toLocaleDateString('tr-TR', { weekday: 'long', day: 'numeric', month: 'long', year: 'numeric' });

  stats = [
    { icon: 'fa-newspaper',  label: 'Haber',    value: 0, color: '#2856a8', bg: 'rgba(40,86,168,.12)',  route: '/admin/news'          },
    { icon: 'fa-bullhorn',   label: 'Duyuru',   value: 0, color: '#d97706', bg: 'rgba(217,119,6,.12)',  route: '/admin/announcements' },
    { icon: 'fa-file-lines', label: 'Sayfa',    value: 0, color: '#16a34a', bg: 'rgba(22,163,74,.12)',  route: '/admin/pages'         },
    { icon: 'fa-envelope',   label: 'Mesaj',    value: 0, color: '#0284c7', bg: 'rgba(2,132,199,.12)',  route: '/admin/contact'       },
  ];

  quickActions = [
    { icon: 'fa-plus',            label: 'Yeni Haber',     route: '/admin/news',          color: '#2856a8' },
    { icon: 'fa-bullhorn',        label: 'Yeni Duyuru',    route: '/admin/announcements', color: '#d97706' },
    { icon: 'fa-file-circle-plus',label: 'Yeni Sayfa',     route: '/admin/pages',         color: '#16a34a' },
    { icon: 'fa-bars',            label: 'Menü Düzenle',   route: '/admin/menu',          color: '#7c3aed' },
    { icon: 'fa-users',           label: 'Kullanıcılar',   route: '/admin/users',         color: '#0891b2' },
    { icon: 'fa-gear',            label: 'Ayarlar',        route: '/admin/settings',      color: '#64748b' },
  ];

  recentNews:          any[] = [];
  recentAnnouncements: any[] = [];
  unreadMsgs:          any[] = [];

  sysInfo = [
    { key: 'Ortam',    val: 'Development',        cls: 'badge badge-warning' },
    { key: 'API',      val: 'http://localhost:5000', cls: '' },
    { key: 'Frontend', val: 'Angular 19',          cls: '' },
    { key: 'Backend',  val: '.NET Core 9',         cls: '' },
    { key: 'DB',       val: 'MSSQL — TTODb',       cls: '' },
  ];

  constructor(private api: ApiService, public auth: AuthService) {}

  ngOnInit() {
    this.api.get<any>('news/admin/all', { pageSize: 5 }).subscribe({
      next: r => { this.stats[0].value = r.total ?? 0; this.recentNews = r.items ?? []; }
    });
    this.api.get<any>('announcements/admin/all', { pageSize: 5 }).subscribe({
      next: r => { this.stats[1].value = r.total ?? 0; this.recentAnnouncements = r.items ?? []; }
    });
    this.api.get<any[]>('pages/admin/all').subscribe({
      next: r => this.stats[2].value = r.length
    });
    this.api.get<any>('contact', { pageSize: 5, unreadOnly: true }).subscribe({
      next: r => { this.stats[3].value = r.total ?? 0; this.unreadMsgs = r.items ?? []; }
    });
  }

  formatDate(d: string) {
    return new Date(d).toLocaleDateString('tr-TR', { day: 'numeric', month: 'short', year: 'numeric' });
  }
}
