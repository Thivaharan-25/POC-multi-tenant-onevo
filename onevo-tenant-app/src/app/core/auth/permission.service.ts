import { Injectable, inject } from '@angular/core';
import { AppContextStore } from '../context/app-context.store';

@Injectable({ providedIn: 'root' })
export class PermissionService {
  private appContext = inject(AppContextStore);

  hasPermission(permission: string): boolean {
    return this.appContext.permissions().includes(permission);
  }

  hasAnyPermission(permissions: string[]): boolean {
    if (!permissions || permissions.length === 0) return true;
    return permissions.some(p => this.hasPermission(p));
  }

  hasFeature(feature: string): boolean {
    return this.appContext.activeFeatures().includes(feature);
  }

  canShowNavItem(item: { permission?: string; permissions?: string[]; feature?: string }): boolean {
    if (item.feature && !this.hasFeature(item.feature)) return false;
    
    if (item.permissions && item.permissions.length > 0) {
      return this.hasAnyPermission(item.permissions);
    }
    
    if (item.permission) {
      return this.hasPermission(item.permission);
    }
    
    return true; // No restrictions
  }
}
