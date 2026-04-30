import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../../../../core/services/api.service';

@Component({
  selector: 'app-news-list',
  standalone: false,
  templateUrl: './news-list.component.html',
  styleUrl: './news-list.component.scss'
})
export class NewsListComponent implements OnInit {
  items:    any[] = [];
  categories: any[] = [];
  total = 0; page = 1; pageSize = 12; loading = false;
  selectedCategory: number | null = null;

  get totalPages() { return Math.max(1, Math.ceil(this.total / this.pageSize)); }
  get pages() { return Array.from({ length: this.totalPages }, (_, i) => i + 1); }

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.api.get<any[]>('categories').subscribe({ next: r => this.categories = (r ?? []).filter((c: any) => c.contentType === 'news' || c.contentType === 'News'), error: () => {} });
    this.load();
  }

  load() {
    this.loading = true;
    const params: Record<string, string | number | boolean> = { page: this.page, pageSize: this.pageSize };
    if (this.selectedCategory) params['categoryId'] = this.selectedCategory;
    this.api.get<any>('news', params).subscribe({
      next: r => { this.items = r.items ?? []; this.total = r.total ?? 0; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  filterBy(catId: number | null) { this.selectedCategory = catId; this.page = 1; this.load(); }
  goPage(p: number)               { this.page = p; this.load(); window.scrollTo({ top: 0, behavior: 'smooth' }); }

  formatDate(d: string) {
    return new Date(d).toLocaleDateString('tr-TR', { day: 'numeric', month: 'long', year: 'numeric' });
  }
}
