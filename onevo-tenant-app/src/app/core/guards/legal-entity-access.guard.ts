import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { LegalEntityContextStore } from '../context/legal-entity-context.store';

export const legalEntityAccessGuard: CanActivateFn = () => {
  const context = inject(LegalEntityContextStore);
  return context.legalEntityId() !== null || true;
};
