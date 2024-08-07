import { Component } from '@angular/core';
import { AbstractControl, FormControl, FormControlDirective, FormGroup, Validators } from '@angular/forms';
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

  oldPassword : boolean = false;
  showPassword: boolean = false;
  showConfirmPassword : boolean = false;
  
  constructor(private router : Router, private service : AuthService, private toastr : ToastrService) {}

  oldPasswordShowHide() {
    this.oldPassword = !this.oldPassword;
  }

  passwordShowHide() {
    this.showPassword = !this.showPassword;
  }

  confirmPasswordShowHide() {
    this.showConfirmPassword = !this.showConfirmPassword;
  }


  ngOnInit(): void {
    this.changePasswordForm = new FormGroup({
      oldPassword : new FormControl('', [Validators.required]),
      newPassword : new FormControl('', [
        Validators.required,
        this.noSpacesValidator
      ]),
      confirmNewPassword : new FormControl('', [Validators.required])
    },
    {
      validators: this.passwordMatchValidator,
    }
  );
  }


    // Custom validator to disallow spaces
    noSpacesValidator(control: AbstractControl): { [key: string]: boolean } | null {
      if (control.value && control.value.includes(' ')) {
        return { noSpaces: true };
      }
      return null;
    }
  
  


  passwordMatchValidator(control: AbstractControl) {

    const password = control.get('newPassword');
    const confirmPassword = control.get('confirmNewPassword');

    const mismatch = password?.value !== confirmPassword?.value;

    confirmPassword?.setErrors(mismatch ? { mismatch: true } : null);
    return mismatch ? { mismatch: true } : null;
  }


  onSubmitChangePasswordForm() {
    if(this.changePasswordForm.valid){
      

      this.service.changePassword(this.changePasswordForm.value).subscribe({

        next : (res) => {
          debugger;
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
            this.toastr.error('Wrong password!', 'Error!');
          }
        }
     } )
    }
    else {
      this.toastr.error('Validation Failed', 'Error!')
    }
  }
}
