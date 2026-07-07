import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';

// -----------------------------------------------------------------------
// Types matching the backend DTOs
// -----------------------------------------------------------------------
interface ValidationIssue {
  code: string;
  message: string;
  field: string | null;
}

interface DraftValidationResult {
  isValid: boolean;
  errors: ValidationIssue[];
  warnings: ValidationIssue[];
  actions: string[];
}

interface SendInviteResult {
  status: 'completed' | 'blocked' | 'invalid' | 'not_found';
  draftReason?: string;
  actions?: string[];
  validation?: DraftValidationResult;
  dev_invite_url?: string;
  employeeId?: string;
  userId?: string;
  error?: string;
}

interface DraftActionResult {
  status: 'ok' | 'invalid_state' | 'not_found';
  message?: string;
}

interface SaveDraftResult {
  id: string;
}

// Lookup options for the Step 2 Org Assignment dropdowns.
// The UI shows names; only the IDs are stored on the draft payload.
interface LegalEntityOption { id: string; name: string; code: string; status: string; }
interface DepartmentOption { id: string; legalEntityId: string; name: string; code: string; status: string; }
interface PositionOption { id: string; legalEntityId: string; departmentId: string; name: string; code: string; status: string; }
interface WorkScheduleOption { id: string; legalEntityId: string; name: string; timezone: string; isActive: boolean; }

type Step = 'employee_details' | 'org_assignment' | 'checklist_review' | 'final_review';

@Component({
  selector: 'app-onboarding',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="onboarding-container">
      <h2>Employee Onboarding</h2>
      <p class="subtitle">Phase 1 — Session-cookie auth · CSRF-protected · employees:write required</p>

      <!-- ========================================================
           Resume by draft ID
           ======================================================== -->
      <div class="card resume-card">
        <h3>Resume Draft</h3>
        <div class="field-row">
          <label for="resume-id">Draft ID</label>
          <input id="resume-id" type="text" [(ngModel)]="resumeDraftId"
                 placeholder="xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx" />
          <button (click)="resumeDraft()" [disabled]="!resumeDraftId" class="btn btn-secondary">
            Load Draft
          </button>
        </div>
        <p *ngIf="resumeError" class="error-text">{{ resumeError }}</p>
      </div>

      <!-- ========================================================
           Completed banner
           ======================================================== -->
      <div *ngIf="finalResult?.status === 'completed'" class="alert alert-success">
        <strong>✓ Onboarding invite sent.</strong>
        <p>Employee ID: <code>{{ finalResult?.employeeId }}</code></p>
        <p>User ID: <code>{{ finalResult?.userId }}</code></p>
        <p *ngIf="finalResult?.dev_invite_url">
          Dev Invite URL:
          <a [href]="finalResult!.dev_invite_url" target="_blank" rel="noopener">
            {{ finalResult!.dev_invite_url }}
          </a>
        </p>
      </div>

      <!-- ========================================================
           Blocked banner — no seat
           ======================================================== -->
      <div *ngIf="finalResult?.status === 'blocked' && finalResult?.draftReason === 'waiting_for_seat'"
           class="alert alert-warning">
        <strong>⚠ No employee seat available.</strong>
        <p>This draft is blocked. Request a seat increase from the billing manager.</p>
        <button (click)="requestSeat()" class="btn btn-warning">Request Seat Increase</button>
        <p *ngIf="actionResult" class="action-result">{{ actionResult }}</p>
      </div>

      <!-- ========================================================
           Blocked banner — position approval
           ======================================================== -->
      <div *ngIf="finalResult?.status === 'blocked' && finalResult?.draftReason === 'waiting_for_position_approval'"
           class="alert alert-warning">
        <strong>⚠ Sensitive position requires approval.</strong>
        <p>A position approver must review this draft before the invite can be sent.</p>
        <button (click)="submitPositionApproval()" class="btn btn-warning">Submit for Approval</button>
        <p *ngIf="actionResult" class="action-result">{{ actionResult }}</p>
      </div>

      <!-- ========================================================
           Wizard (hidden once completed)
           ======================================================== -->
      <div class="wizard" *ngIf="finalResult?.status !== 'completed'">

        <!-- Step indicator -->
        <nav class="step-nav">
          <span [class.active]="currentStep === 'employee_details'">1. Employee Details</span>
          <span class="sep">›</span>
          <span [class.active]="currentStep === 'org_assignment'">2. Org Assignment</span>
          <span class="sep">›</span>
          <span [class.active]="currentStep === 'checklist_review'">3. Checklist</span>
          <span class="sep">›</span>
          <span [class.active]="currentStep === 'final_review'">4. Final Review</span>
        </nav>

        <!-- ---- Step 1: Employee Details ---- -->
        <div class="step-card" *ngIf="currentStep === 'employee_details'">
          <h3>Step 1 — Employee Details</h3>
          <div class="field-row">
            <label for="emp-name">Full Name <span class="req">*</span></label>
            <input id="emp-name" type="text" [(ngModel)]="draft.employeeName"
                   placeholder="e.g. Jane Smith" required />
          </div>
          <div class="field-row">
            <label for="emp-email">Work Email <span class="req">*</span></label>
            <input id="emp-email" type="email" [(ngModel)]="draft.workEmail"
                   placeholder="jane@yourcompany.com" required />
          </div>
          <div class="field-row">
            <label for="emp-type">Employment Type</label>
            <select id="emp-type" [(ngModel)]="draft.employmentType">
              <option value="full_time">Full Time</option>
              <option value="part_time">Part Time</option>
              <option value="contract">Contract</option>
            </select>
          </div>
          <div class="field-row">
            <label for="emp-start">Start Date</label>
            <input id="emp-start" type="date" [(ngModel)]="draft.startDate" />
          </div>
          <div class="field-row">
            <label for="emp-number">Employee Number</label>
            <input id="emp-number" type="text" [(ngModel)]="draft.employeeNumber"
                   placeholder="Leave blank to auto-generate" />
          </div>
          <div class="step-actions">
            <button class="btn btn-primary"
                    [disabled]="!draft.employeeName || !draft.workEmail"
                    (click)="saveDraftAndAdvance('org_assignment')">
              Save & Next
            </button>
          </div>
        </div>

        <!-- ---- Step 2: Org Assignment ---- -->
        <div class="step-card" *ngIf="currentStep === 'org_assignment'">
          <h3>Step 2 — Org Assignment</h3>
          <p *ngIf="lookupError" class="error-text">{{ lookupError }}</p>
          <div class="field-row">
            <label for="company">Company <span class="req">*</span></label>
            <select id="company" [(ngModel)]="draft.legalEntityId"
                    (ngModelChange)="onCompanyChange()">
              <option value="">— Select a company —</option>
              <option *ngFor="let le of legalEntities" [value]="le.id">{{ le.name }}</option>
            </select>
          </div>
          <div class="field-row">
            <label for="dept">Department</label>
            <select id="dept" [(ngModel)]="draft.departmentId"
                    (ngModelChange)="onDepartmentChange()"
                    [disabled]="!draft.legalEntityId">
              <option value="">— Optional —</option>
              <option *ngFor="let d of departments" [value]="d.id">{{ d.name }} ({{ d.code }})</option>
            </select>
          </div>
          <div class="field-row">
            <label for="pos">Position</label>
            <select id="pos" [(ngModel)]="draft.positionId"
                    (ngModelChange)="onPositionChange()"
                    [disabled]="!draft.legalEntityId">
              <option value="">— Optional — sensitive positions trigger approval —</option>
              <option *ngFor="let p of positions" [value]="p.id">{{ positionOptionLabel(p) }}</option>
            </select>
          </div>
          <div class="field-row">
            <label for="sched">Work Schedule</label>
            <select id="sched" [(ngModel)]="draft.scheduleId"
                    [disabled]="!draft.legalEntityId">
              <option value="">— Optional —</option>
              <option *ngFor="let s of schedules" [value]="s.id">{{ s.name }} ({{ s.timezone }})</option>
            </select>
          </div>
          <div class="step-actions">
            <button class="btn btn-secondary" (click)="currentStep = 'employee_details'">Back</button>
            <button class="btn btn-primary"
                    (click)="saveDraftAndAdvance('checklist_review')">
              Save & Next
            </button>
          </div>
        </div>

        <!-- ---- Step 3: Checklist ---- -->
        <div class="step-card" *ngIf="currentStep === 'checklist_review'">
          <h3>Step 3 — Checklist Tasks</h3>
          <p class="field-hint">
            Edit the JSON task list below. Each task must have at least a <code>title</code> field.
            Example: <code>[{{ '{' }}"title":"IT setup","ownerType":"it"{{ '}' }}]</code>
          </p>
          <div class="field-row">
            <label for="template-id">Checklist Template ID</label>
            <input id="template-id" type="text" [(ngModel)]="draft.selectedTemplateId"
                   placeholder="optional — leave blank to use edited tasks only" />
          </div>
          <div class="field-row full-width">
            <label for="tasks-json">Edited Tasks JSON</label>
            <textarea id="tasks-json" [(ngModel)]="draft.editedTasksJson" rows="6"
                      placeholder='[{"title":"Complete HR forms","ownerType":"hr"},{"title":"IT equipment setup","ownerType":"it"}]'>
            </textarea>
          </div>
          <div class="step-actions">
            <button class="btn btn-secondary" (click)="currentStep = 'org_assignment'">Back</button>
            <button class="btn btn-primary" (click)="saveChecklistAndAdvance()">Save & Next</button>
          </div>
        </div>

        <!-- ---- Step 4: Final Review ---- -->
        <div class="step-card" *ngIf="currentStep === 'final_review'">
          <h3>Step 4 — Final Review</h3>
          <table class="review-table">
            <tr><th>Name</th><td>{{ draft.employeeName }}</td></tr>
            <tr><th>Work Email</th><td>{{ draft.workEmail }}</td></tr>
            <tr><th>Employment Type</th><td>{{ draft.employmentType }}</td></tr>
            <tr><th>Start Date</th><td>{{ draft.startDate || '—' }}</td></tr>
            <tr><th>Employee No.</th><td>{{ draft.employeeNumber || 'Auto-generate' }}</td></tr>
            <tr><th>Company</th><td>{{ companyName() }}</td></tr>
            <tr><th>Department</th><td>{{ departmentName() }}</td></tr>
            <tr><th>Position</th><td>{{ positionName() }}</td></tr>
            <tr><th>Work Schedule</th><td>{{ scheduleName() }}</td></tr>
            <tr><th>Draft ID</th><td><code>{{ draftId || '—' }}</code></td></tr>
          </table>

          <!-- Pre-invite validation -->
          <div class="validate-section">
            <button class="btn btn-outline" (click)="validateDraft()" [disabled]="!draftId">
              Run Validation
            </button>
            <div *ngIf="validationResult" class="validation-result">
              <div [class]="validationResult.isValid ? 'badge badge-ok' : 'badge badge-error'">
                {{ validationResult.isValid ? '✓ Valid' : '✗ Invalid' }}
              </div>
              <ul *ngIf="validationResult.errors?.length" class="issue-list errors">
                <li *ngFor="let e of validationResult.errors">
                  <strong>[{{ e.code }}]</strong> {{ e.message }}
                  <span *ngIf="e.field" class="field-badge">{{ e.field }}</span>
                </li>
              </ul>
              <ul *ngIf="validationResult.warnings?.length" class="issue-list warnings">
                <li *ngFor="let w of validationResult.warnings">
                  ⚠ [{{ w.code }}] {{ w.message }}
                </li>
              </ul>
              <div *ngIf="validationResult.actions?.length" class="actions-list">
                <strong>Available actions:</strong> {{ validationResult.actions.join(', ') }}
              </div>
            </div>
          </div>

          <!-- Send invite errors -->
          <div *ngIf="finalResult?.status === 'invalid'" class="alert alert-error">
            <strong>✗ Validation failed — no rows were created.</strong>
            <ul *ngIf="finalResult?.validation?.errors?.length" class="issue-list errors">
              <li *ngFor="let e of finalResult!.validation!.errors">
                [{{ e.code }}] {{ e.message }}
              </li>
            </ul>
            <p *ngIf="finalResult?.error">{{ finalResult!.error }}</p>
          </div>

          <div class="step-actions">
            <button class="btn btn-secondary" (click)="currentStep = 'checklist_review'">Back</button>
            <button class="btn btn-primary btn-finalize" (click)="sendInvite()" [disabled]="!draftId">
              Send Invite
            </button>
          </div>
        </div>
      </div>

      <!-- ========================================================
           Dev admin actions
           ======================================================== -->
      <div class="admin-panel">
        <h3>Dev Tools</h3>
        <p class="field-hint">Requires <code>notifications:manage</code> permission.</p>
        <button class="btn btn-outline" (click)="processOutbox()">Process Email Outbox</button>
        <p *ngIf="outboxResult" class="action-result">{{ outboxResult }}</p>
      </div>

      <!-- ========================================================
           Last raw API response (for debugging)
           ======================================================== -->
      <details *ngIf="lastRawResponse" class="raw-panel">
        <summary>Last API Response</summary>
        <pre>{{ lastRawResponse | json }}</pre>
      </details>
    </div>
  `,
  styles: [`
    .onboarding-container {
      font-family: 'Segoe UI', system-ui, sans-serif;
      max-width: 720px;
      margin: 32px auto;
      padding: 0 16px 64px;
      color: #1a1a2e;
    }
    h2 { font-size: 1.75rem; margin-bottom: 4px; }
    .subtitle { color: #6b7280; font-size: 0.85rem; margin-bottom: 28px; }

    .card, .wizard, .admin-panel, .raw-panel {
      background: #fff;
      border: 1px solid #e5e7eb;
      border-radius: 10px;
      padding: 20px 24px;
      margin-bottom: 20px;
    }
    .resume-card { background: #f9fafb; }

    h3 { font-size: 1.1rem; margin: 0 0 16px; color: #111827; }

    .step-nav {
      display: flex;
      align-items: center;
      gap: 8px;
      font-size: 0.82rem;
      color: #9ca3af;
      margin-bottom: 20px;
      flex-wrap: wrap;
    }
    .step-nav span.active { color: #4f46e5; font-weight: 600; }
    .sep { color: #d1d5db; }

    .step-card {
      border: 1px solid #e5e7eb;
      border-radius: 8px;
      padding: 20px;
      margin-bottom: 16px;
    }
    .step-card h3 { margin-bottom: 16px; }

    .field-row {
      display: flex;
      align-items: center;
      gap: 10px;
      margin-bottom: 14px;
    }
    .field-row.full-width { flex-direction: column; align-items: flex-start; }
    .field-row label { min-width: 150px; font-size: 0.9rem; font-weight: 500; color: #374151; }
    .field-row input, .field-row select, .field-row textarea {
      flex: 1;
      width: 100%;
      padding: 7px 10px;
      border: 1px solid #d1d5db;
      border-radius: 6px;
      font-size: 0.9rem;
      font-family: inherit;
      box-sizing: border-box;
    }
    .field-row textarea { min-height: 110px; resize: vertical; }
    .field-hint { font-size: 0.8rem; color: #6b7280; margin-bottom: 14px; }
    .req { color: #ef4444; }

    .step-actions { display: flex; gap: 10px; justify-content: flex-end; margin-top: 20px; }

    .btn {
      padding: 8px 18px;
      border: none;
      border-radius: 6px;
      cursor: pointer;
      font-size: 0.9rem;
      font-weight: 500;
      transition: opacity 0.15s;
    }
    .btn:disabled { opacity: 0.5; cursor: not-allowed; }
    .btn-primary { background: #4f46e5; color: #fff; }
    .btn-primary:hover:not(:disabled) { background: #4338ca; }
    .btn-secondary { background: #f3f4f6; color: #374151; border: 1px solid #d1d5db; }
    .btn-secondary:hover:not(:disabled) { background: #e5e7eb; }
    .btn-outline { background: transparent; color: #4f46e5; border: 1px solid #4f46e5; }
    .btn-outline:hover:not(:disabled) { background: #eef2ff; }
    .btn-warning { background: #d97706; color: #fff; }
    .btn-warning:hover:not(:disabled) { background: #b45309; }
    .btn-finalize { background: #059669; }
    .btn-finalize:hover:not(:disabled) { background: #047857; }

    .alert {
      border-radius: 8px;
      padding: 16px 20px;
      margin-bottom: 16px;
      font-size: 0.9rem;
    }
    .alert-success { background: #ecfdf5; border: 1px solid #6ee7b7; color: #065f46; }
    .alert-warning { background: #fffbeb; border: 1px solid #fcd34d; color: #92400e; }
    .alert-error   { background: #fef2f2; border: 1px solid #fca5a5; color: #7f1d1d; }
    .alert a { color: inherit; font-weight: 600; }

    .validate-section { margin: 16px 0; }
    .validation-result { margin-top: 12px; }
    .badge { display: inline-block; padding: 3px 10px; border-radius: 9999px; font-size: 0.8rem; font-weight: 600; margin-bottom: 8px; }
    .badge-ok { background: #d1fae5; color: #065f46; }
    .badge-error { background: #fee2e2; color: #7f1d1d; }

    .issue-list { margin: 6px 0; padding-left: 20px; }
    .issue-list li { margin-bottom: 4px; font-size: 0.85rem; }
    .issue-list.errors { color: #7f1d1d; }
    .issue-list.warnings { color: #92400e; }
    .field-badge { background: #e0e7ff; color: #3730a3; border-radius: 4px; padding: 1px 6px; font-size: 0.75rem; margin-left: 6px; }
    .actions-list { font-size: 0.85rem; margin-top: 6px; color: #374151; }

    .review-table { width: 100%; border-collapse: collapse; margin-bottom: 16px; font-size: 0.9rem; }
    .review-table th, .review-table td { padding: 6px 10px; border-bottom: 1px solid #f3f4f6; text-align: left; }
    .review-table th { width: 140px; color: #6b7280; font-weight: 500; }

    .admin-panel { background: #f9fafb; }
    .action-result { font-size: 0.85rem; color: #374151; margin-top: 8px; }
    .error-text { color: #ef4444; font-size: 0.85rem; margin-top: 4px; }

    .raw-panel { background: #f9fafb; }
    .raw-panel summary { cursor: pointer; font-size: 0.85rem; color: #6b7280; }
    .raw-panel pre { font-size: 0.78rem; overflow: auto; margin-top: 10px; }
  `]
})
export class OnboardingComponent implements OnInit {
  private http = inject(HttpClient);
  private router = inject(Router);

  // ---- State ----
  draftId: string | null = null;
  currentStep: Step = 'employee_details';
  finalResult: SendInviteResult | null = null;
  validationResult: DraftValidationResult | null = null;
  outboxResult: string | null = null;
  actionResult: string | null = null;
  resumeDraftId = '';
  resumeError: string | null = null;
  lastRawResponse: unknown = null;

  // ---- Step 2 lookup data (names shown in UI; IDs stored on the draft) ----
  legalEntities: LegalEntityOption[] = [];
  departments: DepartmentOption[] = [];
  positions: PositionOption[] = [];
  schedules: WorkScheduleOption[] = [];
  lookupError: string | null = null;

  draft = {
    employeeName: '',
    workEmail: '',
    employmentType: 'full_time' as string,
    startDate: '' as string,
    employeeNumber: '' as string,
    legalEntityId: '' as string,
    departmentId: '' as string,
    positionId: '' as string,
    scheduleId: '' as string,
    selectedTemplateId: '' as string,
    editedTasksJson: '[]' as string,
  };

  async ngOnInit() {
    await this.loadLegalEntities();
  }

  // ---- Step 2: Org lookup loading + cascading ----
  private async loadLegalEntities() {
    try {
      this.legalEntities = await firstValueFrom(
        this.http.get<LegalEntityOption[]>('/api/v1/org/legal-entities')
      );
      this.lookupError = null;
    } catch {
      this.lookupError = 'Could not load companies. Check that your role has org read permissions.';
    }
  }

  private async loadCompanyScopedLookups() {
    if (!this.draft.legalEntityId) {
      this.departments = [];
      this.positions = [];
      this.schedules = [];
      return;
    }
    try {
      const le = encodeURIComponent(this.draft.legalEntityId);
      const deptFilter = this.draft.departmentId
        ? `&departmentId=${encodeURIComponent(this.draft.departmentId)}`
        : '';
      [this.departments, this.positions, this.schedules] = await Promise.all([
        firstValueFrom(this.http.get<DepartmentOption[]>(`/api/v1/org/departments?legalEntityId=${le}`)),
        firstValueFrom(this.http.get<PositionOption[]>(`/api/v1/org/positions?legalEntityId=${le}${deptFilter}`)),
        firstValueFrom(this.http.get<WorkScheduleOption[]>(`/api/v1/time-attendance/work-schedules?legalEntityId=${le}`)),
      ]);
      this.schedules = this.schedules.filter(s => s.isActive);
      this.lookupError = null;
    } catch {
      this.lookupError = 'Could not load org data for the selected company.';
    }
  }

  async onCompanyChange() {
    this.draft.departmentId = '';
    this.draft.positionId = '';
    this.draft.scheduleId = '';
    await this.loadCompanyScopedLookups();
  }

  async onDepartmentChange() {
    const pos = this.positions.find(p => p.id === this.draft.positionId);
    if (pos && this.draft.departmentId && pos.departmentId !== this.draft.departmentId) {
      this.draft.positionId = '';
    }
    await this.loadCompanyScopedLookups();
  }

  async onPositionChange() {
    const pos = this.positions.find(p => p.id === this.draft.positionId);
    if (!pos) return;
    // Positions belong to exactly one department; keep the department aligned.
    if (this.draft.departmentId !== pos.departmentId) {
      this.draft.departmentId = pos.departmentId;
      await this.loadCompanyScopedLookups();
    }
  }

  positionOptionLabel(p: PositionOption): string {
    const dept = this.departments.find(d => d.id === p.departmentId);
    return dept ? `${p.name} (${p.code}) — ${dept.name}` : `${p.name} (${p.code})`;
  }

  companyName(): string {
    return this.legalEntities.find(le => le.id === this.draft.legalEntityId)?.name ?? '—';
  }

  departmentName(): string {
    return this.departments.find(d => d.id === this.draft.departmentId)?.name ?? '—';
  }

  positionName(): string {
    return this.positions.find(p => p.id === this.draft.positionId)?.name ?? '—';
  }

  scheduleName(): string {
    return this.schedules.find(s => s.id === this.draft.scheduleId)?.name ?? '—';
  }

  // ---- Step 1 & 2: Save draft ----
  async saveDraftAndAdvance(nextStep: Step) {
    try {
      const payload: Record<string, unknown> = {
        employeeName: this.draft.employeeName,
        workEmail: this.draft.workEmail,
        employmentType: this.draft.employmentType || null,
        lastSavedStep: this.currentStep,
      };

      if (this.draft.startDate) payload['startDate'] = this.draft.startDate;
      if (this.draft.employeeNumber?.trim()) payload['employeeNumber'] = this.draft.employeeNumber.trim();
      if (this.draft.legalEntityId?.trim()) payload['legalEntityId'] = this.draft.legalEntityId.trim();
      if (this.draft.departmentId?.trim()) payload['departmentId'] = this.draft.departmentId.trim();
      if (this.draft.positionId?.trim()) payload['positionId'] = this.draft.positionId.trim();
      if (this.draft.scheduleId?.trim()) payload['scheduleId'] = this.draft.scheduleId.trim();
      if (this.draftId) payload['id'] = this.draftId; // for upsert

      const url = this.draftId
        ? `/api/v1/onboarding/drafts`
        : `/api/v1/onboarding/drafts`;

      const res = await firstValueFrom(
        this.http.post<SaveDraftResult>(url, payload)
      );
      this.lastRawResponse = res;
      this.draftId = res.id;
      this.currentStep = nextStep;
      this.finalResult = null;
      this.validationResult = null;
    } catch (err: any) {
      this.lastRawResponse = err?.error;
      alert('Failed to save draft: ' + (err?.error?.error?.message ?? JSON.stringify(err?.error)));
    }
  }

  // ---- Step 3: Checklist ----
  async saveChecklistAndAdvance() {
    if (!this.draftId) {
      await this.saveDraftAndAdvance('checklist_review');
    }
    try {
      const payload = {
        selectedTemplateId: this.draft.selectedTemplateId?.trim() || null,
        editedTasksJson: this.draft.editedTasksJson || '[]',
      };
      await firstValueFrom(
        this.http.post(`/api/v1/onboarding/drafts/${this.draftId}/checklist`, payload)
      );
      this.currentStep = 'final_review';
    } catch (err: any) {
      this.lastRawResponse = err?.error;
      alert('Failed to save checklist: ' + (err?.error?.error ?? JSON.stringify(err?.error)));
    }
  }

  // ---- Step 4: Validate ----
  async validateDraft() {
    if (!this.draftId) return;
    try {
      this.validationResult = await firstValueFrom(
        this.http.post<DraftValidationResult>(`/api/v1/onboarding/drafts/${this.draftId}/validate`, null)
      );
      this.lastRawResponse = this.validationResult;
    } catch (err: any) {
      this.lastRawResponse = err?.error;
      alert('Validation request failed: ' + JSON.stringify(err?.error));
    }
  }

  // ---- Step 4: Send invite ----
  async sendInvite() {
    if (!this.draftId) return;
    try {
      const result = await firstValueFrom(
        this.http.post<SendInviteResult>(`/api/v1/onboarding/drafts/${this.draftId}/send-invite`, null)
      );
      this.finalResult = result;
      this.lastRawResponse = result;
      this.actionResult = null;
    } catch (err: any) {
      // 400 returns the structured invalid result; parse it
      this.finalResult = err?.error ?? null;
      this.lastRawResponse = err?.error;
    }
  }

  // ---- Blocked: Request seat ----
  async requestSeat() {
    if (!this.draftId) return;
    try {
      const result = await firstValueFrom(
        this.http.post<DraftActionResult>(`/api/v1/onboarding/drafts/${this.draftId}/request-seat`, null)
      );
      this.lastRawResponse = result;
      this.actionResult = result.status === 'ok'
        ? '✓ Seat increase request sent to the billing manager.'
        : result.message ?? 'Unexpected response.';
    } catch (err: any) {
      this.lastRawResponse = err?.error;
      this.actionResult = 'Request failed: ' + (err?.error?.error ?? JSON.stringify(err?.error));
    }
  }

  // ---- Blocked: Submit position approval ----
  async submitPositionApproval() {
    if (!this.draftId) return;
    try {
      const result = await firstValueFrom(
        this.http.post<DraftActionResult>(`/api/v1/onboarding/drafts/${this.draftId}/submit-approval`, null)
      );
      this.lastRawResponse = result;
      this.actionResult = result.status === 'ok'
        ? '✓ Approval request sent to the position approver.'
        : result.message ?? 'Unexpected response.';
    } catch (err: any) {
      this.lastRawResponse = err?.error;
      this.actionResult = 'Submit failed: ' + (err?.error?.error ?? JSON.stringify(err?.error));
    }
  }

  // ---- Resume by ID ----
  async resumeDraft() {
    this.resumeError = null;
    if (!this.resumeDraftId.trim()) return;
    try {
      const draft = await firstValueFrom(
        this.http.get<any>(`/api/v1/onboarding/drafts/${this.resumeDraftId.trim()}`)
      );
      this.lastRawResponse = draft;
      this.draftId = draft.id;

      // Restore fields
      this.draft.employeeName = draft.employeeName ?? '';
      this.draft.workEmail = draft.workEmail ?? '';
      this.draft.employmentType = draft.employmentType ?? 'full_time';
      this.draft.startDate = draft.startDate ?? '';
      this.draft.employeeNumber = draft.employeeNumber ?? '';
      this.draft.legalEntityId = draft.legalEntityId ?? '';
      this.draft.departmentId = draft.departmentId ?? '';
      this.draft.positionId = draft.positionId ?? '';
      this.draft.scheduleId = draft.scheduleId ?? '';
      this.draft.editedTasksJson = draft.editedTasksJson ?? '[]';

      const stepMap: Record<string, Step> = {
        employee_details: 'employee_details',
        org_assignment: 'org_assignment',
        checklist_review: 'checklist_review',
        final_review: 'final_review',
      };
      this.currentStep = stepMap[draft.lastSavedStep] ?? 'employee_details';
      this.finalResult = null;
      this.validationResult = null;
      await this.loadCompanyScopedLookups();
    } catch (err: any) {
      this.lastRawResponse = err?.error;
      this.resumeError = err?.status === 404
        ? 'Draft not found (it may belong to a different tenant).'
        : 'Failed to load draft: ' + JSON.stringify(err?.error);
    }
  }

  // ---- Process email outbox (dev tool) ----
  async processOutbox() {
    this.outboxResult = null;
    try {
      const res = await firstValueFrom(
        this.http.post<{ processed: number }>('/api/v1/outbox/process-emails', null)
      );
      this.lastRawResponse = res;
      this.outboxResult = `✓ Processed ${res.processed} email(s).`;
    } catch (err: any) {
      this.lastRawResponse = err?.error;
      if (err?.status === 401) {
        this.outboxResult = '✗ Not authenticated.';
      } else if (err?.status === 403) {
        this.outboxResult = '✗ Forbidden — requires notifications:manage permission.';
      } else {
        this.outboxResult = '✗ Error: ' + JSON.stringify(err?.error);
      }
    }
  }
}
