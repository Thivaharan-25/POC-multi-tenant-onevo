import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

export interface SaveOnboardingDraftPayload {
  id?: string;
  employeeName: string;
  workEmail: string;
  legalEntityId: string | null;
  departmentId: string | null;
  positionId: string | null;
  employmentType: string | null;
  startDate: string | null;
  employeeNumber: string | null;
  scheduleId: string | null;
  lastSavedStep: string;
}

export interface OnboardingDraft {
  id: string;
  employeeName: string;
  workEmail: string;
  legalEntityId: string | null;
  departmentId: string | null;
  positionId: string | null;
  employmentType: string | null;
  startDate: string | null;
  employeeNumber: string | null;
  scheduleId: string | null;
  selectedTemplateId: string | null;
  editedTasksJson: string;
  status: string;
  draftReason: string;
  lastSavedStep: string;
}

export interface MyDraftSummary {
  id: string;
  employeeName: string;
  workEmail: string;
  lastSavedStep: string;
  draftReason: string;
  updatedAtUtc: string;
}

export interface ChecklistTemplateSummary {
  id: string;
  name: string;
  templateType: string;
  departmentId: string | null;
  isActive: boolean;
}

export interface ChecklistTemplateTask {
  title: string;
  ownerType: string | null;
  sequence: number | null;
  isRequired: boolean;
  isLocked: boolean;
}

export interface ChecklistTemplateDetail {
  id: string;
  name: string;
  templateType: string;
  departmentId: string | null;
  isActive: boolean;
  tasks: ChecklistTemplateTask[];
}

export interface ChecklistTemplateListResponse {
  templates: ChecklistTemplateSummary[];
  recommendedTemplateId: string | null;
}

export interface ValidationIssue {
  code: string;
  message: string;
  field?: string;
}

export interface DraftValidationResult {
  isValid: boolean;
  errors: ValidationIssue[];
  warnings: ValidationIssue[];
  actions: string[];
}

export interface SendInviteResult {
  status: 'completed' | 'blocked' | 'invalid' | 'not_found';
  draftReason?: string;
  validation?: DraftValidationResult;
  actions?: string[];
  employeeId?: string;
  userId?: string;
  dev_invite_url?: string;
  error?: string;
}

@Injectable({ providedIn: 'root' })
export class OnboardingApiService {
  private http = inject(HttpClient);

  saveDraft(payload: SaveOnboardingDraftPayload) {
    return this.http.post<{ id: string }>('/api/v1/onboarding/drafts', payload);
  }

  getDraft(id: string) {
    return this.http.get<OnboardingDraft>(`/api/v1/onboarding/drafts/${id}`);
  }

  getMyDrafts() {
    return this.http.get<MyDraftSummary[]>('/api/v1/onboarding/drafts/mine');
  }

  getChecklistTemplates(legalEntityId: string, departmentId?: string | null) {
    const params: Record<string, string> = { legalEntityId };
    if (departmentId) params['departmentId'] = departmentId;
    return this.http.get<ChecklistTemplateListResponse>('/api/v1/onboarding/checklist-templates', { params });
  }

  getChecklistTemplateDetail(templateId: string) {
    return this.http.get<ChecklistTemplateDetail>(`/api/v1/onboarding/checklist-templates/${templateId}`);
  }

  updateChecklist(draftId: string, selectedTemplateId: string | null, editedTasksJson: string) {
    return this.http.post(`/api/v1/onboarding/drafts/${draftId}/checklist`, { selectedTemplateId, editedTasksJson });
  }

  validateDraft(draftId: string) {
    return this.http.post<DraftValidationResult>(`/api/v1/onboarding/drafts/${draftId}/validate`, null);
  }

  sendInvite(draftId: string) {
    return this.http.post<SendInviteResult>(`/api/v1/onboarding/drafts/${draftId}/send-invite`, null);
  }

  requestSeat(draftId: string) {
    return this.http.post<{ status: string; message?: string }>(`/api/v1/onboarding/drafts/${draftId}/request-seat`, null);
  }

  submitPositionApproval(draftId: string) {
    return this.http.post<{ status: string; message?: string }>(`/api/v1/onboarding/drafts/${draftId}/submit-approval`, null);
  }
}
