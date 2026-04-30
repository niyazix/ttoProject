import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { ContactManagerRoutingModule } from './contact-manager-routing.module';
import { ContactManagerComponent } from './contact-manager/contact-manager.component';


@NgModule({
  declarations: [
    ContactManagerComponent
  ],
  imports: [
    CommonModule, RouterModule, FormsModule,
    ContactManagerRoutingModule
  ]
})
export class ContactManagerModule { }
