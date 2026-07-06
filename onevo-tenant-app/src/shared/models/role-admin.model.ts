export interface RoleAdminDto {
  id: string;
  name: string;
  description: string;
  isSystemRole: boolean;
  permissionIds: string[];
  permissionCodes: string[];
}

export interface PermissionCatalogEntryDto {
  id: string;
  code: string;
  description: string;
  module: string | null;
  featureKey: string | null;
}

export interface TenantUserDto {
  id: string;
  email: string;
  displayName: string;
}

export interface OverrideDto {
  id: string;
  userId: string;
  permissionId: string;
  permissionCode: string;
  grantType: 'grant' | 'revoke';
  reason: string;
  validFrom: string | null;
  expiresAt: string | null;
  grantedBy: string;
  createdAt: string;
}

export interface FeatureEntitlementViewDto {
  moduleFeatureId: string;
  featureKey: string;
  isEnabled: boolean;
}

export interface ModuleEntitlementViewDto {
  moduleCatalogId: string;
  moduleKey: string;
  displayName: string;
  isFoundation: boolean;
  isEnabled: boolean;
  features: FeatureEntitlementViewDto[];
}

export interface CreateRoleRequestDto {
  name: string;
  description: string;
}

export interface SetRolePermissionsRequestDto {
  permissionIds: string[];
}

export interface AssignUserRoleRequestDto {
  userId: string;
  roleId: string;
}

export interface OverrideRequestDto {
  userId: string;
  permissionId: string;
  grantType: 'grant' | 'revoke';
  reason: string;
  validFrom: string | null;
  expiresAt: string | null;
}
