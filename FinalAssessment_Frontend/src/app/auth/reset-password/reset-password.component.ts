import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../service/auth.service';
import { ToastrService } from 'ngx-toastr';
import {
  AbstractControl,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { IResetCredential } from '../Interface/IResetCredential';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css'],
})
export class ResetPasswordComponent {
  resetForm: any;

  showPassword: boolean = false;
  showConfirmPassword : boolean = false;

  constructor(
    private router: Router,
    private service: AuthService,
    private toastr: ToastrService
  ) {}

  passwordShowHide() {
    this.showPassword = !this.showPassword;
  }

  confirmPasswordShowHide() {
    this.showConfirmPassword = !this.showConfirmPassword;
  }


  ngOnInit(): void {
    this.resetForm = new FormGroup(
      {
        password: new FormControl('', [Validators.required]),
        confirmPassword: new FormControl('', [Validators.required]),
      },
      {
        validators: this.passwordMatchValidator,
      }
    );
  }



  passwordMatchValidator(control: AbstractControl) {
    const password = control.get('password');
    const confirmPassword = control.get('confirmPassword');

    const mismatch = password?.value !== confirmPassword?.value;

    confirmPassword?.setErrors(mismatch ? { mismatch: true } : null);
    return mismatch ? { mismatch: true } : null;
  }



  onSubmitResetForm() {
    if (this.resetForm.valid) {
      const resetCredential: IResetCredential = {
        password: this.resetForm.value.password,
        confirmPassword: this.resetForm.value.confirmPassword,
      };

      this.service.resetPassword(resetCredential).subscribe({
        next: (res) => {
          if (res.success) {
            this.toastr.success(res.message, 'Successfull!');
            this.router.navigateByUrl("/auth/login");
          } else {
            this.toastr.error(res.message, 'Error!');
          }
        },

        error: (err) => {
          if (err.error && err.error.message) {
            this.toastr.error(err.error.message, 'Error!');
          } else {
            this.toastr.error('Something went wrong!', 'Error!');
          }
        },
      });
    } else {
      this.toastr.error('Validation Failed', 'Error!');
    }
  }
}
