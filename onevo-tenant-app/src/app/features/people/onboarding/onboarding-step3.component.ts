import { Component, EventEmitter, Input, OnInit, Output, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { firstValueFrom } from 'rxjs';
import { OnboardingApiService, SendInviteResult } from '../../../core/api/endpoints/onboarding-api.service';
import { Step1FormData } from './onboarding-step1.component';

@Component({
  selector: 'app-onboarding-step3',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="review">
      <dl class="review-grid">
        <dt>Employee Name</dt>
        <dd>{{ formData.employeeName }}</dd>
        <dt>Work Email</dt>
        <dd>{{ formData.workEmail }}</dd>
        <dt>Employee Number</dt>
        <dd>{{ formData.employeeNumber || 'Auto-generated' }}</dd>
        <dt>Employment Type</dt>
        <dd>{{ formData.employmentType }}</dd>
        <dt>Start Date</dt>
        <dd>{{ formData.startDate }}</dd>
        <dt>Company</dt>
        <dd>{{ formData.companyName || '—' }}</dd>
        <dt>Department</dt>
        <dd>{{ formData.departmentName || '—' }}</dd>
        <dt>Position</dt>
        <dd>{{ formData.positionName || '—' }}</dd>
        <dt>Work Schedule</dt>
        <dd>{{ formData.scheduleName || '—' }}</dd>
        <dt>Reporting Manager</dt>
        <dd>{{ formData.reportingManagerName || 'None' }}</dd>
        <dt>Checklist Template</dt>
        <dd>{{ templateName }}</dd>
        <dt>Checklist Tasks</dt>
        <dd>{{ taskCount }} task(s)</dd>
        <dt>Required Tasks</dt>
        <dd>{{ requiredTaskTitles.join(', ') || 'None' }}</dd>
        <dt>Invite will be sent to</dt>
        <dd>{{ formData.workEmail }}</dd>
      </dl>

      <div class="warning-banner" *ngIf="warnings().length > 0">
        <div class="warning-item" *ngFor="let w of warnings()">{{ w }}</div>
      </div>

      <div class="validation-errors" *ngIf="errors().length > 0">
        <div class="error-item" *ngFor="let e of errors()">{{ e }}</div>
      </div>

      <div class="blocked-banner" *ngIf="blockedReason()">
        <p>{{ blockedMessage() }}</p>
        <button
          type="button"
          class="btn btn-outline"
          *ngIf="blockedReason() === 'waiting_for_seat'"
          (click)="requestSeat()"
        >
          Request Seat Increase
        </button>
        <button
          type="button"
          class="btn btn-outline"
          *ngIf="blockedReason() === 'waiting_for_position_approval'"
          (click)="submitApproval()"
        >
          Submit Approval
        </button>
      </div>

      <div class="success-banner" *ngIf="completed()">
        Invite queued for {{ formData.workEmail }}.
      </div>

      <div class="wizard-actions">
        <button type="button" class="btn btn-outline" (click)="back.emit()" [disabled]="sending()">Back</button>
        <button type="button" class="btn btn-primary" (click)="sendInvite()" [disabled]="sending() || completed()">
          {{ sending() ? 'Sending…' : 'Send Invite' }}
        </button>
      </div>
    </div>
  `,
  styles: [`
    .review-grid { display: grid; grid-template-columns: 200px 1fr; row-gap: 10px; margin-bottom: 20px; }
    .review-grid dt { color: var(--shell-muted-fg); font-size: 0.8125rem; }
    .review-grid dd { margin: 0; }
    .validation-errors, .blocked-banner {
      background: rgba(239, 68, 68, 0.08);
      border: 1px solid var(--border-color);
      border-radius: 6px;
      padding: 12px 16px;
      margin-bottom: 16px;
    }
    .warning-banner {
      background: rgba(245, 158, 11, 0.1);
      border: 1px solid var(--border-color);
      border-radius: 6px;
      padding: 12px 16px;
      margin-bottom: 16px;
    }
    .success-banner {
      background: rgba(16, 185, 129, 0.1);
      border-radius: 6px;
      padding: 12px 16px;
      margin-bottom: 16px;
    }
    .wizard-actions { display: flex; justify-content: space-between; }
  `]
})
export class OnboardingStep3Component implements OnInit {
  private onboardingApi = inject(OnboardingApiService);

  @Input() draftId!: string;
  @Input() formData!: Step1FormData;
  @Input() templateName = '';
  @Input() taskCount = 0;
  @Input() requiredTaskTitles: string[] = [];
  @Output() back = new EventEmitter<void>();
  @Output() sent = new EventEmitter<void>();

  sending = signal(false);
  errors = signal<string[]>([]);
  warnings = signal<string[]>([]);
  blockedReason = signal<string | null>(null);
  completed = signal(false);

  async ngOnInit() {
    // Surface seat-availability / sensitive-position warnings proactively, before
    // the user clicks Send Invite, using the existing validate endpoint.
    try {
      const result = await firstValueFrom(this.onboardingApi.validateDraft(this.draftId));
      this.warnings.set((result.warnings ?? []).map(w => w.message));
    } catch {
      this.warnings.set([]);
    }
  }

  blockedMessage(): string {
    return this.blockedReason() === 'waiting_for_seat'
      ? 'No seats are available on the current plan.'
      : 'This position requires approval before an invite can be sent.';
  }

  async sendInvite() {
    this.sending.set(true);
    this.errors.set([]);
    this.blockedReason.set(null);
    try {
      const result: SendInviteResult = await firstValueFrom(this.onboardingApi.sendInvite(this.draftId));
      if (result.status === 'completed') {
        this.completed.set(true);
        this.sent.emit();
      } else if (result.status === 'blocked') {
        this.blockedReason.set(result.draftReason ?? null);
      } else if (result.status === 'invalid') {
        this.errors.set((result.validation?.errors ?? []).map(e => e.message));
      } else {
        this.errors.set(['Draft could not be found.']);
      }
    } finally {
      this.sending.set(false);
    }
  }

  async requestSeat() {
    await firstValueFrom(this.onboardingApi.requestSeat(this.draftId));
    this.blockedReason.set(null);
    this.errors.set(['Seat increase requested. You will be notified when a seat is available.']);
  }

  async submitApproval() {
    await firstValueFrom(this.onboardingApi.submitPositionApproval(this.draftId));
    this.blockedReason.set(null);
    this.errors.set(['Approval submitted. You will be notified once it is reviewed.']);
  }
}
