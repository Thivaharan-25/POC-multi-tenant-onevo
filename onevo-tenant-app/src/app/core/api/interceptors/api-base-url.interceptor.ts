import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { API_BASE_URL } from '../api-base-url.token';

export const apiBaseUrlInterceptor: HttpInterceptorFn = (req, next) => {
  const apiBaseUrl = inject(API_BASE_URL);

  if (req.url.startsWith('http://') || req.url.startsWith('https://')) {
    return next(req);
  }

  if (req.url.startsWith('/api/') || req.url.startsWith('/admin/') || req.url.startsWith('api/') || req.url.startsWith('admin/')) {
    const normalizedBaseUrl = apiBaseUrl.endsWith('/') ? apiBaseUrl.slice(0, -1) : apiBaseUrl;
    const normalizedPath = req.url.startsWith('/') ? req.url : '/' + req.url;
    const newUrl = `${normalizedBaseUrl}${normalizedPath}`;
    return next(req.clone({ url: newUrl }));
  }

  return next(req);
};
