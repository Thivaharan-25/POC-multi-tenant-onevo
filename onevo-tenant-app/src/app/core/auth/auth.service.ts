import { Injectable, computed, inject, signal } from '@angular/core';
import { catchError, map, of, tap } from 'rxjs';
import { AuthApiService } from '../api/endpoints/auth-api.service';
import { AppContextStore } from '../context/app-context.store';
import { TenantContextStore } from '../context/tenant-context.store';
import { SessionDto, SessionUser } from '../../shared/models/session.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private authApi = inject(AuthApiService);
  private appContext = inject(AppContextStore);
  private tenantContext = inject(TenantContextStore);

  private userState = signal<SessionUser | null>(null);
  private initializedState = signal(false);
  
  sessionApiStatus = signal<{ status: 'idle' | 'loading' | 'success' | 'error', statusCode?: number, error?: string }>({ status: 'idle' });

  user = this.userState.asReadonly();
  isInitialized = this.initializedState.asReadonly();
  isAuthenticated = computed(() => this.userState() !== null);

  initializeSession() {
    this.sessionApiStatus.set({ status: 'loading' });
    return this.authApi.session().pipe(
      tap(session => {
        if (!session || !session.user) {
          this.clear();
          this.sessionApiStatus.set({ status: 'success' });
        } else {
          this.setSession(session);
        }
      }),
      map(() => true),
      catchError(err => {
        this.clear();
        this.sessionApiStatus.set({ 
          status: 'error', 
          statusCode: err.status, 
          error: err.error?.error || err.message || 'Session verification failed' 
        });
        this.initializedState.set(true);
        return of(false);
      })
    );
  }

  setSession(session: SessionDto): void {
    this.userState.set(session.user);
    this.appContext.setSession(session);
    this.tenantContext.setTenantId(session.user?.tenantId ?? null);
    this.initializedState.set(true);
    this.sessionApiStatus.set({ status: 'success' });
  }

  clear(): void {
    this.userState.set(null);
    this.appContext.clear();
    this.tenantContext.setTenantId(null);
    this.sessionApiStatus.set({ status: 'idle' });
  }
}
