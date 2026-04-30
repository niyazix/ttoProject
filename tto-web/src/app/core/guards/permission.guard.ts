import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const permissionGuard: CanActivateFn = (route) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const requiredPermission: string = route.data?.['permission'];
  if (!requiredPermission) return true;

  if (!auth.isLoggedIn()) {
    router.navigate(['/admin/login']);
    return false;
  }

  if (auth.hasPermission(requiredPermission)) return true;

  router.navigate(['/admin/unauthorized']);
  return false;
};
