import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NewsManagerComponent } from './news-manager/news-manager.component';

const routes: Routes = [
  { path: '', component: NewsManagerComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class NewsManagerRoutingModule {}
