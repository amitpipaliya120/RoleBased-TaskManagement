import { Routes } from '@angular/router';
import { LoginComponent } from './features/login/login.component';
import { RegisterComponent } from './features/register/register.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { TaskListComponent } from './features/tasks/task-list/task-list.component';
import { TeamListComponent } from './features/teams/team-list/team-list.component';
import { UserManagementComponent } from './features/admin/user-management/user-management.component';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';
export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'dashboard', component: DashboardComponent, canActivate: [authGuard] },
  { path: 'tasks', component: TaskListComponent, canActivate: [authGuard] },
  { 
    path: 'teams', 
    component: TeamListComponent, 
    canActivate: [authGuard, roleGuard],
    data: { expectedRoles: ['Admin', 'Manager'] }
  },
  { 
    path: 'admin/users', 
    component: UserManagementComponent, 
    canActivate: [authGuard, roleGuard],
    data: { expectedRoles: ['Admin', 'Manager'] }
  }
];
