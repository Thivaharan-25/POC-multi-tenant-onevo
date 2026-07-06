import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class AttendanceApiService {
  private http = inject(HttpClient);

  summary() {
    return this.http.get('/api/v1/attendance/summary');
  }
}
