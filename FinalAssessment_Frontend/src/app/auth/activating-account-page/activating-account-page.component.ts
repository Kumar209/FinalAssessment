import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../service/auth.service';

@Component({
  selector: 'app-activating-account-page',
  templateUrl: './activating-account-page.component.html',
  styleUrls: ['./activating-account-page.component.css']
})
export class ActivatingAccountPageComponent {
  constructor(private router: Router, private service : AuthService, private toastr : ToastrService){}


  activateAccount(){
    this.service.activateAccount().subscribe({
      next : (res) => {
        if(res.success){
          this.toastr.success(res.message, 'Successfully!');
          this.router.navigate(['/auth/login']);
        }
        else{
          this.toastr.error(res.message, 'Error!')
        }
      },

      error : (err) => {
        if(err.error && err.error.message){
          this.toastr.error(err.error.message, 'Error!');
        }
        else {
          this.toastr.error('Something went wrong', 'Error!');
        }
      }
    })
  }
}
