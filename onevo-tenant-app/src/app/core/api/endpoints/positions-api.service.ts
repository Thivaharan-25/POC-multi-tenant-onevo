import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class PositionsApiService {
  private http = inject(HttpClient);

  list(legalEntityId: string) {
    return this.http.get('/api/v1/org/positions', { params: { legalEntityId } });
  }
}
