import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  RoleAdminDto,
  PermissionCatalogEntryDto,
  TenantUserDto,
  OverrideDto,
  ModuleEntitlementViewDto,
  CreateRoleRequestDto,
  SetRolePermissionsRequestDto,
  AssignUserRoleRequestDto,
  OverrideRequestDto
} from '../../../../shared/models/role-admin.model';

@Injectable({
  providedIn: 'root'
})
export class RolesAdminApiService {
  private http = inject(HttpClient);
  private apiBase = environment.apiBaseUrl;

  listRoles(): Observable<RoleAdminDto[]> {
    return this.http.get<RoleAdminDto[]>(`${this.apiBase}/roles`);
  }

  listUsers(): Observable<TenantUserDto[]> {
    return this.http.get<TenantUserDto[]>(`${this.apiBase}/roles/users`);
  }

  createRole(request: CreateRoleRequestDto): Observable<RoleAdminDto> {
    return this.http.post<RoleAdminDto>(`${this.apiBase}/roles`, request);
  }

  setRolePermissions(roleId: string, request: SetRolePermissionsRequestDto): Observable<void> {
    return this.http.put<void>(`${this.apiBase}/roles/${roleId}/permissions`, request);
  }

  assignUserRole(request: AssignUserRoleRequestDto): Observable<void> {
    return this.http.post<void>(`${this.apiBase}/roles/assignments`, request);
  }

  listPermissionCatalog(): Observable<PermissionCatalogEntryDto[]> {
    return this.http.get<PermissionCatalogEntryDto[]>(`${this.apiBase}/permissions/catalog`);
  }

  listOverrides(userId: string): Observable<OverrideDto[]> {
    return this.http.get<OverrideDto[]>(`${this.apiBase}/permissions/overrides/${userId}`);
  }

  createOverride(request: OverrideRequestDto): Observable<OverrideDto> {
    return this.http.post<OverrideDto>(`${this.apiBase}/permissions/overrides`, request);
  }

  revokeOverride(overrideId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiBase}/permissions/overrides/${overrideId}`);
  }

  listEntitlements(): Observable<ModuleEntitlementViewDto[]> {
    return this.http.get<ModuleEntitlementViewDto[]>(`${this.apiBase}/tenant/entitlements`);
  }

  setModuleEntitlement(moduleCatalogId: string, isEnabled: boolean): Observable<void> {
    return this.http.put<void>(`${this.apiBase}/tenant/modules/${moduleCatalogId}/entitlement`, { id: moduleCatalogId, isEnabled });
  }

  setFeatureEntitlement(moduleFeatureId: string, isEnabled: boolean): Observable<void> {
    return this.http.put<void>(`${this.apiBase}/tenant/features/${moduleFeatureId}/entitlement`, { id: moduleFeatureId, isEnabled });
  }
}
