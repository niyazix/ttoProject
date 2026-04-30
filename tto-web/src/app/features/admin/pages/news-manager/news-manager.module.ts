import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NewsManagerRoutingModule } from './news-manager-routing.module';
import { NewsManagerComponent } from './news-manager/news-manager.component';

@NgModule({
  declarations: [NewsManagerComponent],
  imports: [CommonModule, RouterModule, FormsModule, NewsManagerRoutingModule]
})
export class NewsManagerModule {}
