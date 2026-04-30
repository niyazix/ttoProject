import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { AnnouncementsRoutingModule } from './announcements-routing.module';
import { AnnouncementsListComponent } from './announcements-list/announcements-list.component';
import { AnnouncementDetailComponent } from './announcement-detail/announcement-detail.component';


@NgModule({
  declarations: [
    AnnouncementsListComponent,
    AnnouncementDetailComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    AnnouncementsRoutingModule
  ]
})
export class AnnouncementsModule { }
