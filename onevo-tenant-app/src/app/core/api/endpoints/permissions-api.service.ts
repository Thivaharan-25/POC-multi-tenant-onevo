import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class PermissionsApiService {
  private http = inject(HttpClient);

  catalog() {
    return this.http.get('/api/v1/permissions/catalog');
  }
}
