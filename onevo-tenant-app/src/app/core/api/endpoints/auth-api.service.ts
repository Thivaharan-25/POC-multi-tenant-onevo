import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { SessionDto } from '../../../shared/models/session.model';

@Injectable({ providedIn: 'root' })
export class AuthApiService {
  private http = inject(HttpClient);

  session(): Observable<SessionDto> {
    return this.http.get<SessionDto>('/api/v1/auth/session');
  }

  login(email: string, password: string): Observable<SessionDto> {
    return this.http.post<SessionDto>('/api/v1/auth/login', { email, password });
  }

  refresh(): Observable<SessionDto> {
    return this.http.post<SessionDto>('/api/v1/auth/refresh', {});
  }

  logout(): Observable<void> {
    return this.http.post<void>('/api/v1/auth/logout', {});
  }

  validateInvitation(token: string): Observable<any> {
    return this.http.get<any>(`/api/v1/auth/invitations/validate?token=${encodeURIComponent(token)}`);
  }

  acceptInvitation(request: any): Observable<SessionDto> {
    return this.http.post<SessionDto>('/api/v1/auth/invitations/accept', request);
  }
}
