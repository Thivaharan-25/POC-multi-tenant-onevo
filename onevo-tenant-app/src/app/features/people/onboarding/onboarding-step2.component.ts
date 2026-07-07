import { Component, EventEmitter, Input, OnChanges, Output, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';
import { OnboardingApiService, ChecklistTemplateSummary, ChecklistTemplateTask } from '../../../core/api/endpoints/onboarding-api.service';

export interface EditableTask {
  title: string;
  ownerType: string;
  dueDate: string | null;
  isRequired: boolean;
  isLocked: boolean;
}

export interface Step2Result {
  selectedTemplateId: string | null;
  editedTasksJson: string;
  templateName: string;
  taskCount: number;
  requiredTaskTitles: string[];
}

@Component({
  selector: 'app-onboarding-step2',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="wizard-form">
      <label class="field-label">Checklist Template</label>
      <select [(ngModel)]="selectedTemplateId" (ngModelChange)="onTemplateChange()">
        <option [ngValue]="null">No template — start blank</option>
        <option *ngFor="let t of templates()" [ngValue]="t.id">
          {{ t.name }}{{ t.id === recommendedTemplateId() ? ' (Recommended)' : '' }}
        </option>
      </select>

      <div class="task-list">
        <div class="task-row" *ngFor="let task of tasks(); let i = index">
          <input
            type="text"
            [(ngModel)]="task.title"
            name="title{{ i }}"
            placeholder="Task title"
            [disabled]="task.isLocked"
          />
          <select [(ngModel)]="task.ownerType" name="owner{{ i }}">
            <option value="employee">Employee</option>
            <option value="manager">Manager</option>
            <option value="hr">HR</option>
            <option value="it">IT</option>
          </select>
          <input type="date" [(ngModel)]="task.dueDate" name="due{{ i }}" />
          <label class="required-toggle">
            <input type="checkbox" [(ngModel)]="task.isRequired" name="req{{ i }}" [disabled]="task.isLocked" />
            Required
          </label>
          <button
            type="button"
            class="btn-icon"
            (click)="removeTask(i)"
            [disabled]="task.isRequired || task.isLocked"
          >
            Remove
          </button>
        </div>
      </div>

      <button type="button" class="btn btn-outline btn-sm" (click)="addTask()">Add Task</button>

      <div class="wizard-actions">
        <button type="button" class="btn btn-outline" (click)="back.emit()">Back</button>
        <button type="button" class="btn btn-primary" (click)="onNext()">Save & Next</button>
      </div>
    </div>
  `,
  styles: [`
    .field-label { display: block; font-size: 0.875rem; font-weight: 500; margin-bottom: 6px; }
    .task-list { margin: 16px 0; display: flex; flex-direction: column; gap: 10px; }
    .task-row {
      display: grid;
      grid-template-columns: 2fr 1fr 1fr auto auto;
      gap: 8px;
      align-items: center;
    }
    .required-toggle { display: flex; align-items: center; gap: 6px; font-size: 0.8125rem; }
    .wizard-actions { display: flex; justify-content: space-between; margin-top: 20px; }
  `]
})
export class OnboardingStep2Component implements OnChanges {
  private onboardingApi = inject(OnboardingApiService);

  @Input() draftId!: string;
  @Input() legalEntityId!: string;
  @Input() departmentId: string | null = null;
  @Input() initialTemplateId: string | null = null;
  @Input() initialTasksJson = '[]';
  @Output() back = new EventEmitter<void>();
  @Output() next = new EventEmitter<Step2Result>();

  templates = signal<ChecklistTemplateSummary[]>([]);
  recommendedTemplateId = signal<string | null>(null);
  tasks = signal<EditableTask[]>([]);
  selectedTemplateId: string | null = null;
  private loadedFromInitial = false;

  async ngOnChanges() {
    if (!this.legalEntityId || this.loadedFromInitial) {
      return;
    }
    this.loadedFromInitial = true;

    const list = await firstValueFrom(
      this.onboardingApi.getChecklistTemplates(this.legalEntityId, this.departmentId)
    );
    this.templates.set(list.templates);
    this.recommendedTemplateId.set(list.recommendedTemplateId);

    if (this.initialTemplateId) {
      this.selectedTemplateId = this.initialTemplateId;
    } else if (list.recommendedTemplateId) {
      this.selectedTemplateId = list.recommendedTemplateId;
    }

    if (this.initialTasksJson && this.initialTasksJson !== '[]') {
      this.tasks.set(this.parseTasksJson(this.initialTasksJson));
    } else if (this.selectedTemplateId) {
      await this.loadTasksFromTemplate(this.selectedTemplateId);
    }
  }

  async onTemplateChange() {
    if (this.selectedTemplateId) {
      await this.loadTasksFromTemplate(this.selectedTemplateId);
    } else {
      this.tasks.set([]);
    }
  }

  private async loadTasksFromTemplate(templateId: string) {
    const detail = await firstValueFrom(this.onboardingApi.getChecklistTemplateDetail(templateId));
    this.tasks.set(
      detail.tasks.map((t: ChecklistTemplateTask) => ({
        title: t.title,
        ownerType: t.ownerType || 'employee',
        dueDate: null,
        isRequired: t.isRequired,
        isLocked: t.isLocked
      }))
    );
  }

  private parseTasksJson(json: string): EditableTask[] {
    try {
      const parsed = JSON.parse(json) as any[];
      return parsed.map(t => ({
        title: t.title ?? '',
        ownerType: t.ownerType ?? 'employee',
        dueDate: t.dueDate ?? null,
        isRequired: !!t.isRequired,
        isLocked: !!t.isLocked
      }));
    } catch {
      return [];
    }
  }

  addTask() {
    this.tasks.update(list => [
      ...list,
      { title: '', ownerType: 'employee', dueDate: null, isRequired: false, isLocked: false }
    ]);
  }

  removeTask(index: number) {
    this.tasks.update(list => list.filter((_, i) => i !== index));
  }

  onNext() {
    const editedTasksJson = JSON.stringify(
      this.tasks()
        .filter(t => t.title.trim().length > 0)
        .map((t, i) => ({
          title: t.title,
          ownerType: t.ownerType,
          sequence: i + 1,
          dueDate: t.dueDate || null,
          isRequired: t.isRequired,
          isLocked: t.isLocked
        }))
    );
    const selectedTemplate = this.templates().find(t => t.id === this.selectedTemplateId);
    const requiredTaskTitles = this.tasks()
      .filter(t => t.isRequired || t.isLocked)
      .map(t => t.title);

    this.next.emit({
      selectedTemplateId: this.selectedTemplateId,
      editedTasksJson,
      templateName: selectedTemplate ? selectedTemplate.name : 'No template',
      taskCount: this.tasks().length,
      requiredTaskTitles
    });
  }
}
