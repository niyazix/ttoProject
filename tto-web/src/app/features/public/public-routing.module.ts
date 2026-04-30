import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PublicLayoutComponent } from './layout/public-layout/public-layout.component';

const routes: Routes = [
  {
    path: '',
    component: PublicLayoutComponent,
    children: [
      { path: '', loadChildren: () => import('./pages/home/home.module').then(m => m.HomeModule) },
      { path: 'duyurular', loadChildren: () => import('./pages/announcements/announcements.module').then(m => m.AnnouncementsModule) },
      { path: 'haberler', loadChildren: () => import('./pages/news/news.module').then(m => m.NewsModule) },
      { path: 'iletisim', loadChildren: () => import('./pages/contact/contact.module').then(m => m.ContactModule) },
      { path: ':slug', loadChildren: () => import('./pages/dynamic-page/dynamic-page.module').then(m => m.DynamicPageModule) }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PublicRoutingModule {}
