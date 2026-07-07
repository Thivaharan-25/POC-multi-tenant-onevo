import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

export interface EmployeeListItem {
  id: string;
  employeeNumber: string;
  firstName: string;
  lastName: string;
  workEmail: string;
  status: string;
  hireDate: string;
  legalEntityId: string;
  departmentId: string | null;
  departmentName: string | null;
  currentPositionId: string | null;
  positionName: string | null;
}

export interface EmployeeListFilters {
  search?: string;
  status?: string;
  departmentId?: string;
  positionId?: string;
}

@Injectable({ providedIn: 'root' })
export class EmployeesApiService {
  private http = inject(HttpClient);

  list(filters: EmployeeListFilters = {}) {
    const params: Record<string, string> = {};
    if (filters.search) params['search'] = filters.search;
    if (filters.status) params['status'] = filters.status;
    if (filters.departmentId) params['departmentId'] = filters.departmentId;
    if (filters.positionId) params['positionId'] = filters.positionId;
    return this.http.get<EmployeeListItem[]>('/api/v1/employees', { params });
  }
}
