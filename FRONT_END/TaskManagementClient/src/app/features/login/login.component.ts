import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { ToastrService } from 'ngx-toastr';
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  loginForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });
  ngOnInit() {
    // Ensure any stale sessions are cleared when hitting the login page
    this.authService.logout();
  }
  showPassword = false;
  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }
  onSubmit() {
    if (this.loginForm.valid) {
      const credentials = {
        email: this.loginForm.value.email!,
        password: this.loginForm.value.password!
      };
      this.authService.login(credentials).subscribe({
        next: () => {
          this.toastr.success('Logged in successfully!', 'Welcome');
          this.router.navigate(['/dashboard']);
        },
        error: (err) => {
          this.toastr.error(err.error?.message || 'Login failed. Invalid credentials.', 'Login Error');
        }
      });
    } else {
      this.toastr.warning('Please enter valid credentials.', 'Validation');
      this.loginForm.markAllAsTouched();
    }
  }
}
