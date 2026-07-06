import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NotificationPanelComponent } from '../notification-panel/notification-panel.component';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { TopbarComponent } from '../topbar/topbar.component';

@Component({
  selector: 'app-tenant-shell',
  standalone: true,
  imports: [RouterOutlet, SidebarComponent, TopbarComponent, NotificationPanelComponent],
  template: `
    <app-topbar />
    <div class="app-shell">
      <app-sidebar />
      <main><router-outlet /></main>
      <app-notification-panel />
    </div>
  `
})
export class TenantShellComponent {}
