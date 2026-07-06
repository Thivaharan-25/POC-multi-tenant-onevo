import { HttpInterceptorFn } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

export const tenantContextInterceptor: HttpInterceptorFn = (req, next) => {
  const hostname = window.location.hostname;
  const isLocalhost = hostname === 'localhost' || hostname === '127.0.0.1';
  const domain = isLocalhost ? (environment.tenantDomain || 'acme.test') : hostname;
  return next(req.clone({ setHeaders: { 'X-Tenant-Domain': domain } }));
};
