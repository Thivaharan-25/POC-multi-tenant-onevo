import { Injectable, computed, inject } from '@angular/core';
import { AppContextStore } from '../context/app-context.store';

@Injectable({ providedIn: 'root' })
export class PermissionService {
  private appContext = inject(AppContextStore);

  hasPermission(permission: string) {
    return computed(() => this.appContext.permissions().includes(permission));
  }

  hasFeature(feature: string) {
    return computed(() => this.appContext.activeFeatures().includes(feature));
  }
}
