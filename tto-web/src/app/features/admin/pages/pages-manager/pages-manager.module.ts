import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PagesManagerRoutingModule } from './pages-manager-routing.module';
import { PagesManagerComponent } from './pages-manager/pages-manager.component';

@NgModule({
  declarations: [PagesManagerComponent],
  imports: [CommonModule, RouterModule, FormsModule, PagesManagerRoutingModule]
})
export class PagesManagerModule {}
