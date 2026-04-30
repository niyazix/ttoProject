import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { ApiService } from '../../../../../core/services/api.service';

interface ContentBlock { id: number; blockType: string; content: string | null; sortOrder: number; isActive: boolean; }
interface PageDetail   { id: number; title: string; slug: string; metaTitle: string | null; metaDescription: string | null; contentBlocks: ContentBlock[]; }

@Component({
  selector: 'app-dynamic-page',
  standalone: false,
  templateUrl: './dynamic-page.component.html',
  styleUrl: './dynamic-page.component.scss'
})
export class DynamicPageComponent implements OnInit {
  page:     PageDetail | null = null;
  loading   = true;
  notFound  = false;

  constructor(
    private route:     ActivatedRoute,
    private api:       ApiService,
    private sanitizer: DomSanitizer
  ) {}

  ngOnInit() {
    const slug = this.route.snapshot.parent?.paramMap.get('slug') ?? '';
    if (!slug) { this.notFound = true; this.loading = false; return; }

    this.api.get<PageDetail>(`pages/${slug}`).subscribe({
      next:  p  => { this.page = p; this.loading = false; },
      error: () => { this.notFound = true; this.loading = false; }
    });
  }

  trust(html: string | null): SafeHtml {
    return this.sanitizer.bypassSecurityTrustHtml(html ?? '');
  }

  isRich(type: string)  { return ['richtext', 'html', 'banner', 'video', 'gallery'].includes(type); }
  isImage(type: string) { return type === 'image'; }
}
