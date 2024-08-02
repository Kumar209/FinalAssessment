import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../service/auth.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent {
  forgotPasswordForm : any;

  constructor(private router : Router, private service : AuthService, private toastr : ToastrService) {}


  ngOnInit(): void {
    this.forgotPasswordForm = new FormGroup({
      email : new FormControl('', [Validators.required, Validators.pattern('^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$')])
    });
  }


  onSubmitForgotPasswordForm() {
    if(this.forgotPasswordForm.valid){

      this.service.forgotPassword(this.forgotPasswordForm.value.email).subscribe({
        next : (res) => {
          if(res.success){
            this.toastr.success(res.message, 'Successfull!');
          }
          else {
            this.toastr.error(res.message, 'Error!');
          }
        },

        error : (err) => {
          if(err.error && err.error.message){
            this.toastr.error(err.error.message, 'Error!');
          }
          else {
            this.toastr.error('Something went wrong!', 'Error!');
          }
        }
      })

    }
    else {
      this.toastr.error('Validation Failed', 'Error!')
    }
  }
}
