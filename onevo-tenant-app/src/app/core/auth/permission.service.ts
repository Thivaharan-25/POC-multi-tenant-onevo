import { Injectable, inject } from '@angular/core';
import { AppContextStore } from '../context/app-context.store';

@Injectable({ providedIn: 'root' })
export class PermissionService {
  private appContext = inject(AppContextStore);

  hasPermission(permission: string): boolean {
    return this.appContext.permissions().includes(permission);
  }

  hasFeature(feature: string): boolean {
    return this.appContext.activeFeatures().includes(feature);
  }
}
