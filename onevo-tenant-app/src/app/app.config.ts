import { ApplicationConfig, provideZonelessChangeDetection } from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { routes } from './app.routes';
import { API_BASE_URL } from './core/api/api-base-url.token';
import { apiBaseUrlInterceptor } from './core/api/interceptors/api-base-url.interceptor';
import { correlationInterceptor } from './core/api/interceptors/correlation.interceptor';
import { credentialsInterceptor } from './core/api/interceptors/credentials.interceptor';
import { csrfInterceptor } from './core/api/interceptors/csrf.interceptor';
import { errorInterceptor } from './core/api/interceptors/error.interceptor';
import { legalEntityContextInterceptor } from './core/api/interceptors/legal-entity-context.interceptor';
import { tenantContextInterceptor } from './core/api/interceptors/tenant-context.interceptor';
import { initializeSessionProvider } from './core/auth/session-initializer';
import { environment } from '../environments/environment';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZonelessChangeDetection(),
    provideRouter(routes, withComponentInputBinding()),
    provideAnimationsAsync(),
    provideHttpClient(withInterceptors([
      apiBaseUrlInterceptor,
      credentialsInterceptor,
      csrfInterceptor,
      tenantContextInterceptor,
      legalEntityContextInterceptor,
      correlationInterceptor,
      errorInterceptor
    ])),
    { provide: API_BASE_URL, useValue: environment.apiBaseUrl },
    initializeSessionProvider
  ]
};
