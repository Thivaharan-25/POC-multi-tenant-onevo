import { Component, EventEmitter, Input, OnInit, Output, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { firstValueFrom } from 'rxjs';
import { ModalComponent } from '../../../shared/ui/modal/modal.component';
import { OnboardingApiService } from '../../../core/api/endpoints/onboarding-api.service';
import { LegalEntitiesApiService } from '../../../core/api/endpoints/legal-entities-api.service';
import { DepartmentsApiService } from '../../../core/api/endpoints/departments-api.service';
import { PositionsApiService } from '../../../core/api/endpoints/positions-api.service';
import { WorkSchedulesApiService } from '../../../core/api/endpoints/work-schedules-api.service';
import { OnboardingStep1Component, Step1FormData } from './onboarding-step1.component';
import { OnboardingStep2Component, Step2Result } from './onboarding-step2.component';
import { OnboardingStep3Component } from './onboarding-step3.component';

@Component({
  selector: 'app-onboarding-wizard',
  standalone: true,
  imports: [CommonModule, ModalComponent, OnboardingStep1Component, OnboardingStep2Component, OnboardingStep3Component],
  template: `
    <ov-modal
      *ngIf="mode === 'modal'"
      [open]="true"
      [title]="wizardTitle"
      [dismissible]="false"
      (closed)="onClose()"
    >
      <ng-container *ngTemplateOutlet="wizardBody"></ng-container>
    </ov-modal>

    <div class="wizard-inline" *ngIf="mode === 'inline'">
      <ng-container *ngTemplateOutlet="wizardBody"></ng-container>
    </div>

    <ng-template #wizardBody>
      <div class="wizard-steps">
        <span [class.active]="step() === 1">1. Employee &amp; Assignment</span>
        <span [class.active]="step() === 2">2. Checklist</span>
        <span [class.active]="step() === 3">3. Review &amp; Send</span>
      </div>

      <div *ngIf="loadingDraft()" class="state-message">Loading draft…</div>

      <app-onboarding-step1
        *ngIf="!loadingDraft() && step() === 1"
        [initialData]="formData"
        (next)="onStep1Next($event)"
        (cancel)="onClose()"
      ></app-onboarding-step1>

      <app-onboarding-step2
        *ngIf="!loadingDraft() && step() === 2"
        [draftId]="draftId()!"
        [legalEntityId]="formData.legalEntityId!"
        [departmentId]="formData.departmentId"
        [initialTemplateId]="formData.selectedTemplateId"
        [initialTasksJson]="formData.editedTasksJson"
        (back)="step.set(1)"
        (next)="onStep2Next($event)"
      ></app-onboarding-step2>

      <app-onboarding-step3
        *ngIf="!loadingDraft() && step() === 3"
        [draftId]="draftId()!"
        [formData]="formData"
        [templateName]="selectedTemplateName"
        [taskCount]="taskCount"
        [requiredTaskTitles]="requiredTaskTitles"
        (back)="step.set(2)"
        (sent)="onSent()"
      ></app-onboarding-step3>
      <!-- formData (Step1FormData) already carries companyName/departmentName/positionName/
           scheduleName/reportingManagerName set by Step 1's onNext(); Step 3 reads them
           directly off the [formData] input, no separate bindings needed. -->
    </ng-template>
  `,
  styles: [`
    .wizard-steps {
      display: flex;
      gap: 20px;
      margin-bottom: 20px;
      font-size: 0.8125rem;
      color: var(--shell-muted-fg);
    }
    .wizard-steps .active { color: var(--content-fg); font-weight: 600; }
    .state-message { color: var(--shell-muted-fg); padding: 24px 0; text-align: center; }
    .wizard-inline { max-width: 900px; }
  `]
})
export class OnboardingWizardComponent implements OnInit {
  private onboardingApi = inject(OnboardingApiService);
  private legalEntitiesApi = inject(LegalEntitiesApiService);
  private departmentsApi = inject(DepartmentsApiService);
  private positionsApi = inject(PositionsApiService);
  private schedulesApi = inject(WorkSchedulesApiService);

  @Input() mode: 'modal' | 'inline' = 'modal';
  @Input() initialDraftId: string | null = null;
  @Output() closed = new EventEmitter<void>();
  @Output() saved = new EventEmitter<void>();

  step = signal<1 | 2 | 3>(1);
  draftId = signal<string | null>(null);
  loadingDraft = signal(false);

  formData: Step1FormData = {
    employeeName: '',
    workEmail: '',
    employeeNumber: '',
    employmentType: 'full_time',
    startDate: '',
    legalEntityId: null,
    departmentId: null,
    positionId: null,
    scheduleId: null,
    selectedTemplateId: null,
    editedTasksJson: '[]',
    companyName: null,
    departmentName: null,
    positionName: null,
    scheduleName: null,
    reportingManagerName: null
  };

  selectedTemplateName = '';
  taskCount = 0;
  requiredTaskTitles: string[] = [];

  get wizardTitle(): string {
    if (this.step() === 1) return 'Add Employee — Employee & Assignment';
    if (this.step() === 2) return 'Add Employee — Checklist';
    return 'Add Employee — Review & Send Invite';
  }

  async ngOnInit() {
    if (this.initialDraftId) {
      await this.loadExistingDraft(this.initialDraftId);
    }
  }

  private async loadExistingDraft(id: string) {
    this.loadingDraft.set(true);
    try {
      const draft = await firstValueFrom(this.onboardingApi.getDraft(id));
      this.draftId.set(draft.id);
      this.formData = {
        employeeName: draft.employeeName,
        workEmail: draft.workEmail,
        employeeNumber: draft.employeeNumber ?? '',
        employmentType: draft.employmentType ?? 'full_time',
        startDate: draft.startDate ?? '',
        legalEntityId: draft.legalEntityId,
        departmentId: draft.departmentId,
        positionId: draft.positionId,
        scheduleId: draft.scheduleId,
        selectedTemplateId: draft.selectedTemplateId,
        editedTasksJson: draft.editedTasksJson || '[]',
        companyName: null,
        departmentName: null,
        positionName: null,
        scheduleName: null,
        reportingManagerName: null
      };
      await this.resolveDisplayNames();
      this.step.set(this.stepFromLastSaved(draft.lastSavedStep));
    } finally {
      this.loadingDraft.set(false);
    }
  }

  /// Resuming a draft can land directly on Step 2 or 3, skipping Step 1's onNext
  /// (which is what normally fills in the display names) — so resolve them here
  /// from the stored IDs whenever a draft is loaded with org assignment already set.
  private async resolveDisplayNames() {
    const legalEntityId = this.formData.legalEntityId;
    if (!legalEntityId) {
      return;
    }
    const [companies, departments, positions, schedules] = await Promise.all([
      firstValueFrom(this.legalEntitiesApi.list()) as Promise<any[]>,
      firstValueFrom(this.departmentsApi.list(legalEntityId)) as Promise<any[]>,
      firstValueFrom(this.positionsApi.list(legalEntityId)) as Promise<any[]>,
      firstValueFrom(this.schedulesApi.list(legalEntityId)) as Promise<any[]>
    ]);
    this.formData.companyName = companies.find((c: any) => c.id === legalEntityId)?.name ?? null;
    this.formData.departmentName = departments.find((d: any) => d.id === this.formData.departmentId)?.name ?? null;
    this.formData.positionName = positions.find((p: any) => p.id === this.formData.positionId)?.name ?? null;
    this.formData.scheduleName = schedules.find((s: any) => s.id === this.formData.scheduleId)?.name ?? null;

    if (this.formData.positionId) {
      try {
        const manager = await firstValueFrom(this.positionsApi.getReportingManager(this.formData.positionId));
        this.formData.reportingManagerName = manager.hasManager ? (manager.employeeName ?? null) : null;
      } catch {
        this.formData.reportingManagerName = null;
      }
    }
  }

  private stepFromLastSaved(lastSavedStep: string): 1 | 2 | 3 {
    if (lastSavedStep === 'checklist_review') return 2;
    if (lastSavedStep === 'final_review') return 3;
    return 1;
  }

  async onStep1Next(data: Step1FormData) {
    this.formData = { ...this.formData, ...data };
    const payload = {
      id: this.draftId() ?? undefined,
      employeeName: this.formData.employeeName,
      workEmail: this.formData.workEmail,
      legalEntityId: this.formData.legalEntityId,
      departmentId: this.formData.departmentId,
      positionId: this.formData.positionId,
      employmentType: this.formData.employmentType,
      startDate: this.formData.startDate || null,
      employeeNumber: this.formData.employeeNumber || null,
      scheduleId: this.formData.scheduleId,
      lastSavedStep: 'org_assignment'
    };
    const result = await firstValueFrom(this.onboardingApi.saveDraft(payload));
    this.draftId.set(result.id);
    this.step.set(2);
  }

  async onStep2Next(result: Step2Result) {
    this.formData.selectedTemplateId = result.selectedTemplateId;
    this.formData.editedTasksJson = result.editedTasksJson;
    this.selectedTemplateName = result.templateName;
    this.taskCount = result.taskCount;
    this.requiredTaskTitles = result.requiredTaskTitles;

    await firstValueFrom(
      this.onboardingApi.updateChecklist(this.draftId()!, result.selectedTemplateId, result.editedTasksJson)
    );
    this.step.set(3);
  }

  onSent() {
    this.saved.emit();
  }

  onClose() {
    this.closed.emit();
  }
}
