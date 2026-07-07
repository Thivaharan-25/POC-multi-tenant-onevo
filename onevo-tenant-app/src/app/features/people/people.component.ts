import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';
import { PageShellComponent } from '../../shared/ui/page-shell/page-shell.component';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header.component';
import { CardComponent } from '../../shared/ui/card/card.component';
import { StatusPillComponent } from '../../shared/ui/status-pill/status-pill.component';
import { HasPermissionDirective } from '../../shared/directives/has-permission.directive';
import { EmployeesApiService, EmployeeListItem } from '../../core/api/endpoints/employees-api.service';
import { DepartmentsApiService } from '../../core/api/endpoints/departments-api.service';
import { LegalEntitiesApiService } from '../../core/api/endpoints/legal-entities-api.service';
import { OnboardingWizardComponent } from './onboarding/onboarding-wizard.component';
import { MyDraftsDrawerComponent } from './onboarding/my-drafts-drawer.component';

interface DepartmentOption {
  id: string;
  name: string;
}

@Component({
  selector: 'app-people',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    PageShellComponent,
    PageHeaderComponent,
    CardComponent,
    StatusPillComponent,
    HasPermissionDirective,
    OnboardingWizardComponent,
    MyDraftsDrawerComponent
  ],
  template: `
    <ov-page-shell>
      <ov-page-header title="Employees" subtitle="People visible to you based on your role and coverage.">
        <div actions>
          <button type="button" class="btn btn-outline" *hasPermission="'employees:write'" (click)="openMyDrafts()">
            My Drafts
          </button>
          <button type="button" class="btn btn-primary" *hasPermission="'employees:write'" (click)="openAddEmployee()">
            Add Employee
          </button>
        </div>
      </ov-page-header>

      <ov-card>
        <div class="filters-row">
          <input
            type="text"
            class="search-input"
            placeholder="Search by name, email, or employee number"
            [(ngModel)]="search"
            (ngModelChange)="onFiltersChanged()"
          />
          <select [(ngModel)]="statusFilter" (ngModelChange)="onFiltersChanged()">
            <option value="">All statuses</option>
            <option value="draft">Draft</option>
            <option value="active">Active</option>
            <option value="inactive">Inactive</option>
          </select>
          <select [(ngModel)]="departmentFilter" (ngModelChange)="onFiltersChanged()">
            <option value="">All departments</option>
            <option *ngFor="let d of departments()" [value]="d.id">{{ d.name }}</option>
          </select>
        </div>

        <div *ngIf="loading()" class="state-message">Loading employees…</div>
        <div *ngIf="!loading() && error()" class="state-message error">{{ error() }}</div>
        <div *ngIf="!loading() && !error() && employees().length === 0" class="state-message">
          No employees match your current view.
        </div>

        <table class="employees-table" *ngIf="!loading() && employees().length > 0">
          <thead>
            <tr>
              <th>Name</th>
              <th>Work Email</th>
              <th>Employee #</th>
              <th>Position</th>
              <th>Department</th>
              <th>Status</th>
              <th>Start Date</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let e of employees()">
              <td>{{ e.firstName }} {{ e.lastName }}</td>
              <td>{{ e.workEmail }}</td>
              <td>{{ e.employeeNumber }}</td>
              <td>{{ e.positionName || '—' }}</td>
              <td>{{ e.departmentName || '—' }}</td>
              <td><ov-status-pill [status]="statusPillFor(e.status)">{{ e.status }}</ov-status-pill></td>
              <td>{{ e.hireDate | date: 'mediumDate' }}</td>
              <td></td>
            </tr>
          </tbody>
        </table>
      </ov-card>
    </ov-page-shell>

    <app-onboarding-wizard
      *ngIf="wizardOpen()"
      mode="modal"
      [initialDraftId]="wizardDraftId()"
      (closed)="closeWizard()"
      (saved)="onWizardSaved()"
    ></app-onboarding-wizard>

    <app-my-drafts-drawer
      *ngIf="myDraftsOpen()"
      (closed)="myDraftsOpen.set(false)"
      (continueDraft)="continueDraft($event)"
    ></app-my-drafts-drawer>
  `,
  styles: [`
    .filters-row { display: flex; gap: 12px; margin-bottom: 20px; }
    .search-input {
      flex: 1;
      padding: 8px 12px;
      border: 1px solid var(--border-color);
      border-radius: 6px;
      background: transparent;
      color: var(--content-fg);
    }
    .filters-row select {
      padding: 8px 12px;
      border: 1px solid var(--border-color);
      border-radius: 6px;
      background: transparent;
      color: var(--content-fg);
    }
    .state-message { color: var(--shell-muted-fg); padding: 24px 0; text-align: center; }
    .state-message.error { color: #dc2626; }
    .employees-table { width: 100%; border-collapse: collapse; }
    .employees-table th {
      text-align: left;
      font-size: 0.75rem;
      text-transform: uppercase;
      color: var(--shell-muted-fg);
      padding: 8px 12px;
      border-bottom: 1px solid var(--border-color);
    }
    .employees-table td {
      padding: 12px;
      border-bottom: 1px solid var(--border-color);
      font-size: 0.875rem;
    }
  `]
})
export class PeopleComponent implements OnInit {
  private employeesApi = inject(EmployeesApiService);
  private departmentsApi = inject(DepartmentsApiService);
  private legalEntitiesApi = inject(LegalEntitiesApiService);

  employees = signal<EmployeeListItem[]>([]);
  departments = signal<DepartmentOption[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  search = '';
  statusFilter = '';
  departmentFilter = '';

  wizardOpen = signal(false);
  wizardDraftId = signal<string | null>(null);
  myDraftsOpen = signal(false);

  async ngOnInit() {
    await this.loadDepartments();
    await this.loadEmployees();
  }

  private async loadDepartments() {
    try {
      const legalEntities = (await firstValueFrom(this.legalEntitiesApi.list())) as any[];
      const allDepartments: DepartmentOption[] = [];
      for (const le of legalEntities) {
        const depts = (await firstValueFrom(this.departmentsApi.list(le.id))) as any[];
        for (const d of depts) {
          allDepartments.push({ id: d.id, name: d.name });
        }
      }
      this.departments.set(allDepartments);
    } catch {
      this.departments.set([]);
    }
  }

  async loadEmployees() {
    this.loading.set(true);
    this.error.set(null);
    try {
      const result = await firstValueFrom(
        this.employeesApi.list({
          search: this.search || undefined,
          status: this.statusFilter || undefined,
          departmentId: this.departmentFilter || undefined
        })
      );
      this.employees.set(result);
    } catch {
      this.error.set('Could not load employees. Please try again.');
    } finally {
      this.loading.set(false);
    }
  }

  onFiltersChanged() {
    this.loadEmployees();
  }

  statusPillFor(status: string): 'success' | 'warning' | 'danger' | 'neutral' | 'info' {
    switch (status) {
      case 'active':
        return 'success';
      case 'draft':
        return 'warning';
      case 'inactive':
        return 'neutral';
      default:
        return 'info';
    }
  }

  openAddEmployee() {
    this.wizardDraftId.set(null);
    this.wizardOpen.set(true);
  }

  openMyDrafts() {
    this.myDraftsOpen.set(true);
  }

  continueDraft(draftId: string) {
    this.myDraftsOpen.set(false);
    this.wizardDraftId.set(draftId);
    this.wizardOpen.set(true);
  }

  closeWizard() {
    this.wizardOpen.set(false);
    this.wizardDraftId.set(null);
  }

  onWizardSaved() {
    this.closeWizard();
    this.loadEmployees();
  }
}
