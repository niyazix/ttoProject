import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../../../../core/services/api.service';

interface AnnForm {
  title: string; slug: string; summary: string; content: string;
  coverImageUrl: string; categoryId: number | null;
  isPinned: boolean; publishDate: string; expiryDate: string;
}

@Component({
  selector: 'app-announcements-manager',
  standalone: false,
  templateUrl: './announcements-manager.component.html',
  styleUrl: './announcements-manager.component.scss'
})
export class AnnouncementsManagerComponent implements OnInit {
  items:         any[] = [];
  categories:    any[] = [];
  total          = 0; page = 1; pageSize = 15; loading = false;
  confirmDelete: any   = null;
  showForm       = false; saving = false; editItem: any = null;
  activeTab: 'edit' | 'preview' = 'edit';
  form: AnnForm  = this.emptyForm();

  get totalPages() { return Math.max(1, Math.ceil(this.total / this.pageSize)); }

  constructor(private api: ApiService) {}
  ngOnInit() {
    this.load();
    this.api.get<any[]>('categories').subscribe({ next: r => this.categories = r, error: () => {} });
  }

  load() {
    this.loading = true;
    this.api.get<any>('announcements/admin/all', { page: this.page, pageSize: this.pageSize }).subscribe({
      next: r => { this.items = r.items ?? []; this.total = r.total ?? 0; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  goPage(p: number) { this.page = p; this.load(); }

  openCreate() { this.editItem = null; this.form = this.emptyForm(); this.activeTab = 'edit'; this.showForm = true; }

  openEdit(a: any) {
    this.editItem = a;
    this.form = {
      title: a.title ?? '', slug: a.slug ?? '', summary: a.summary ?? '',
      content: a.content ?? '', coverImageUrl: a.coverImageUrl ?? '',
      categoryId: a.categoryId ?? null, isPinned: a.isPinned ?? false,
      publishDate: a.publishDate ? new Date(a.publishDate).toISOString().slice(0, 16) : '',
      expiryDate:  a.expiryDate  ? new Date(a.expiryDate).toISOString().slice(0, 16)  : '',
    };
    this.activeTab = 'edit'; this.showForm = true;
  }

  save(andPublish = false) {
    if (!this.form.title.trim()) return;
    this.saving = true;
    const payload = {
      ...this.form,
      publishDate: this.form.publishDate || null,
      expiryDate:  this.form.expiryDate  || null,
      categoryId:  this.form.categoryId  || null,
    };
    const req = this.editItem
      ? this.api.put(`announcements/${this.editItem.id}`, payload)
      : this.api.post('announcements', payload);
    req.subscribe({
      next: (res: any) => {
        if (andPublish) {
          const id = this.editItem?.id ?? res?.id;
          if (id) this.api.patch(`announcements/${id}/publish`).subscribe({ next: () => this.afterSave() });
          else this.afterSave();
        } else { this.afterSave(); }
      },
      error: () => { this.saving = false; }
    });
  }

  private afterSave() { this.saving = false; this.showForm = false; this.load(); }

  togglePublish(a: any) {
    this.api.patch(`announcements/${a.id}/publish`).subscribe({ next: (r: any) => a.isPublished = r.isPublished });
  }

  delete(a: any) { this.confirmDelete = a; }
  doDelete() {
    this.api.delete(`announcements/${this.confirmDelete.id}`).subscribe({
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
    const ta = document.getElementById('ann-content') as HTMLTextAreaElement;
    if (!ta) return;
    const s = ta.selectionStart, e = ta.selectionEnd;
    const sel = this.form.content.substring(s, e);
    const wrapped = `<${tag}>${sel || 'metin'}</${tag}>`;
    this.form.content = this.form.content.substring(0, s) + wrapped + this.form.content.substring(e);
    setTimeout(() => ta.focus());
  }

  formatDate(d: string) {
    return new Date(d).toLocaleDateString('tr-TR', { day: 'numeric', month: 'short', year: 'numeric' });
  }

  isExpired(d: string) { return new Date(d) < new Date(); }

  private emptyForm(): AnnForm {
    return { title: '', slug: '', summary: '', content: '', coverImageUrl: '', categoryId: null, isPinned: false, publishDate: '', expiryDate: '' };
  }
}
