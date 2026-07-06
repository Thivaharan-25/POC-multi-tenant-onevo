import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class NotificationsApiService {
  private http = inject(HttpClient);

  inbox() {
    return this.http.get('/api/v1/notifications');
  }
}
