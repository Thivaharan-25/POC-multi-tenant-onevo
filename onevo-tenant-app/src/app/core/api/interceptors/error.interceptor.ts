import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

import { HttpContextToken } from '@angular/common/http';

export const BYPASS_GLOBAL_ERROR_HANDLING = new HttpContextToken<boolean>(() => false);

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const bypass = req.context.get(BYPASS_GLOBAL_ERROR_HANDLING);
      
      if (!bypass) {
        if (error.status === 401) {
          router.navigate(['/login']);
        }
        if (error.status === 403) {
          router.navigate(['/403']);
        }
      }
      return throwError(() => error);
    })
  );
};
