import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

export interface ReportingManagerPreview {
  hasManager: boolean;
  employeeId?: string;
  employeeName?: string;
}

@Injectable({ providedIn: 'root' })
export class PositionsApiService {
  private http = inject(HttpClient);

  list(legalEntityId: string, departmentId?: string) {
    const params: Record<string, string> = { legalEntityId };
    if (departmentId) {
      params['departmentId'] = departmentId;
    }
    return this.http.get('/api/v1/org/positions', { params });
  }

  getReportingManager(positionId: string) {
    return this.http.get<ReportingManagerPreview>(`/api/v1/org/positions/${positionId}/reporting-manager`);
  }
}
