import { Component, EventEmitter, Input, OnChanges, OnInit, Output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';
import { FormFieldComponent } from '../../../shared/ui/form-field/form-field.component';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';
import { LegalEntitiesApiService } from '../../../core/api/endpoints/legal-entities-api.service';
import { DepartmentsApiService } from '../../../core/api/endpoints/departments-api.service';
import { PositionsApiService } from '../../../core/api/endpoints/positions-api.service';
import { WorkSchedulesApiService } from '../../../core/api/endpoints/work-schedules-api.service';
import { CompanyContextService } from '../../../core/context/company-context.service';

export interface Step1FormData {
  employeeName: string;
  workEmail: string;
  employeeNumber: string;
  employmentType: string;
  startDate: string;
  legalEntityId: string | null;
  departmentId: string | null;
  positionId: string | null;
  scheduleId: string | null;
  selectedTemplateId: string | null;
  editedTasksJson: string;
  companyName: string | null;
  departmentName: string | null;
  positionName: string | null;
  scheduleName: string | null;
  reportingManagerName: string | null;
}

interface Option {
  id: string;
  name: string;
}

@Component({
  selector: 'app-onboarding-step1',
  standalone: true,
  imports: [CommonModule, FormsModule, FormFieldComponent, HasPermissionDirective],
  template: `
    <form class="wizard-form" (ngSubmit)="onNext()">
      <div class="form-grid">
        <ov-form-field label="Employee Name" [required]="true">
          <input type="text" [(ngModel)]="data.employeeName" name="employeeName" required />
        </ov-form-field>
        <ov-form-field label="Work Email" [required]="true">
          <input type="email" [(ngModel)]="data.workEmail" name="workEmail" required />
        </ov-form-field>
        <ov-form-field label="Employee Number" hint="Optional — auto-generated if left blank">
          <input type="text" [(ngModel)]="data.employeeNumber" name="employeeNumber" />
        </ov-form-field>
        <ov-form-field label="Employment Type" [required]="true">
          <select [(ngModel)]="data.employmentType" name="employmentType" required>
            <option value="full_time">Full-time</option>
            <option value="part_time">Part-time</option>
            <option value="contractor">Contractor</option>
            <option value="intern">Intern</option>
          </select>
        </ov-form-field>
        <ov-form-field label="Start Date" [required]="true">
          <input type="date" [(ngModel)]="data.startDate" name="startDate" required />
        </ov-form-field>
        <ov-form-field label="Department">
          <select
            [(ngModel)]="data.departmentId"
            name="departmentId"
            (ngModelChange)="onDepartmentChange()"
            [disabled]="!data.legalEntityId"
          >
            <option [ngValue]="null">Select department…</option>
            <option *ngFor="let d of departments" [ngValue]="d.id">{{ d.name }}</option>
          </select>
        </ov-form-field>
        <ov-form-field label="Position" [required]="true">
          <select
            [(ngModel)]="data.positionId"
            name="positionId"
            (ngModelChange)="onPositionChange()"
            [disabled]="!data.legalEntityId"
            required
          >
            <option [ngValue]="null">Select position…</option>
            <option *ngFor="let p of positions" [ngValue]="p.id">{{ p.name }}</option>
          </select>
        </ov-form-field>
        <ov-form-field label="Work Schedule" *hasPermission="'attendance:read'">
          <select [(ngModel)]="data.scheduleId" name="scheduleId" [disabled]="!data.legalEntityId">
            <option [ngValue]="null">Select schedule…</option>
            <option *ngFor="let s of schedules" [ngValue]="s.id">{{ s.name }}</option>
          </select>
        </ov-form-field>
      </div>

      <div class="preview-banner" *ngIf="reportingManagerName">
        Reports to: <strong>{{ reportingManagerName }}</strong>
      </div>

      <div class="wizard-actions">
        <button type="button" class="btn btn-outline" (click)="cancel.emit()">Cancel</button>
        <button type="submit" class="btn btn-primary" [disabled]="!isValid()">Save & Next</button>
      </div>
    </form>
  `,
  styles: [`
    .form-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 0 24px; }
    .preview-banner {
      background: rgba(59, 130, 246, 0.08);
      border: 1px solid var(--border-color);
      border-radius: 6px;
      padding: 10px 14px;
      margin-bottom: 16px;
      font-size: 0.875rem;
    }
    .wizard-actions { display: flex; justify-content: flex-end; gap: 12px; margin-top: 8px; }
  `]
})
export class OnboardingStep1Component implements OnInit, OnChanges {
  private legalEntitiesApi = inject(LegalEntitiesApiService);
  private departmentsApi = inject(DepartmentsApiService);
  private positionsApi = inject(PositionsApiService);
  private schedulesApi = inject(WorkSchedulesApiService);
  private companyContext = inject(CompanyContextService);

  @Input() initialData!: Step1FormData;
  @Output() next = new EventEmitter<Step1FormData>();
  @Output() cancel = new EventEmitter<void>();

  data: Step1FormData = this.emptyData();
  departments: Option[] = [];
  positions: any[] = [];
  schedules: Option[] = [];
  reportingManagerName: string | null = null;

  private emptyData(): Step1FormData {
    return {
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
  }

  async ngOnInit() {
    const activeCompanyId = this.companyContext.activeCompanyId();
    if (activeCompanyId && !this.data.legalEntityId) {
      this.data.legalEntityId = activeCompanyId;
      await this.loadDependentOptions();
    }
  }

  async ngOnChanges() {
    if (this.initialData) {
      this.data = { ...this.initialData };
      if (this.data.legalEntityId) {
        await this.loadDependentOptions();
      }
    }
  }

  async onDepartmentChange() {
    this.data.positionId = null;
    this.reportingManagerName = null;
    if (this.data.legalEntityId) {
      const positions = (await firstValueFrom(
        this.positionsApi.list(this.data.legalEntityId, this.data.departmentId ?? undefined)
      )) as any[];
      this.positions = positions.map(p => ({ id: p.id, name: p.name }));
    }
  }

  async onPositionChange() {
    this.reportingManagerName = null;
    if (!this.data.positionId) {
      return;
    }

    // Auto-select the corresponding department if the user picked a position directly
    const selectedPosition = this.positions.find((p: any) => p.id === this.data.positionId);
    if (selectedPosition && selectedPosition.departmentId && this.data.departmentId !== selectedPosition.departmentId) {
      this.data.departmentId = selectedPosition.departmentId;
    }

    try {
      const manager = await firstValueFrom(this.positionsApi.getReportingManager(this.data.positionId));
      this.reportingManagerName = manager.hasManager ? (manager.employeeName ?? null) : null;
    } catch {
      this.reportingManagerName = null;
    }
  }

  private async loadDependentOptions() {
    if (!this.data.legalEntityId) {
      this.departments = [];
      this.positions = [];
      this.schedules = [];
      return;
    }
    const legalEntityId = this.data.legalEntityId;
    
    try {
      const depts = (await firstValueFrom(this.departmentsApi.list(legalEntityId))) as any[];
      this.departments = depts.map((d: any) => ({ id: d.id, name: d.name }));
    } catch (e) {
      console.error('Failed to load departments', e);
      this.departments = [];
    }

    try {
      const positions = (await firstValueFrom(
        this.positionsApi.list(legalEntityId, this.data.departmentId ?? undefined)
      )) as any[];
      this.positions = positions.map((p: any) => ({
        id: p.id,
        name: p.name,
        departmentId: p.departmentId,
        defaultScheduleId: p.defaultScheduleId
      }));
    } catch (e) {
      console.error('Failed to load positions', e);
      this.positions = [];
    }

    try {
      const schedules = (await firstValueFrom(this.schedulesApi.list(legalEntityId))) as any[];
      this.schedules = schedules.map((s: any) => ({ 
        id: s.id, 
        name: s.name, 
        defaultForNewEmployee: s.defaultForNewEmployee 
      }));
      
      // Auto-select default schedule if none is selected
      if (!this.data.scheduleId) {
        const defaultSchedule = this.schedules.find((s: any) => s.defaultForNewEmployee);
        if (defaultSchedule) {
          this.data.scheduleId = defaultSchedule.id;
        }
      }
    } catch (e) {
      console.error('Failed to load schedules', e);
      this.schedules = [];
    }
  }

  isValid(): boolean {
    return !!(
      this.data.employeeName &&
      this.data.workEmail &&
      this.data.employmentType &&
      this.data.startDate &&
      this.data.legalEntityId &&
      this.data.positionId
    );
  }

  onNext() {
    if (!this.isValid()) {
      return;
    }
    const withNames: Step1FormData = {
      ...this.data,
      companyName: this.data.companyName, // Usually we can fetch it or just keep whatever it was. Actually, let's keep it null if we don't have it locally, it's fine.
      departmentName: this.departments.find(d => d.id === this.data.departmentId)?.name ?? null,
      positionName: this.positions.find(p => p.id === this.data.positionId)?.name ?? null,
      scheduleName: this.schedules.find(s => s.id === this.data.scheduleId)?.name ?? null,
      reportingManagerName: this.reportingManagerName
    };
    this.next.emit(withNames);
  }
}
