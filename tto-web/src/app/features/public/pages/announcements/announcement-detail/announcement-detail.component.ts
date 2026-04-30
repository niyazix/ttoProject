import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../../../../../core/services/api.service';

@Component({
  selector: 'app-announcement-detail',
  standalone: false,
  templateUrl: './announcement-detail.component.html',
  styleUrl: './announcement-detail.component.scss'
})
export class AnnouncementDetailComponent implements OnInit {
  announcement: any = null;
  loading  = true;
  notFound = false;

  constructor(private route: ActivatedRoute, private api: ApiService) {}

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      const slug = params.get('slug')!;
      this.loading = true;
      this.notFound = false;
      this.api.get<any>(`announcements/${slug}`).subscribe({
        next:  a => { this.announcement = a; this.loading = false; },
        error: e => { this.notFound = e.status === 404; this.loading = false; }
      });
    });
  }

  formatDate(d: string) {
    return new Date(d).toLocaleDateString('tr-TR', { day: 'numeric', month: 'long', year: 'numeric' });
  }
}
