import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path : 'auth', loadChildren : () => import('./auth/auth.module').then(m => m.AuthModule)
  },
  {
    path : 'user-management', loadChildren: () => import('../app/user-management/user-management.module').then(m=> m.UserManagementModule)
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
