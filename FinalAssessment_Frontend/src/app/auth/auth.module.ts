import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AuthRoutingModule } from './auth-routing.module';
import { LoginComponent } from './login/login.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ForgotPasswordComponent } from './forgot-password/forgot-password.component';
import { EmailSendPageComponent } from './email-send-page/email-send-page.component';

import { HttpClientModule } from "@angular/common/http";
import { ResetPasswordComponent } from './reset-password/reset-password.component';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { ActivatingAccountPageComponent } from './activating-account-page/activating-account-page.component';

@NgModule({
  declarations: [
    LoginComponent,
    ForgotPasswordComponent,
    EmailSendPageComponent,
    ResetPasswordComponent,
    ChangePasswordComponent,
    ActivatingAccountPageComponent
  ],
  imports: [
    CommonModule,
    AuthRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    HttpClientModule
  ]
})
export class AuthModule { }
