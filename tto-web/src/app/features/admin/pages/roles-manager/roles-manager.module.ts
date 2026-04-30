import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { RolesManagerRoutingModule } from './roles-manager-routing.module';
import { RolesManagerComponent } from './roles-manager/roles-manager.component';


@NgModule({
  declarations: [
    RolesManagerComponent
  ],
  imports: [
    CommonModule, RouterModule, FormsModule,
    RolesManagerRoutingModule
  ]
})
export class RolesManagerModule { }
