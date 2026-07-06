import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { PermissionService } from '../auth/permission.service';

export const permissionGuard = (permission: string): CanActivateFn => () => {
  const permissions = inject(PermissionService);
  const router = inject(Router);
  return permissions.hasPermission(permission)() ? true : router.createUrlTree(['/403']);
};
