import { Component, inject } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { SubSidebarComponent } from '../../shared/ui/sub-sidebar/sub-sidebar.component';
import { PermissionService } from '../../core/auth/permission.service';

@Component({
  selector: 'app-settings-shell',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive, SubSidebarComponent],
  template: `
    <div class="section-shell">
      <ov-sub-sidebar title="Settings">
        <a routerLink="/settings/appearance" routerLinkActive="active">
          <svg viewBox="0 0 24 24"><path d="M22.7 19l-9.1-9.1c.9-2.3.4-5-1.5-6.9-2-2-5-2.4-7.4-1.3L9 6 6 9 1.6 4.7C.4 7.1.9 10.1 2.9 12.1c1.9 1.9 4.6 2.4 6.9 1.5l9.1 9.1c.4.4 1 .4 1.4 0l2.3-2.3c.5-.4.5-1.1.1-1.4z"/></svg>
          Appearance
        </a>
        <a routerLink="/settings/roles-permissions" routerLinkActive="active" *ngIf="perm.hasPermission('roles:manage')">
          <svg viewBox="0 0 24 24"><path d="M12 1L3 5v6c0 5.55 3.84 10.74 9 12 5.16-1.26 9-6.45 9-12V5l-9-4zm0 10.99h7c-.53 4.12-3.28 7.79-7 8.94V12H5V6.3l7-3.11v8.8z"/></svg>
          Roles & Permissions
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
      overflow: hidden;
      background: var(--content-bg);
    }
  `]
})
export class SettingsShellComponent {
  perm = inject(PermissionService);
}
