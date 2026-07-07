import { Component, inject } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { SubSidebarComponent } from '../../shared/ui/sub-sidebar/sub-sidebar.component';
import { PermissionService } from '../../core/auth/permission.service';

@Component({
  selector: 'app-people-shell',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive, SubSidebarComponent],
  template: `
    <div class="section-shell">
      <ov-sub-sidebar title="People">
        <a routerLink="/people" routerLinkActive="active" [routerLinkActiveOptions]="{exact: true}" *ngIf="perm.hasPermission('employees:read')">
          <svg viewBox="0 0 24 24"><path d="M16 11c1.66 0 2.99-1.34 2.99-3S17.66 5 16 5c-1.66 0-3 1.34-3 3s1.34 3 3 3zm-8 0c1.66 0 2.99-1.34 2.99-3S9.66 5 8 5C6.34 5 5 6.34 5 8s1.34 3 3 3zm0 2c-2.33 0-7 1.17-7 3.5V19h14v-2.5c0-2.33-4.67-3.5-7-3.5zm8 0c-.29 0-.62.02-.97.05 1.16.84 1.97 1.97 1.97 3.45V19h6v-2.5c0-2.33-4.67-3.5-7-3.5z"/></svg>
          Employees
        </a>
        <a routerLink="/people/onboarding" routerLinkActive="active" *ngIf="perm.hasPermission('employees:write')">
          <svg viewBox="0 0 24 24"><path d="M15 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm-9-2V7H4v3H1v2h3v3h2v-3h3v-2H6zm9 4c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z"/></svg>
          Onboarding
        </a>
        <a routerLink="/people/offboarding" routerLinkActive="active" *ngIf="perm.hasPermission('employees:write')">
          <svg viewBox="0 0 24 24"><path d="M15 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4zM8.5 7h-6v2h6V7z"/></svg>
          Offboarding
        </a>
        <a routerLink="/people/checklist" routerLinkActive="active" *ngIf="perm.hasAnyPermission(['employees:read', 'employees:write'])">
          <svg viewBox="0 0 24 24"><path d="M19 3H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm-9 14l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z"/></svg>
          Checklist
        </a>
      </ov-sub-sidebar>
      <div class="section-content">
        <router-outlet></router-outlet>
      </div>
    </div>
  `,
  styles: [`
    .section-shell {
      display: flex;
      width: 100%;
      height: 100%;
    }
    .section-content {
      flex: 1;
      height: 100%;
      overflow: hidden; /* router-outlet children should handle scroll */
      background: var(--content-bg);
    }
  `]
})
export class PeopleShellComponent {
  perm = inject(PermissionService);
}
