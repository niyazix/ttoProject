import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { MenuManagerRoutingModule } from './menu-manager-routing.module';
import { MenuManagerComponent } from './menu-manager/menu-manager.component';


@NgModule({
  declarations: [
    MenuManagerComponent
  ],
  imports: [
    CommonModule, RouterModule, FormsModule,
    MenuManagerRoutingModule
  ]
})
export class MenuManagerModule { }
