import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../../../../core/services/api.service';

@Component({
  selector: 'app-announcements-list',
  standalone: false,
  templateUrl: './announcements-list.component.html',
  styleUrl: './announcements-list.component.scss'
})
export class AnnouncementsListComponent implements OnInit {
  items:   any[] = [];
  total = 0; page = 1; pageSize = 10; loading = false;

  get totalPages() { return Math.max(1, Math.ceil(this.total / this.pageSize)); }
  get pages() { return Array.from({ length: this.totalPages }, (_, i) => i + 1); }

  constructor(private api: ApiService) {}

  ngOnInit() { this.load(); }

  load() {
    this.loading = true;
    this.api.get<any>('announcements', { page: this.page, pageSize: this.pageSize }).subscribe({
      next: r => { this.items = r.items ?? []; this.total = r.total ?? 0; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  goPage(p: number) { this.page = p; this.load(); window.scrollTo({ top: 0, behavior: 'smooth' }); }

  formatDate(d: string) {
    return new Date(d).toLocaleDateString('tr-TR', { day: 'numeric', month: 'long', year: 'numeric' });
  }
}
