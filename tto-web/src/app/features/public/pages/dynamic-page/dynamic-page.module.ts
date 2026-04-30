import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DynamicPageRoutingModule } from './dynamic-page-routing.module';
import { DynamicPageComponent } from './dynamic-page/dynamic-page.component';


@NgModule({
  declarations: [
    DynamicPageComponent
  ],
  imports: [
    CommonModule,
    DynamicPageRoutingModule
  ]
})
export class DynamicPageModule { }
