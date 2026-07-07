import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { PermissionService } from '../../core/auth/permission.service';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { CompanySwitcherComponent } from './company-switcher.component';

interface NavItem {
  label: string;
  route: string;
  icon: SafeHtml;
  permission?: string;
  permissions?: string[];
  feature?: string;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, CompanySwitcherComponent],
  template: `
    <aside class="sidebar">
      <div class="logo-area" (click)="showCompanySwitcher.set(true)" style="cursor: pointer;">
        <span class="brand">OV</span>
      </div>
      
      <nav class="nav-menu">
        <ng-container *ngFor="let item of navItems">
          <a *ngIf="permissionService.canShowNavItem(item)"
             [routerLink]="item.route"
             routerLinkActive="active"
             class="nav-item">
            <span class="nav-icon" [innerHTML]="item.icon"></span>
            <span class="nav-label">{{ item.label }}</span>
          </a>
        </ng-container>
      </nav>
    </aside>

    <app-company-switcher *ngIf="showCompanySwitcher()" (close)="showCompanySwitcher.set(false)" />
  `,
  styles: [`
    .sidebar {
      width: 88px;
      height: 100%;
      background: transparent;
      display: flex;
      flex-direction: column;
      padding: 16px 0 24px 0;
      overflow-y: auto;
      align-items: center;
      /* hide scrollbar */
      -ms-overflow-style: none;
      scrollbar-width: none;
    }
    .sidebar::-webkit-scrollbar {
      display: none;
    }
    
    .logo-area {
      display: flex;
      align-items: center;
      justify-content: center;
      margin-bottom: 24px;
      width: 48px;
      height: 48px;
      border-radius: 12px;
      background: rgba(255, 255, 255, 0.1);
      color: var(--shell-fg);
    }
    
    .brand {
      font-weight: 800;
      font-size: 1.25rem;
      letter-spacing: -0.03em;
    }
    
    .nav-menu {
      display: flex;
      flex-direction: column;
      gap: 4px;
      width: 100%;
      padding: 0 10px;
    }
    
    .nav-item {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      gap: 4px;
      padding: 8px 4px;
      border-radius: 12px;
      color: var(--shell-muted-fg);
      text-decoration: none;
      transition: all 0.2s ease;
      text-align: center;
      height: 58px;
      width: 100%;
    }
    
    .nav-item:hover {
      background: rgba(128, 128, 128, 0.15);
      color: var(--shell-fg);
    }
    
    .nav-item.active {
      background: var(--shell-active-bg, rgba(128, 128, 128, 0.25));
      color: var(--shell-active-fg, var(--shell-fg));
      font-weight: 600;
    }
    
    .nav-icon {
      width: 22px;
      height: 22px;
      display: flex;
      align-items: center;
      justify-content: center;
    }
    
    .nav-icon svg {
      width: 100%;
      height: 100%;
      fill: currentColor;
    }
    
    .nav-label {
      font-size: 0.65rem;
      line-height: 1.1;
      font-weight: 500;
      max-width: 100%;
      overflow: hidden;
      text-overflow: ellipsis;
      display: -webkit-box;
      -webkit-line-clamp: 2;
      -webkit-box-orient: vertical;
    }
  `]
})
export class SidebarComponent {
  permissionService = inject(PermissionService);
  private sanitizer = inject(DomSanitizer);

  showCompanySwitcher = signal(false);

  // Define nav items with their permissions
  navItems: NavItem[] = [];

  constructor() {
    this.navItems = [
      { 
        label: 'Dashboard', 
        route: '/dashboard', 
        icon: this.sanitizer.bypassSecurityTrustHtml('<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="3" width="7" height="7"></rect><rect x="14" y="3" width="7" height="7"></rect><rect x="14" y="14" width="7" height="7"></rect><rect x="3" y="14" width="7" height="7"></rect></svg>') 
      },
      { 
        label: 'People', 
        route: '/people', 
        permissions: ['employees:read', 'employees:write'],
        icon: this.sanitizer.bypassSecurityTrustHtml('<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"></path><circle cx="9" cy="7" r="4"></circle><path d="M23 21v-2a4 4 0 0 0-3-3.87"></path><path d="M16 3.13a4 4 0 0 1 0 7.75"></path></svg>') 
      },
      { 
        label: 'Time Off', 
        route: '/time-off', 
        permissions: ['leave:create', 'leave:approve', 'time_off:read'],
        icon: this.sanitizer.bypassSecurityTrustHtml('<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="4" width="18" height="18" rx="2" ry="2"></rect><line x1="16" y1="2" x2="16" y2="6"></line><line x1="8" y1="2" x2="8" y2="6"></line><line x1="3" y1="10" x2="21" y2="10"></line><line x1="9" y1="16" x2="15" y2="16"></line></svg>') 
      },
      { 
        label: 'Attendance', 
        route: '/time-attendance', 
        icon: this.sanitizer.bypassSecurityTrustHtml('<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"></circle><polyline points="12 6 12 12 16 14"></polyline></svg>') 
      },
      { 
        label: 'Calendar', 
        route: '/calendar', 
        permission: 'calendar:read',
        icon: this.sanitizer.bypassSecurityTrustHtml('<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="4" width="18" height="18" rx="2" ry="2"></rect><line x1="16" y1="2" x2="16" y2="6"></line><line x1="8" y1="2" x2="8" y2="6"></line><line x1="3" y1="10" x2="21" y2="10"></line></svg>') 
      },
      { 
        label: 'Monitoring', 
        route: '/monitoring', 
        icon: this.sanitizer.bypassSecurityTrustHtml('<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="2" y="3" width="20" height="14" rx="2" ry="2"></rect><line x1="8" y1="21" x2="16" y2="21"></line><line x1="12" y1="17" x2="12" y2="21"></line></svg>') 
      },
      { 
        label: 'Organization', 
        route: '/organization/legal-entities', 
        permissions: ['org:legal-entities:manage', 'org:departments:manage', 'org:positions:manage'],
        icon: this.sanitizer.bypassSecurityTrustHtml('<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="4" y="2" width="16" height="20" rx="2" ry="2"></rect><path d="M9 22v-4h6v4"></path><path d="M8 6h.01"></path><path d="M16 6h.01"></path><path d="M12 6h.01"></path><path d="M12 10h.01"></path><path d="M12 14h.01"></path><path d="M16 10h.01"></path><path d="M16 14h.01"></path><path d="M8 10h.01"></path><path d="M8 14h.01"></path></svg>') 
      },
      { 
        label: 'Settings', 
        route: '/settings', 
        icon: this.sanitizer.bypassSecurityTrustHtml('<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="3"></circle><path d="M19.4 15a1.65 1.65 0 0 0 .33 1.82l.06.06a2 2 0 0 1 0 2.83 2 2 0 0 1-2.83 0l-.06-.06a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 0 1-2 2 2 2 0 0 1-2-2v-.09A1.65 1.65 0 0 0 9 19.4a1.65 1.65 0 0 0-1.82.33l-.06.06a2 2 0 0 1-2.83 0 2 2 0 0 1 0-2.83l.06-.06a1.65 1.65 0 0 0 .33-1.82 1.65 1.65 0 0 0-1.51-1H3a2 2 0 0 1-2-2 2 2 0 0 1 2-2h.09A1.65 1.65 0 0 0 4.6 9a1.65 1.65 0 0 0-.33-1.82l-.06-.06a2 2 0 0 1 0-2.83 2 2 0 0 1 2.83 0l.06.06a1.65 1.65 0 0 0 1.82.33H9a1.65 1.65 0 0 0 1-1.51V3a2 2 0 0 1 2-2 2 2 0 0 1 2 2v.09a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l.06-.06a2 2 0 0 1 2.83 0 2 2 0 0 1 0 2.83l-.06.06a1.65 1.65 0 0 0-.33 1.82V9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 0 1 2 2 2 2 0 0 1-2 2h-.09a1.65 1.65 0 0 0-1.51 1z"></path></svg>') 
      },
      { 
        label: 'Reports', 
        route: '/reports', 
        icon: this.sanitizer.bypassSecurityTrustHtml('<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21.21 15.89A10 10 0 1 1 8 2.83"></path><path d="M22 12A10 10 0 0 0 12 2v10z"></path></svg>') 
      }
    ];
  }
}
