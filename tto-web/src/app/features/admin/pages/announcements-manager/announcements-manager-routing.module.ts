import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AnnouncementsManagerComponent } from './announcements-manager/announcements-manager.component';

const routes: Routes = [
  { path: '', component: AnnouncementsManagerComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AnnouncementsManagerRoutingModule {}
