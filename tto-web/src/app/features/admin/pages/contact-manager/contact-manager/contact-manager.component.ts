import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../../../../core/services/api.service';

@Component({
  selector: 'app-contact-manager',
  standalone: false,
  templateUrl: './contact-manager.component.html',
  styleUrl: './contact-manager.component.scss'
})
export class ContactManagerComponent implements OnInit {
  items:         any[] = [];
  total          = 0;
  page           = 1;
  pageSize       = 15;
  loading        = false;
  unreadOnly     = false;
  selected:      any   = null;
  confirmDelete: any   = null;

  get totalPages() { return Math.max(1, Math.ceil(this.total / this.pageSize)); }

  constructor(private api: ApiService) {}
  ngOnInit() { this.load(); }

  load() {
    this.loading = true;
    const params: any = { page: this.page, pageSize: this.pageSize };
    if (this.unreadOnly) params['unreadOnly'] = true;
    this.api.get<any>('contact', params).subscribe({
      next: r => { this.items = r.items ?? []; this.total = r.total ?? 0; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  goPage(p: number) { this.page = p; this.load(); }

  open(m: any) {
    this.selected = m;
    if (!m.isRead) {
      this.api.get(`contact/${m.id}`).subscribe({ next: () => m.isRead = true });
    }
  }

  markReplied(m: any) {
    this.api.patch(`contact/${m.id}/replied`).subscribe({ next: () => m.isReplied = true });
  }

  delete(m: any) { this.confirmDelete = m; }
  doDelete() {
    this.api.delete(`contact/${this.confirmDelete.id}`).subscribe({
      next: () => {
        this.items = this.items.filter(i => i.id !== this.confirmDelete.id);
        this.total--;
        if (this.selected?.id === this.confirmDelete.id) this.selected = null;
        this.confirmDelete = null;
      }
    });
  }

  formatDate(d: string) {
    return new Date(d).toLocaleDateString('tr-TR', { day: 'numeric', month: 'long', year: 'numeric', hour: '2-digit', minute: '2-digit' });
  }
}
