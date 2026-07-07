import { Component, EventEmitter, OnInit, Output, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { firstValueFrom } from 'rxjs';
import { ModalComponent } from '../../../shared/ui/modal/modal.component';
import { OnboardingApiService, MyDraftSummary } from '../../../core/api/endpoints/onboarding-api.service';

@Component({
  selector: 'app-my-drafts-drawer',
  standalone: true,
  imports: [CommonModule, ModalComponent],
  template: `
    <ov-modal [open]="true" variant="drawer" title="My Drafts" (closed)="closed.emit()">
      <div *ngIf="loading()" class="state-message">Loading your drafts…</div>
      <div *ngIf="!loading() && drafts().length === 0" class="state-message">
        You have no saved onboarding drafts.
      </div>
      <div class="draft-card" *ngFor="let d of drafts()">
        <div class="draft-name">{{ d.employeeName || 'Unnamed employee' }}</div>
        <div class="draft-email">{{ d.workEmail }}</div>
        <div class="draft-meta">
          <span>Step: {{ stepLabel(d.lastSavedStep) }}</span>
          <span>Reason: {{ reasonLabel(d.draftReason) }}</span>
          <span>Updated: {{ d.updatedAtUtc | date: 'short' }}</span>
        </div>
        <button type="button" class="btn btn-primary btn-sm" (click)="continueDraft.emit(d.id)">Continue</button>
      </div>
    </ov-modal>
  `,
  styles: [`
    .state-message { color: var(--shell-muted-fg); padding: 12px 0; }
    .draft-card {
      border: 1px solid var(--border-color);
      border-radius: 8px;
      padding: 16px;
      margin-bottom: 12px;
    }
    .draft-name { font-weight: 600; }
    .draft-email { color: var(--shell-muted-fg); font-size: 0.875rem; margin-bottom: 8px; }
    .draft-meta {
      display: flex;
      flex-wrap: wrap;
      gap: 4px 12px;
      font-size: 0.75rem;
      color: var(--shell-muted-fg);
      margin-bottom: 12px;
    }
  `]
})
export class MyDraftsDrawerComponent implements OnInit {
  private onboardingApi = inject(OnboardingApiService);

  @Output() closed = new EventEmitter<void>();
  @Output() continueDraft = new EventEmitter<string>();

  drafts = signal<MyDraftSummary[]>([]);
  loading = signal(true);

  async ngOnInit() {
    try {
      const result = await firstValueFrom(this.onboardingApi.getMyDrafts());
      this.drafts.set(result);
    } finally {
      this.loading.set(false);
    }
  }

  stepLabel(step: string): string {
    switch (step) {
      case 'employee_details':
      case 'org_assignment':
        return 'Employee & Assignment';
      case 'checklist_review':
        return 'Checklist';
      case 'final_review':
        return 'Review & Send';
      default:
        return step;
    }
  }

  reasonLabel(reason: string): string {
    switch (reason) {
      case 'saved_manually':
        return 'Saved manually';
      case 'waiting_for_seat':
        return 'Waiting for seat';
      case 'waiting_for_position_approval':
        return 'Waiting for approval';
      default:
        return reason || '—';
    }
  }
}
