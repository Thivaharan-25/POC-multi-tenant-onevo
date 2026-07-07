import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { guestGuard } from './core/guards/guest.guard';
import { legalEntityAccessGuard } from './core/guards/legal-entity-access.guard';
import { permissionGuard } from './core/guards/permission.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./layout/auth-layout/auth-layout.component').then(m => m.AuthLayoutComponent),
    canActivate: [guestGuard],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'login' },
      { path: 'login', loadComponent: () => import('./features/auth/login.component').then(m => m.LoginComponent) },
      { path: 'accept-invite', loadComponent: () => import('./features/auth/accept-invite/accept-invite.component').then(m => m.AcceptInviteComponent) }
    ]
  },
  {
    path: '',
    loadComponent: () => import('./layout/tenant-shell/tenant-shell.component').then(m => m.TenantShellComponent),
    canActivate: [authGuard, legalEntityAccessGuard],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
      { path: 'dashboard', loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent) },
      { path: 'my-profile', loadComponent: () => import('./features/my-profile/my-profile.component').then(m => m.MyProfileComponent) },
      { path: 'people', canActivate: [permissionGuard('employees:read')], loadComponent: () => import('./features/people/people.component').then(m => m.PeopleComponent) },
      { path: 'people/onboarding', canActivate: [permissionGuard('employees:write')], loadComponent: () => import('./features/people/onboarding/onboarding.component').then(m => m.OnboardingComponent) },
      { path: 'time-off', canActivate: [permissionGuard('leave:create')], loadComponent: () => import('./features/leave/leave.component').then(m => m.LeaveComponent) },
      { path: 'time-attendance', loadComponent: () => import('./features/attendance/attendance.component').then(m => m.AttendanceComponent) },
      { path: 'calendar', canActivate: [permissionGuard('calendar:read')], loadComponent: () => import('./features/calendar/calendar.component').then(m => m.CalendarComponent) },
      { path: 'inbox', loadComponent: () => import('./features/notifications/notifications.component').then(m => m.NotificationsComponent) },
      { path: 'monitoring', loadComponent: () => import('./features/monitoring/monitoring.component').then(m => m.MonitoringComponent) },
      { path: 'organization/legal-entities', canActivate: [permissionGuard('org:legal-entities:manage')], loadComponent: () => import('./features/organization/legal-entities/legal-entities.component').then(m => m.LegalEntitiesComponent) },
      { path: 'organization/departments', canActivate: [permissionGuard('org:departments:manage')], loadComponent: () => import('./features/organization/departments/departments.component').then(m => m.DepartmentsComponent) },
      { path: 'organization/positions', canActivate: [permissionGuard('org:positions:manage')], loadComponent: () => import('./features/organization/positions/positions.component').then(m => m.PositionsComponent) },
      { path: 'settings/roles-permissions', canActivate: [permissionGuard('roles:manage')], loadComponent: () => import('./features/settings/roles-permissions/roles-permissions.component').then(m => m.RolesPermissionsComponent) },
      { path: 'reports', loadComponent: () => import('./features/reports/reports.component').then(m => m.ReportsComponent) }
    ]
  },
  { path: '403', loadComponent: () => import('./features/errors/forbidden.component').then(m => m.ForbiddenComponent) },
  { path: '**', loadComponent: () => import('./features/errors/not-found.component').then(m => m.NotFoundComponent) }
];
