import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SharedRoutingModule } from './shared-routing.module';
import { HeaderComponent } from './header/header.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import { PhoneFormatPipe } from './pipes/phone-format.pipe';



@NgModule({
  declarations: [
    HeaderComponent,
    SidebarComponent,
    PhoneFormatPipe,
  ],
  imports: [
    CommonModule,
    SharedRoutingModule
  ],
  exports : [
    HeaderComponent,
    SidebarComponent,
    PhoneFormatPipe,
  ]
})
export class SharedModule { }
