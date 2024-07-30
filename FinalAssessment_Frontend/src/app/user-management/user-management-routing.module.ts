import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { AddUserComponent } from './add-user/add-user.component';
import { authGuard } from '../guards/auth.guard';
import { UpdateUserComponent } from './update-user/update-user.component';

const routes: Routes = [
  {
    path: 'dashboard' , component : DashboardComponent
  },
  {
    path : 'add-user' , component : AddUserComponent, canActivate : [authGuard]
  },
  {
    path : 'update-user' , component : UpdateUserComponent, canActivate : [authGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UserManagementRoutingModule { }
