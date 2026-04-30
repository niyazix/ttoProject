import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { AnnouncementsManagerRoutingModule } from './announcements-manager-routing.module';
import { AnnouncementsManagerComponent } from './announcements-manager/announcements-manager.component';


@NgModule({
  declarations: [
    AnnouncementsManagerComponent
  ],
  imports: [
    CommonModule, RouterModule, FormsModule,
    AnnouncementsManagerRoutingModule
  ]
})
export class AnnouncementsManagerModule { }
