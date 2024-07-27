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

  constructor(private router : Router, private service : AuthService, private toastr : ToastrService) {}


  ngOnInit(): void {
    this.loginForm = new FormGroup({
      email : new FormControl('', [Validators.required, Validators.email, Validators.pattern("^[a-zA-Z0-9._%±]+@[a-zA-Z0-9.-]+.+[a-zA-Z]{2,}$")]),
      password : new FormControl('', [Validators.required])
    });
  }


  onSubmitLoginForm() {
    if(this.loginForm.valid){
      

      this.service.login(this.loginForm.value).subscribe({
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
