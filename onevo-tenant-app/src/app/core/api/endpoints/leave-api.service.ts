import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class LeaveApiService {
  private http = inject(HttpClient);

  listRequests() {
    return this.http.get('/api/v1/leave/requests');
  }
}
