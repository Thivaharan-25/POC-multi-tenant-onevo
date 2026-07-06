import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RolesAdminApiService } from '../../../core/api/endpoints/roles-admin-api.service';
import { AppContextStore } from '../../../core/context/app-context.store';
import { AuthService } from '../../../core/auth/auth.service';
import {
  RoleAdminDto,
  PermissionCatalogEntryDto,
  TenantUserDto,
  OverrideDto,
  ModuleEntitlementViewDto,
  FeatureEntitlementViewDto
} from '../../../../shared/models/role-admin.model';

@Component({
  selector: 'app-roles-permissions',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './roles-permissions.component.html'
})
export class RolesPermissionsComponent implements OnInit {
  private api = inject(RolesAdminApiService);
  private auth = inject(AuthService);
  public appContext = inject(AppContextStore);

  roles: RoleAdminDto[] = [];
  users: TenantUserDto[] = [];
  catalog: PermissionCatalogEntryDto[] = [];
  overrides: OverrideDto[] = [];
  entitlements: ModuleEntitlementViewDto[] = [];

  // UI state
  activeTab = 'roles';
  selectedUserId: string | null = null;
  newRoleName = '';
  newRoleDesc = '';
  entitlementError = '';

  // Assignment selections
  selectedRoleIdForAssignment = '';
  selectedUserIdForAssignment = '';

  // Override creation
  overridePermissionId = '';
  grantType: 'grant' | 'revoke' = 'grant';

  ngOnInit() {
    this.loadCatalog();
    this.loadRoles();
    this.loadUsers();
    this.loadEntitlements();
  }

  loadCatalog() {
    this.api.listPermissionCatalog().subscribe(c => this.catalog = c);
  }

  loadRoles() {
    this.api.listRoles().subscribe(r => this.roles = r);
  }

  loadUsers() {
    this.api.listUsers().subscribe(u => this.users = u);
  }

  loadEntitlements() {
    this.api.listEntitlements().subscribe(e => this.entitlements = e);
  }

  loadOverrides(userId: string) {
    this.selectedUserId = userId;
    this.api.listOverrides(userId).subscribe(o => this.overrides = o);
  }

  createRole() {
    if (!this.newRoleName) return;
    this.api.createRole({ name: this.newRoleName, description: this.newRoleDesc }).subscribe(r => {
      this.roles.push(r);
      this.newRoleName = '';
      this.newRoleDesc = '';
    });
  }

  roleHasPermission(role: RoleAdminDto, permissionId: string): boolean {
    return role.permissionIds.includes(permissionId);
  }

  toggleRolePermission(role: RoleAdminDto, permissionId: string) {
    const current = new Set<string>(role.permissionIds);
    if (current.has(permissionId)) {
      current.delete(permissionId);
    } else {
      current.add(permissionId);
    }
    this.api.setRolePermissions(role.id, { permissionIds: Array.from(current) }).subscribe({
      next: () => this.loadRoles(),
      error: () => this.loadRoles()
    });
  }

  assignRoleToUser() {
    if (!this.selectedRoleIdForAssignment || !this.selectedUserIdForAssignment) return;
    this.api.assignUserRole({ userId: this.selectedUserIdForAssignment, roleId: this.selectedRoleIdForAssignment }).subscribe(() => {
      alert('Role assigned');
      this.loadUsers();
    });
  }

  createOverride() {
    if (!this.selectedUserId || !this.overridePermissionId) return;
    this.api.createOverride({
      userId: this.selectedUserId,
      permissionId: this.overridePermissionId,
      grantType: this.grantType,
      reason: 'Admin override',
      validFrom: null,
      expiresAt: null
    }).subscribe(() => {
      this.loadOverrides(this.selectedUserId!);
    });
  }

  revokeOverride(overrideId: string) {
    this.api.revokeOverride(overrideId).subscribe(() => {
      if (this.selectedUserId) {
        this.loadOverrides(this.selectedUserId);
      }
    });
  }

  toggleModule(module: ModuleEntitlementViewDto) {
    this.entitlementError = '';
    this.api.setModuleEntitlement(module.moduleCatalogId, !module.isEnabled).subscribe({
      next: () => this.refreshAfterEntitlementChange(),
      error: err => this.entitlementError = err?.error?.error ?? 'Failed to toggle module.'
    });
  }

  toggleFeature(feature: FeatureEntitlementViewDto) {
    this.entitlementError = '';
    this.api.setFeatureEntitlement(feature.moduleFeatureId, !feature.isEnabled).subscribe({
      next: () => this.refreshAfterEntitlementChange(),
      error: err => this.entitlementError = err?.error?.error ?? 'Failed to toggle feature.'
    });
  }

  // Entitlement changes ripple into the session (activeModules/activeFeatures),
  // the filtered permission catalog, and role assignability — reload all of them.
  private refreshAfterEntitlementChange() {
    this.loadEntitlements();
    this.auth.initializeSession().subscribe();
    this.loadCatalog();
    this.loadRoles();
  }
}
