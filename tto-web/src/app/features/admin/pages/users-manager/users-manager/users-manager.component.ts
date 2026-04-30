import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../../../../core/services/api.service';

@Component({
  selector: 'app-users-manager',
  standalone: false,
  templateUrl: './users-manager.component.html',
  styleUrl: './users-manager.component.scss'
})
export class UsersManagerComponent implements OnInit {
  items:         any[] = [];
  roles:         any[] = [];
  permissions:   any[] = [];
  loading        = false;
  showForm       = false;
  showPerms      = false;
  saving         = false;
  editItem:      any   = null;
  permTarget:    any   = null;
  confirmDelete: any   = null;
  formData = { email: '', username: '', firstName: '', lastName: '', password: '', roleIds: [] as number[] };
  userPerms: { permissionId: number; isGranted: boolean }[] = [];

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.load();
    this.api.get<any[]>('roles/all').subscribe({ next: r => this.roles = r, error: () => {} });
    this.api.get<any[]>('permissions').subscribe({ next: r => this.permissions = r, error: () => {} });
  }

  load() {
    this.loading = true;
    this.api.get<any[]>('users').subscribe({
      next: r => { this.items = r; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  openCreate() {
    this.editItem = null;
    this.formData = { email: '', username: '', firstName: '', lastName: '', password: '', roleIds: [] };
    this.showForm = true;
  }

  saveForm() {
    this.saving = true;
    this.api.post('auth/register', this.formData).subscribe({
      next: () => { this.showForm = false; this.saving = false; this.load(); },
      error: () => { this.saving = false; }
    });
  }

  toggleRole(user: any, roleId: number) {
    const has = user.roles?.includes(this.roles.find(r => r.id === roleId)?.name);
    const req = has
      ? this.api.delete(`users/${user.id}/roles/${roleId}`)
      : this.api.post(`users/${user.id}/roles`, { roleId });
    req.subscribe({ next: () => this.load() });
  }

  toggleActive(user: any) {
    this.api.patch(`users/${user.id}/toggle-active`).subscribe({ next: () => user.isActive = !user.isActive });
  }

  delete(u: any) { this.confirmDelete = u; }
  doDelete() {
    this.api.delete(`users/${this.confirmDelete.id}`).subscribe({
      next: () => { this.items = this.items.filter(i => i.id !== this.confirmDelete.id); this.confirmDelete = null; }
    });
  }
}
