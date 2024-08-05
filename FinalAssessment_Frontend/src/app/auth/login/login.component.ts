import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../service/auth.service';
import { ToastrService } from 'ngx-toastr';
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  loginForm : any;

  //Variable used to show and hide the password
  showPassword: boolean = false;

  constructor(private router : Router, private service : AuthService, private toastr : ToastrService) {}

 

  //Function used to toggle the showPassword variable
  passwordShowHide() {
    this.showPassword = !this.showPassword;
  }


  ngOnInit(): void {
    this.loginForm = new FormGroup({
      email : new FormControl('', [Validators.required, Validators.pattern('^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$')]),
      password : new FormControl('', [Validators.required])
    });
  }


  onSubmitLoginForm() {
    if(this.loginForm.valid){
      

      this.service.login(this.loginForm.value).subscribe({
        next : (res) => {
          if(res.success){
            localStorage.setItem(this.service.authSecretKey, res.token);
            localStorage.setItem(this.service.authUserCookieKey, JSON.stringify(res.requiredDataForFrontend));
            this.router.navigate(['user-management/dashboard']);
            this.toastr.success(res.message, 'Successfull!');
          }

          if(res.success == false && res.message == "InActive"){
            this.router.navigate(['auth/email-send']);  
            this.toastr.error("Account is not active", 'Error!');
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
