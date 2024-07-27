import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../service/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent {
  changePasswordForm : any;

  constructor(private router : Router, private service : AuthService, private toastr : ToastrService) {}


  ngOnInit(): void {
    this.changePasswordForm = new FormGroup({
      oldPassword : new FormControl('', [Validators.required]),
      newPassword : new FormControl('', [Validators.required])
    });
  }


  onSubmitChangePasswordForm() {
    if(this.changePasswordForm.valid){
      

      // this.service.login(this.resetForm.value).subscribe({
      //   next : (res) => {
      //     if(res.success){
      //       this.toastr.success(res.message, 'Successfull!');
      //     }
      //     else {
      //       this.toastr.error(res.message, 'Error!');
      //     }
      //   },

      //   error : (err) => {
      //     if(err.error && err.error.message){
      //       this.toastr.error(err.error.message, 'Error!');
      //     }
      //     else {
      //       this.toastr.error('Something went wrong!', 'Error!');
      //     }
      //   }
      // })
    }
    else {
      this.toastr.error('Validation Failed', 'Error!')
    }
  }
}
