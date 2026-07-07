import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { BYPASS_GLOBAL_ERROR_HANDLING } from '../interceptors/error.interceptor';

export interface WorkScheduleOption {
  id: string;
  name: string;
  timezone: string;
}

@Injectable({ providedIn: 'root' })
export class WorkSchedulesApiService {
  private http = inject(HttpClient);

  list(legalEntityId: string) {
    return this.http.get<WorkScheduleOption[]>('/api/v1/time-attendance/work-schedules', {
      params: { legalEntityId },
      context: new HttpContext().set(BYPASS_GLOBAL_ERROR_HANDLING, true)
    });
  }
}
