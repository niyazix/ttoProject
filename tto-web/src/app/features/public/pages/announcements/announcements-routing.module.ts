import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AnnouncementsListComponent } from './announcements-list/announcements-list.component';
import { AnnouncementDetailComponent } from './announcement-detail/announcement-detail.component';

const routes: Routes = [
  { path: '', component: AnnouncementsListComponent },
  { path: ':slug', component: AnnouncementDetailComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AnnouncementsRoutingModule {}
