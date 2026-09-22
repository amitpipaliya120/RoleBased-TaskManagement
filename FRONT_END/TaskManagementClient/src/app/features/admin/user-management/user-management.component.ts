import { Component, inject, OnInit, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserService } from '../../../core/services/user.service';
import { AuthService } from '../../../core/services/auth.service';
import { User } from '../../../core/models/user.model';
import { ToastrService } from 'ngx-toastr';
@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.scss']
})
export class UserManagementComponent implements OnInit {
  private fb = inject(FormBuilder);
  private userService = inject(UserService);
  private toastr = inject(ToastrService);
  private authService = inject(AuthService);
  @ViewChild('addClientModal') addClientModal!: ElementRef<HTMLDialogElement>;
  users: User[] = [];
  managersList: User[] = [];
  userForm: FormGroup;
  get isAdmin(): boolean {
    return this.authService.getRole() === 'Admin';
  }
  constructor() {
    this.userForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      passwordHash: ['', [Validators.required, Validators.minLength(6)]],
      roleId: [3, Validators.required],
      managerId: [null]
    });
  }
  ngOnInit(): void {
    this.loadUsers();
    if (this.isAdmin) {
      this.loadManagers();
    }
    // Clear managerId if role is not User (3)
    this.userForm.get('roleId')?.valueChanges.subscribe(role => {
      if (Number(role) !== 3) {
        this.userForm.get('managerId')?.setValue(null);
      }
    });
  }
  loadUsers() {
    if (this.isAdmin) {
      this.userService.getUsers().subscribe({
        next: (data) => this.users = data,
        error: () => this.toastr.error('Failed to load users.')
      });
    } else {
      this.userService.getMyUsers().subscribe({
        next: (data) => this.users = data,
        error: () => this.toastr.error('Failed to load team members.')
      });
    }
  }
  loadManagers() {
    this.userService.getManagers().subscribe({
      next: (data) => this.managersList = data,
      error: () => console.error('Failed to load managers')
    });
  }
  startCreate() {
    this.userForm.reset({ roleId: 3 });
    this.addClientModal.nativeElement.showModal();
  }
  cancelCreate() {
    this.userForm.reset();
    this.addClientModal.nativeElement.close();
  }
  saveUser() {
    if (this.userForm.invalid) {
      this.toastr.warning('Please fill all required fields correctly.', 'Validation');
      this.userForm.markAllAsTouched();
      return;
    }
    const formValue = this.userForm.value;
    const roleId = Number(formValue.roleId);
    if (roleId === 3 && !formValue.managerId) {
      this.toastr.warning('Please select a manager for this user.', 'Validation');
      return;
    }
    const newUser: User = {
      name: formValue.name,
      email: formValue.email,
      passwordHash: formValue.passwordHash,
      roleId: roleId,
      managerId: roleId === 3 ? Number(formValue.managerId) : undefined,
      isActive: true
    };
    this.userService.createUser(newUser).subscribe({
      next: () => {
        this.toastr.success('Employee created successfully!', 'Success');
        this.addClientModal.nativeElement.close();
        this.loadUsers();
        this.loadManagers();
      },
      error: (err) => {
        this.toastr.error(err.error?.message || 'Failed to create user.', 'Error');
      }
    });
  }
  @ViewChild('deleteUserModal') deleteUserModal!: ElementRef<HTMLDialogElement>;
  userToDelete: number | null = null;
  confirmDeleteUser(id: number | undefined) {
    if (id) {
      this.userToDelete = id;
      this.deleteUserModal.nativeElement.showModal();
    }
  }
  executeDeleteUser() {
    if (this.userToDelete) {
      this.userService.deleteUser(this.userToDelete).subscribe(() => {
        this.toastr.success('User deleted successfully.', 'Success');
        this.deleteUserModal.nativeElement.close();
        this.userToDelete = null;
        this.loadUsers();
      });
    }
  }
}
