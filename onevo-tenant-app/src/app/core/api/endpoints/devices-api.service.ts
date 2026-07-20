import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

export interface PairConfirmInfo {
  deviceName: string;
  requestedAt: string;
}

@Injectable({ providedIn: 'root' })
export class DevicesApiService {
  private http = inject(HttpClient);

  getConfirmationInfo(code: string): Observable<PairConfirmInfo> {
    return this.http.get<PairConfirmInfo>(`/api/v1/devices/confirm?code=${encodeURIComponent(code)}`);
  }

  confirm(userCode: string): Observable<void> {
    return this.http.post<void>('/api/v1/devices/confirm', { userCode });
  }

  decline(userCode: string): Observable<void> {
    return this.http.post<void>('/api/v1/devices/confirm/decline', { userCode });
  }
}
