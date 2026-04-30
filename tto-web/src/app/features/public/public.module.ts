import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { PublicRoutingModule } from './public-routing.module';
import { PublicLayoutComponent } from './layout/public-layout/public-layout.component';

@NgModule({
  declarations: [PublicLayoutComponent],
  imports: [CommonModule, RouterModule, PublicRoutingModule]
})
export class PublicModule {}
