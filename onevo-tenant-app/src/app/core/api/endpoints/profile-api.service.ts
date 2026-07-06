import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ProfileApiService {
  private http = inject(HttpClient);

  getMine() {
    return this.http.get('/api/v1/profile/me');
  }
}
