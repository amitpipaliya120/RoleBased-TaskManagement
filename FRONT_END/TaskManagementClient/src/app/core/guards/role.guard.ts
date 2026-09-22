import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
export const roleGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const expectedRoles = route.data['expectedRoles'] as string[];
  const currentRole = authService.getRole();
  if (!currentRole || !expectedRoles.includes(currentRole)) {
    router.navigate(['/dashboard']);
    return false;
  }
  return true;
};
