import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NotificationPanelComponent } from '../notification-panel/notification-panel.component';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { TopbarComponent } from '../topbar/topbar.component';
import { ThemeService } from '../../core/theme/theme.service';

@Component({
  selector: 'app-tenant-shell',
  standalone: true,
  imports: [RouterOutlet, SidebarComponent, TopbarComponent, NotificationPanelComponent],
  template: `
    <div class="app-shell">
      <div class="shell-sidebar-container">
        <app-sidebar />
      </div>
      <div class="shell-main-content">
        <div class="shell-topbar-container">
          <app-topbar />
        </div>
        <div class="shell-router-outlet">
          <router-outlet />
        </div>
      </div>
      <app-notification-panel />
    </div>
  `
})
export class TenantShellComponent {
  private themeService = inject(ThemeService);
}
