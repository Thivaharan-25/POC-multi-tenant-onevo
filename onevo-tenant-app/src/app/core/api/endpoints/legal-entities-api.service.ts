import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class LegalEntitiesApiService {
  private http = inject(HttpClient);

  list() {
    return this.http.get('/api/v1/org/legal-entities');
  }
}
