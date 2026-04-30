import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../../../../core/services/api.service';

@Component({
  selector: 'app-settings',
  standalone: false,
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.scss'
})
export class SettingsComponent implements OnInit {
  settings:  any[] = [];
  grouped:   Record<string, any[]> = {};
  groups:    string[] = [];
  loading    = false;
  saving     = false;
  saved      = false;

  groupLabels: Record<string, string> = {
    general: 'Genel',
    contact: 'İletişim',
    social:  'Sosyal Medya',
    seo:     'SEO',
  };

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.loading = true;
    this.api.get<any[]>('settings').subscribe({
      next: r => {
        this.settings = r;
        this.groupSettings();
        this.loading = false;
      },
      error: () => { this.loading = false; }
    });
  }

  private groupSettings() {
    this.grouped = {};
    for (const s of this.settings) {
      if (!this.grouped[s.settingGroup]) this.grouped[s.settingGroup] = [];
      this.grouped[s.settingGroup].push(s);
    }
    this.groups = Object.keys(this.grouped);
  }

  groupLabel(key: string) { return this.groupLabels[key] ?? key; }

  save() {
    this.saving = true;
    const updates = this.settings.map(s => ({ key: s.settingKey, value: s.settingValue }));
    this.api.put('settings', updates).subscribe({
      next: () => { this.saving = false; this.saved = true; setTimeout(() => this.saved = false, 3000); },
      error: () => { this.saving = false; }
    });
  }
}
