import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class TenantContextStore {
  private tenantIdState = signal<string | null>(null);
  tenantId = this.tenantIdState.asReadonly();

  setTenantId(tenantId: string | null): void {
    this.tenantIdState.set(tenantId);
  }
}
