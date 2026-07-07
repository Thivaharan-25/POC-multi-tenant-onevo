import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import { AuthApiService } from '../../core/api/endpoints/auth-api.service';

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <header class="topbar">
      <!-- Empty div for left space to balance flex -->
      <div class="left-spacer"></div>
      
      <div class="search-area">
        <div class="search-input-wrapper">
          <svg class="search-icon" viewBox="0 0 24 24"><path d="M15.5 14h-.79l-.28-.27C15.41 12.59 16 11.11 16 9.5 16 5.91 13.09 3 9.5 3S3 5.91 3 9.5 5.91 16 9.5 16c1.61 0 3.09-.59 4.23-1.57l.27.28v.79l5 4.99L20.49 19l-4.99-5zm-6 0C7.01 14 5 11.99 5 9.5S7.01 5 9.5 5 14 7.01 14 9.5 11.99 14 9.5 14z"/></svg>
          <input type="text" class="search-input" placeholder="Search employees, documents..." />
        </div>
      </div>
      
      <div class="right-area">
        <button class="icon-btn" routerLink="/inbox" title="Inbox">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9"></path><path d="M13.73 21a2 2 0 0 1-3.46 0"></path></svg>
        </button>
        
        <div class="user-area" *ngIf="auth.user() as user">
          <div class="profile-dropdown-container">
            <button class="avatar-btn">
              {{ (user.displayName || user.email || 'U')[0].toUpperCase() }}
            </button>
            <div class="profile-dropdown">
              <a routerLink="/my-profile" class="dropdown-item">My Profile</a>
              <button (click)="logout()" class="dropdown-item logout">Logout</button>
            </div>
          </div>
        </div>
      </div>
    </header>
  `,
  styles: [`
    :host {
      display: block;
      width: 100%;
      height: 100%;
    }
    
    .topbar {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 0 16px;
      height: 100%;
      width: 100%;
      background: transparent;
      color: var(--topbar-fg, var(--shell-fg));
    }
    
    .left-spacer {
      flex: 1;
    }
    
    .search-area {
      flex: 1;
      display: flex;
      justify-content: center;
      width: 100%;
      max-width: 400px;
    }
    
    .search-input-wrapper {
      position: relative;
      display: flex;
      align-items: center;
      width: 100%;
    }
    
    .search-icon {
      position: absolute;
      left: 12px;
      width: 16px;
      height: 16px;
      color: var(--topbar-fg, var(--shell-fg));
      opacity: 0.6;
      fill: currentColor;
    }
    
    .search-input {
      width: 100%;
      height: 36px;
      padding: 0 16px 0 36px;
      border-radius: 18px;
      border: 1px solid rgba(128, 128, 128, 0.2);
      background: rgba(128, 128, 128, 0.1);
      color: var(--topbar-fg, var(--shell-fg));
      font-size: 0.875rem;
      transition: all 0.2s;
    }
    
    .search-input::placeholder {
      color: var(--topbar-fg, var(--shell-fg));
      opacity: 0.5;
    }
    
    .search-input:focus {
      outline: none;
      background: rgba(128, 128, 128, 0.15);
      border-color: rgba(255, 255, 255, 0.3);
    }
    
    .right-area {
      flex: 1;
      display: flex;
      justify-content: flex-end;
      align-items: center;
      gap: 16px;
    }
    
    .icon-btn {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 36px;
      height: 36px;
      border-radius: 50%;
      background: transparent;
      border: none;
      color: var(--topbar-fg, var(--shell-fg));
      cursor: pointer;
      transition: all 0.2s;
      opacity: 0.8;
    }
    
    .icon-btn:hover {
      background: rgba(128, 128, 128, 0.15);
      opacity: 1;
    }
    
    .icon-btn svg {
      width: 18px;
      height: 18px;
    }
    
    .user-area {
      display: flex;
      align-items: center;
      gap: 8px;
    }
    
    .profile-dropdown-container {
      position: relative;
    }
    
    .avatar-btn {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 36px;
      height: 36px;
      border-radius: 12px;
      background: rgba(128, 128, 128, 0.2);
      border: 1px solid rgba(128, 128, 128, 0.3);
      color: var(--topbar-fg, var(--shell-fg));
      font-weight: 600;
      font-size: 1rem;
      cursor: pointer;
      transition: all 0.2s;
    }
    
    .avatar-btn:hover {
      background: rgba(128, 128, 128, 0.3);
    }
    
    .profile-dropdown {
      position: absolute;
      top: 100%;
      right: 0;
      margin-top: 8px;
      background: var(--content-bg, #ffffff);
      border: 1px solid var(--border-color, rgba(0,0,0,0.1));
      border-radius: 8px;
      box-shadow: 0 4px 12px rgba(0,0,0,0.1);
      width: 160px;
      display: flex;
      flex-direction: column;
      opacity: 0;
      visibility: hidden;
      transform: translateY(-10px);
      transition: all 0.2s;
      z-index: 100;
      overflow: hidden;
      padding: 4px 0;
    }
    
    .profile-dropdown-container:hover .profile-dropdown {
      opacity: 1;
      visibility: visible;
      transform: translateY(0);
    }
    
    .dropdown-item {
      display: block;
      padding: 10px 16px;
      color: var(--content-fg, #0f172a);
      text-decoration: none;
      background: transparent;
      border: none;
      width: 100%;
      text-align: left;
      font-size: 0.85rem;
      cursor: pointer;
      font-family: inherit;
    }
    
    .dropdown-item:hover {
      background: rgba(128, 128, 128, 0.1);
    }
    
    .dropdown-item.logout {
      color: #ef4444;
    }
    
    .dropdown-item.logout:hover {
      background: rgba(239, 68, 68, 0.1);
    }
  `]
})
export class TopbarComponent {
  auth = inject(AuthService);
  private authApi = inject(AuthApiService);
  private router = inject(Router);
  
  today = new Date();

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
