import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { UserService } from '../../core/services/user.service';
import { AuthService } from '../../core/services/auth.service';
import { User } from '../../core/models/user.model';
import { ToastrService } from 'ngx-toastr';
export const passwordMatchValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const password = control.get('passwordHash');
  const confirmPassword = control.get('confirmPassword');
  return password && confirmPassword && password.value !== confirmPassword.value
    ? { passwordMismatch: true } : null;
};
@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {
  private fb = inject(FormBuilder);
  private userService = inject(UserService);
  private authService = inject(AuthService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  ngOnInit() {
    this.authService.logout();
  }
  showPassword = false;
  showConfirmPassword = false;
  registerForm = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.required, Validators.email]],
    passwordHash: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', [Validators.required]]
  }, { validators: passwordMatchValidator });
  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }
  toggleConfirmPasswordVisibility() {
    this.showConfirmPassword = !this.showConfirmPassword;
  }
  onSubmit() {
    if (this.registerForm.valid) {
      const newUser: User = {
        name: this.registerForm.value.name!,
        email: this.registerForm.value.email!,
        passwordHash: this.registerForm.value.passwordHash!,
        roleId: 3, // Default to standard User role in the new DB structure
        isActive: true
      };
      this.userService.createUser(newUser).subscribe({
        next: () => {
          this.toastr.success('Registration successful! You can now log in.', 'Success');
          this.registerForm.reset();
          setTimeout(() => this.router.navigate(['/login']), 2000);
        },
        error: (err) => {
          this.toastr.error(err.error?.message || 'Registration failed.', 'Error');
        }
      });
    } else {
      this.toastr.warning('Please fill all required fields correctly.', 'Validation Error');
      this.registerForm.markAllAsTouched();
    }
  }
}
