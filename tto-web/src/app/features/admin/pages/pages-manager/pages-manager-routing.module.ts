import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PagesManagerComponent } from './pages-manager/pages-manager.component';

const routes: Routes = [
  { path: '', component: PagesManagerComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PagesManagerRoutingModule {}
