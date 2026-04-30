import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { UsersManagerRoutingModule } from './users-manager-routing.module';
import { UsersManagerComponent } from './users-manager/users-manager.component';


@NgModule({
  declarations: [
    UsersManagerComponent
  ],
  imports: [
    CommonModule, RouterModule, FormsModule,
    UsersManagerRoutingModule
  ]
})
export class UsersManagerModule { }
