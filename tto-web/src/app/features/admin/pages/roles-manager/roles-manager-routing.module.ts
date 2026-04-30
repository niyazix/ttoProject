import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RolesManagerComponent } from './roles-manager/roles-manager.component';

const routes: Routes = [
  { path: '', component: RolesManagerComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RolesManagerRoutingModule {}
