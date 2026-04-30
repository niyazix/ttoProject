import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../../../../core/services/api.service';

@Component({
  selector: 'app-home',
  standalone: false,
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  latestNews: any[]         = [];
  latestAnnouncements: any[] = [];
  loading = true;

  stats = [
    { icon: 'fa-file-certificate', value: '150+', label: 'Patent Başvurusu'   },
    { icon: 'fa-handshake',        value: '80+',  label: 'Lisans Anlaşması'   },
    { icon: 'fa-flask',            value: '40+',  label: 'Spin-off Şirketi'   },
    { icon: 'fa-user-tie',         value: '200+', label: 'Desteklenen Girişimci' },
  ];

  services = [
    { icon: 'fa-file-signature', title: 'Patent & Marka',    desc: 'Fikri mülkiyet haklarının korunması ve yönetimi.'   },
    { icon: 'fa-chart-line',     title: 'Teknoloji Transferi', desc: 'Araştırma sonuçlarının sektöre aktarılması.'        },
    { icon: 'fa-rocket',         title: 'Girişimcilik',       desc: 'Startup ve spin-off destek programları.'            },
    { icon: 'fa-handshake',      title: 'Sanayi İşbirlikleri', desc: 'Üniversite-sanayi ortak AR-GE projeleri.'          },
    { icon: 'fa-coins',          title: 'Fon & Teşvik',       desc: 'TÜBİTAK, AB ve ulusal hibe programları.'           },
    { icon: 'fa-graduation-cap', title: 'Eğitim & Mentorluk', desc: 'Girişimcilik eğitimleri ve mentör ağı.'            },
  ];

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.api.get<any>('news', { pageSize: 3 }).subscribe({
      next: r => this.latestNews = r.items ?? [],
      error: () => {}
    });
    this.api.get<any>('announcements', { pageSize: 4 }).subscribe({
      next: r => { this.latestAnnouncements = r.items ?? []; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  formatDate(d: string): string {
    return new Date(d).toLocaleDateString('tr-TR', { day: 'numeric', month: 'long', year: 'numeric' });
  }
}
