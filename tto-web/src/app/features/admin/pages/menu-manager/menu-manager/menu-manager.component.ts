import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../../../../core/services/api.service';

@Component({
  selector: 'app-menu-manager',
  standalone: false,
  templateUrl: './menu-manager.component.html',
  styleUrl: './menu-manager.component.scss'
})
export class MenuManagerComponent implements OnInit {
  items:         any[] = [];
  pages:         any[] = [];
  loading        = false;
  confirmDelete: any   = null;
  showForm       = false;
  saving         = false;
  editItem:      any   = null;
  formData = {
    title: '', linkType: 'none', pageId: null as number|null,
    url: '', parentId: null as number|null, menuGroup: 'main',
    sortOrder: 0, openInNewTab: false, icon: ''
  };

  linkTypes = [
    { value: 'none', label: 'Sadece Başlık (tıklanmaz)' },
    { value: 'page', label: 'Dahili Sayfa' },
    { value: 'url',  label: 'Harici URL' },
  ];

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.load();
    this.api.get<any[]>('pages/admin/all').subscribe({ next: r => this.pages = r, error: () => {} });
  }

  load() {
    this.loading = true;
    this.api.get<any[]>('menu/main').subscribe({
      next: items => { this.items = this.flatten(items); this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  flatten(items: any[], depth = 0): any[] {
    const result: any[] = [];
    for (const item of items) {
      result.push({ ...item, _depth: depth });
      if (item.children?.length) result.push(...this.flatten(item.children, depth + 1));
    }
    return result;
  }

  openCreate() {
    this.editItem = null;
    this.formData = { title: '', linkType: 'none', pageId: null, url: '', parentId: null, menuGroup: 'main', sortOrder: 0, openInNewTab: false, icon: '' };
    this.showForm = true;
  }

  openEdit(item: any) {
    this.editItem = item;
    this.formData = { title: item.title, linkType: item.linkType?.toLowerCase() ?? 'none', pageId: item.pageId ?? null, url: item.url ?? '', parentId: item.parentId ?? null, menuGroup: item.menuGroup ?? 'main', sortOrder: item.sortOrder, openInNewTab: item.openInNewTab, icon: item.icon ?? '' };
    this.showForm = true;
  }

  saveForm() {
    this.saving = true;
    const req = this.editItem
      ? this.api.put(`menu/${this.editItem.id}`, this.formData)
      : this.api.post('menu', this.formData);
    req.subscribe({ next: () => { this.showForm = false; this.saving = false; this.load(); }, error: () => { this.saving = false; } });
  }

  delete(item: any) { this.confirmDelete = item; }
  doDelete() {
    this.api.delete(`menu/${this.confirmDelete.id}`).subscribe({
      next: () => { this.items = this.items.filter(i => i.id !== this.confirmDelete.id && i.parentId !== this.confirmDelete.id); this.confirmDelete = null; }
    });
  }
}
