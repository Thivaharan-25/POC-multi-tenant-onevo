export interface SessionUser {
  id: string;
  tenantId: string;
  employeeId?: string;
  displayName: string;
  email: string;
}

export interface SessionDto {
  user: SessionUser | null;
  permissions: string[];
  activeModules: string[];
  activeFeatures: string[];
  setupComplete?: boolean;
}
