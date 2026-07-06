import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class LegalEntityContextStore {
  private legalEntityIdState = signal<string | null>(null);
  legalEntityId = this.legalEntityIdState.asReadonly();

  setLegalEntityId(legalEntityId: string | null): void {
    this.legalEntityIdState.set(legalEntityId);
  }
}
