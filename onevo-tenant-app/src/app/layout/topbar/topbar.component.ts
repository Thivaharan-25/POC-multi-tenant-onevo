import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import { AuthApiService } from '../../core/api/endpoints/auth-api.service';

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [CommonModule],
  template: `
    <header class="topbar">
      <div class="logo-area">
        <span class="brand">ONEVO</span>
        <span class="badge">Tenant Portal</span>
      </div>
      <div class="user-area" *ngIf="auth.user() as user">
        <span class="user-email">{{ user.email }}</span>
        <button class="logout-btn" (click)="logout()">
          <svg class="logout-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
          </svg>
          Logout
        </button>
      </div>
    </header>
  `,
  styles: [`
    .topbar {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 0.75rem 2rem;
      background: #0f172a;
      border-bottom: 1px solid rgba(255, 255, 255, 0.08);
      height: 60px;
    }
    .logo-area {
      display: flex;
      align-items: center;
      gap: 0.75rem;
    }
    .brand {
      font-weight: 800;
      font-size: 1.25rem;
      letter-spacing: -0.03em;
      color: #38bdf8;
    }
    .badge {
      font-size: 0.7rem;
      background: rgba(56, 189, 248, 0.1);
      color: #38bdf8;
      border: 1px solid rgba(56, 189, 248, 0.2);
      padding: 0.1rem 0.4rem;
      border-radius: 4px;
      font-weight: 600;
      text-transform: uppercase;
    }
    .user-area {
      display: flex;
      align-items: center;
      gap: 1rem;
    }
    .user-email {
      font-size: 0.85rem;
      color: #94a3b8;
    }
    .logout-btn {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      background: rgba(239, 68, 68, 0.1);
      border: 1px solid rgba(239, 68, 68, 0.2);
      color: #fca5a5;
      padding: 0.4rem 0.8rem;
      border-radius: 6px;
      font-size: 0.85rem;
      font-weight: 500;
      cursor: pointer;
      transition: all 0.2s;
    }
    .logout-btn:hover {
      background: rgba(239, 68, 68, 0.2);
      color: #ef4444;
      border-color: rgba(239, 68, 68, 0.3);
    }
    .logout-icon {
      width: 16px;
      height: 16px;
    }
  `]
})
export class TopbarComponent {
  auth = inject(AuthService);
  private authApi = inject(AuthApiService);
  private router = inject(Router);

  logout(): void {
    this.authApi.logout().subscribe({
      next: () => {
        this.auth.clear();
        this.router.navigate(['/login']);
      },
      error: () => {
        this.auth.clear();
        this.router.navigate(['/login']);
      }
    });
  }
}
