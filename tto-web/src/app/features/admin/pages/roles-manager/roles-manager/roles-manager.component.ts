import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../../../../core/services/api.service';

@Component({
  selector: 'app-roles-manager',
  standalone: false,
  templateUrl: './roles-manager.component.html',
  styleUrl: './roles-manager.component.scss'
})
export class RolesManagerComponent implements OnInit {
  roles:         any[] = [];
  permissions:   any[] = [];
  modules:       string[] = [];
  loading        = false;
  showForm       = false;
  showPerms      = false;
  saving         = false;
  editRole:      any   = null;
  permRole:      any   = null;
  rolePerms:     number[] = [];
  confirmDelete: any   = null;
  formData = { name: '', description: '' };

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.loadRoles();
    this.api.get<any[]>('permissions').subscribe({
      next: r => {
        this.permissions = r;
        this.modules = [...new Set(r.map((p: any) => p.module))];
      },
      error: () => {}
    });
  }

  loadRoles() {
    this.loading = true;
    this.api.get<any[]>('roles/all').subscribe({
      next: r => { this.roles = r; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  permsByModule(module: string) { return this.permissions.filter(p => p.module === module); }

  openCreate() {
    this.editRole = null;
    this.formData = { name: '', description: '' };
    this.showForm = true;
  }

  openEdit(role: any) {
    this.editRole = role;
    this.formData = { name: role.name, description: role.description ?? '' };
    this.showForm = true;
  }

  saveForm() {
    this.saving = true;
    const req = this.editRole
      ? this.api.put(`roles/${this.editRole.id}`, this.formData)
      : this.api.post('roles', this.formData);
    req.subscribe({ next: () => { this.showForm = false; this.saving = false; this.loadRoles(); }, error: () => { this.saving = false; } });
  }

  openPerms(role: any) {
    this.permRole = role;
    this.api.get<number[]>(`roles/${role.id}/permissions`).subscribe({
      next: r => { this.rolePerms = r; this.showPerms = true; },
      error: () => { this.rolePerms = []; this.showPerms = true; }
    });
  }

  hasPermission(permId: number) { return this.rolePerms.includes(permId); }

  togglePerm(permId: number) {
    if (this.hasPermission(permId)) {
      this.api.delete(`roles/${this.permRole.id}/permissions/${permId}`).subscribe({ next: () => this.rolePerms = this.rolePerms.filter(p => p !== permId) });
    } else {
      this.api.post(`roles/${this.permRole.id}/permissions`, { permissionId: permId }).subscribe({ next: () => this.rolePerms = [...this.rolePerms, permId] });
    }
  }

  delete(r: any) { this.confirmDelete = r; }
  doDelete() {
    this.api.delete(`roles/${this.confirmDelete.id}`).subscribe({
      next: () => { this.roles = this.roles.filter(r => r.id !== this.confirmDelete.id); this.confirmDelete = null; }
    });
  }
}
