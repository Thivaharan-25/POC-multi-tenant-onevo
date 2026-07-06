import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class AppContextApiService {
  private http = inject(HttpClient);

  get() {
    return this.http.get('/api/v1/me/app-context');
  }
}
