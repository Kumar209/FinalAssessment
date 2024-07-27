import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { ForgotPasswordComponent } from './forgot-password/forgot-password.component';
import { EmailSendPageComponent } from './email-send-page/email-send-page.component';
import { ResetPasswordComponent } from './reset-password/reset-password.component';
import { ChangePasswordComponent } from './change-password/change-password.component';

const routes: Routes = [
  {
    path: '', component : LoginComponent
  },
  {
    path: 'login', component : LoginComponent
  },
  {
    path : 'forgot-password', component : ForgotPasswordComponent
  },
  {
    path : 'emailSend' , component: EmailSendPageComponent
  },
  {
    path: 'reset-password', component : ResetPasswordComponent
  },
  {
    path: 'change-password' , component : ChangePasswordComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AuthRoutingModule { }
