import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { LegalEntityContextStore } from '../../context/legal-entity-context.store';

export const legalEntityContextInterceptor: HttpInterceptorFn = (req, next) => {
  const legalEntityId = inject(LegalEntityContextStore).legalEntityId();
  return next(legalEntityId ? req.clone({ setHeaders: { 'X-Legal-Entity-Id': legalEntityId } }) : req);
};
