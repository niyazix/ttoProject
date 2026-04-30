import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../../../../core/services/api.service';

interface NewsForm {
  title: string; slug: string; summary: string; content: string;
  coverImageUrl: string; categoryId: number | null; author: string;
  publishDate: string; isFeatured: boolean; tags: string;
}

@Component({
  selector: 'app-news-manager',
  standalone: false,
  templateUrl: './news-manager.component.html',
  styleUrl: './news-manager.component.scss'
})
export class NewsManagerComponent implements OnInit {
  items:         any[] = [];
  categories:    any[] = [];
  total          = 0; page = 1; pageSize = 15; loading = false;
  confirmDelete: any   = null;
  showForm       = false; saving = false; editItem: any = null;
  activeTab: 'edit' | 'preview' = 'edit';
  form: NewsForm = this.emptyForm();

  get totalPages() { return Math.max(1, Math.ceil(this.total / this.pageSize)); }

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.load();
    this.api.get<any[]>('categories').subscribe({ next: r => this.categories = r, error: () => {} });
  }

  load() {
    this.loading = true;
    this.api.get<any>('news/admin/all', { page: this.page, pageSize: this.pageSize }).subscribe({
      next: r => { this.items = r.items ?? []; this.total = r.total ?? 0; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  goPage(p: number) { this.page = p; this.load(); }

  openCreate() { this.editItem = null; this.form = this.emptyForm(); this.activeTab = 'edit'; this.showForm = true; }

  openEdit(n: any) {
    this.editItem = n;
    this.form = {
      title: n.title ?? '', slug: n.slug ?? '', summary: n.summary ?? '',
      content: n.content ?? '', coverImageUrl: n.coverImageUrl ?? '',
      categoryId: n.categoryId ?? null, author: n.author ?? '',
      publishDate: n.publishDate ? new Date(n.publishDate).toISOString().slice(0, 16) : '',
      isFeatured: n.isFeatured ?? false,
      tags: (n.tags ?? []).join(', ')
    };
    this.activeTab = 'edit'; this.showForm = true;
  }

  save(andPublish = false) {
    if (!this.form.title.trim()) return;
    this.saving = true;
    const payload = {
      ...this.form,
      tags: this.form.tags.split(',').map((t: string) => t.trim()).filter(Boolean),
      publishDate: this.form.publishDate || null,
      categoryId: this.form.categoryId || null,
    };
    const req = this.editItem ? this.api.put(`news/${this.editItem.id}`, payload) : this.api.post('news', payload);
    req.subscribe({
      next: (res: any) => {
        if (andPublish) {
          const id = this.editItem?.id ?? res?.id;
          if (id) this.api.patch(`news/${id}/publish`).subscribe({ next: () => this.afterSave() });
          else this.afterSave();
        } else { this.afterSave(); }
      },
      error: () => { this.saving = false; }
    });
  }

  private afterSave() { this.saving = false; this.showForm = false; this.load(); }

  togglePublish(n: any) {
    this.api.patch(`news/${n.id}/publish`).subscribe({ next: (r: any) => n.isPublished = r.isPublished });
  }

  delete(n: any) { this.confirmDelete = n; }
  doDelete() {
    this.api.delete(`news/${this.confirmDelete.id}`).subscribe({
      next: () => { this.items = this.items.filter(i => i.id !== this.confirmDelete.id); this.total--; this.confirmDelete = null; }
    });
  }

  onTitleChange() { if (!this.editItem) this.form.slug = this.toSlug(this.form.title); }

  toSlug(t: string) {
    return t.toLowerCase()
      .replace(/ş/g,'s').replace(/ç/g,'c').replace(/ğ/g,'g')
      .replace(/ü/g,'u').replace(/ö/g,'o').replace(/ı/g,'i').replace(/İ/g,'i')
      .replace(/[^a-z0-9]+/g,'-').replace(/^-|-$/g,'');
  }

  insertTag(tag: string) {
    const ta = document.getElementById('news-content') as HTMLTextAreaElement;
    if (!ta) return;
    const s = ta.selectionStart, e = ta.selectionEnd;
    const sel = this.form.content.substring(s, e);
    const wrapped = `<${tag}>${sel || 'metin'}</${tag}>`;
    this.form.content = this.form.content.substring(0, s) + wrapped + this.form.content.substring(e);
    setTimeout(() => { ta.focus(); ta.setSelectionRange(s + tag.length + 2, s + wrapped.length - tag.length - 3); });
  }

  formatDate(d: string) {
    return new Date(d).toLocaleDateString('tr-TR', { day: 'numeric', month: 'short', year: 'numeric' });
  }

  private emptyForm(): NewsForm {
    return { title: '', slug: '', summary: '', content: '', coverImageUrl: '', categoryId: null, author: '', publishDate: '', isFeatured: false, tags: '' };
  }
}
