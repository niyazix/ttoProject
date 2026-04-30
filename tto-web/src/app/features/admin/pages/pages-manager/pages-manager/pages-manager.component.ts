import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../../../../core/services/api.service';

interface PageForm  { title: string; slug: string; metaTitle: string; metaDescription: string; sortOrder: number; }
interface BlockForm { blockType: string; content: string; sortOrder: number; isActive: boolean; }

const BLOCK_TYPES = [
  { value: 'richtext',  label: 'Zengin Metin (HTML)' },
  { value: 'image',     label: 'Görsel'               },
  { value: 'banner',    label: 'Banner'               },
  { value: 'video',     label: 'Video Embed'          },
  { value: 'html',      label: 'Ham HTML'             },
  { value: 'gallery',   label: 'Galeri'               },
];

@Component({
  selector: 'app-pages-manager',
  standalone: false,
  templateUrl: './pages-manager.component.html',
  styleUrl: './pages-manager.component.scss'
})
export class PagesManagerComponent implements OnInit {
  // Page list
  items:         any[] = [];
  loading        = false;
  confirmDelete: any   = null;

  // Page form drawer
  showForm  = false;
  editItem: any   = null;
  saving    = false;
  pageForm: PageForm = this.emptyPageForm();

  // Content blocks panel
  showBlocks    = false;
  blocksPage:   any    = null;
  blocks:       any[]  = [];
  blocksLoading = false;

  // Block form drawer
  showBlockForm  = false;
  editBlock:    any    = null;
  blockSaving   = false;
  confirmBlockDelete: any = null;
  blockForm: BlockForm = this.emptyBlockForm();
  blockTypes = BLOCK_TYPES;

  constructor(private api: ApiService) {}
  ngOnInit() { this.load(); }

  load() {
    this.loading = true;
    this.api.get<any[]>('pages/admin/all').subscribe({
      next: r => { this.items = r; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  // ── Page CRUD ──
  openCreate() { this.editItem = null; this.pageForm = this.emptyPageForm(); this.showForm = true; }
  openEdit(p: any) {
    this.editItem = p;
    this.pageForm = { title: p.title, slug: p.slug, metaTitle: p.metaTitle ?? '', metaDescription: p.metaDescription ?? '', sortOrder: p.sortOrder };
    this.showForm = true;
  }

  savePage() {
    if (!this.pageForm.title.trim()) return;
    this.saving = true;
    const payload = { ...this.pageForm, sortOrder: +this.pageForm.sortOrder };
    const req = this.editItem
      ? this.api.put(`pages/${this.editItem.id}`, payload)
      : this.api.post('pages', { ...payload, pageType: 'Dynamic' });
    req.subscribe({
      next: () => { this.showForm = false; this.saving = false; this.load(); },
      error: (err) => { this.saving = false; alert('Kayıt başarısız: ' + (err?.error?.message ?? err?.status ?? 'Bilinmeyen hata')); }
    });
  }

  togglePublish(p: any) {
    this.api.patch(`pages/${p.id}/publish`).subscribe({
      next: (r: any) => p.isPublished = r.isPublished,
      error: (err) => alert('Durum değiştirilemedi: ' + (err?.error?.message ?? err?.status ?? 'Bilinmeyen hata'))
    });
  }

  delete(p: any) { this.confirmDelete = p; }
  doDelete() {
    this.api.delete(`pages/${this.confirmDelete.id}`).subscribe({
      next: () => { this.items = this.items.filter(i => i.id !== this.confirmDelete.id); this.confirmDelete = null; }
    });
  }

  onTitleChange() { if (!this.editItem) this.pageForm.slug = this.toSlug(this.pageForm.title); }

  // ── Content Blocks ──
  openBlocks(p: any) {
    this.blocksPage    = p;
    this.showBlocks    = true;
    this.blocksLoading = true;
    this.api.get<any[]>(`pages/${p.id}/blocks`).subscribe({
      next: r => { this.blocks = r ?? []; this.blocksLoading = false; },
      error: () => { this.blocks = []; this.blocksLoading = false; }
    });
  }

  openAddBlock() {
    this.editBlock = null;
    this.blockForm = this.emptyBlockForm();
    this.blockForm.sortOrder = this.blocks.length;
    this.showBlockForm = true;
  }

  openEditBlock(b: any) {
    this.editBlock = b;
    this.blockForm = { blockType: b.blockType, content: b.content ?? '', sortOrder: b.sortOrder, isActive: b.isActive };
    this.showBlockForm = true;
  }

  saveBlock() {
    this.blockSaving = true;
    const req = this.editBlock
      ? this.api.put(`pages/${this.blocksPage.id}/blocks/${this.editBlock.id}`, this.blockForm)
      : this.api.post(`pages/${this.blocksPage.id}/blocks`, this.blockForm);
    req.subscribe({
      next: () => { this.showBlockForm = false; this.blockSaving = false; this.openBlocks(this.blocksPage); },
      error: () => { this.blockSaving = false; }
    });
  }

  deleteBlock(b: any) { this.confirmBlockDelete = b; }
  doDeleteBlock() {
    this.api.delete(`pages/${this.blocksPage.id}/blocks/${this.confirmBlockDelete.id}`).subscribe({
      next: () => { this.blocks = this.blocks.filter(b => b.id !== this.confirmBlockDelete.id); this.confirmBlockDelete = null; }
    });
  }

  insertBlockTag(tag: string) {
    const ta = document.getElementById('block-content') as HTMLTextAreaElement;
    if (!ta) return;
    const s = ta.selectionStart, e = ta.selectionEnd;
    const sel = this.blockForm.content.substring(s, e);
    const wrapped = `<${tag}>${sel || 'metin'}</${tag}>`;
    this.blockForm.content = this.blockForm.content.substring(0, s) + wrapped + this.blockForm.content.substring(e);
    setTimeout(() => ta.focus());
  }

  blockTypeLabel(type: string) {
    return this.blockTypes.find(t => t.value === type)?.label ?? type;
  }

  toSlug(t: string) {
    return t.toLowerCase()
      .replace(/ş/g,'s').replace(/ç/g,'c').replace(/ğ/g,'g')
      .replace(/ü/g,'u').replace(/ö/g,'o').replace(/ı/g,'i').replace(/İ/g,'i')
      .replace(/[^a-z0-9]+/g,'-').replace(/^-|-$/g,'');
  }

  private emptyPageForm(): PageForm {
    return { title: '', slug: '', metaTitle: '', metaDescription: '', sortOrder: 0 };
  }

  private emptyBlockForm(): BlockForm {
    return { blockType: 'richtext', content: '', sortOrder: 0, isActive: true };
  }
}
