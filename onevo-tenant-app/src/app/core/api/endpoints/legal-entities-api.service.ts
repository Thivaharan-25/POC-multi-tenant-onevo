import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class LegalEntitiesApiService {
  private http = inject(HttpClient);

  list() {
    return this.http.get<any[]>('/api/v1/org/legal-entities');
  }

  create(payload: any) {
    return this.http.post<any>('/api/v1/org/legal-entities', payload);
  }
}
