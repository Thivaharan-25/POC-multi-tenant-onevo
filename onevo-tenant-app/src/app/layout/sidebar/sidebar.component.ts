import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  template: `
    <nav>
      <a routerLink="/dashboard" routerLinkActive="active">Dashboard</a>
      <a routerLink="/my-profile" routerLinkActive="active">My profile</a>
      <a routerLink="/people" routerLinkActive="active">People</a>
      <a routerLink="/time-off" routerLinkActive="active">Time Off</a>
      <a routerLink="/time-attendance" routerLinkActive="active">Time & Attendance</a>
      <a routerLink="/calendar" routerLinkActive="active">Calendar</a>
      <a routerLink="/inbox" routerLinkActive="active">Inbox</a>
      <a routerLink="/monitoring" routerLinkActive="active">Monitoring</a>
      <a routerLink="/organization/legal-entities" routerLinkActive="active">Organization</a>
      <a routerLink="/settings/roles-permissions" routerLinkActive="active">Settings</a>
      <a routerLink="/reports" routerLinkActive="active">Reports</a>
    </nav>
  `
})
export class SidebarComponent {}
