import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { PermissionService } from '../auth/permission.service';

export const featureGuard = (feature: string): CanActivateFn => () => {
  const permissions = inject(PermissionService);
  const router = inject(Router);
  return permissions.hasFeature(feature) ? true : router.createUrlTree(['/403']);
};
