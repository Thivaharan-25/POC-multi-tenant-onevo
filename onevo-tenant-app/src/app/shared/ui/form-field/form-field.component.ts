import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'ov-form-field',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="ov-form-field">
      <label class="ov-label" [class.required]="required" *ngIf="label">
        {{ label }} <span *ngIf="required" class="ov-req-star">*</span>
      </label>
      <div class="ov-control-wrapper">
        <ng-content></ng-content>
      </div>
      <div class="ov-hint" *ngIf="hint">{{ hint }}</div>
      <div class="ov-error" *ngIf="error">{{ error }}</div>
    </div>
  `,
  styles: [`
    .ov-form-field {
      display: flex;
      flex-direction: column;
      margin-bottom: 16px;
      width: 100%;
    }
    .ov-label {
      font-size: 0.875rem;
      font-weight: 500;
      color: var(--content-fg);
      margin-bottom: 6px;
    }
    .ov-req-star {
      color: #ef4444; /* red-500 */
    }
    .ov-control-wrapper {
      position: relative;
    }
    .ov-hint {
      font-size: 0.75rem;
      color: var(--shell-muted-fg);
      margin-top: 4px;
    }
    .ov-error {
      font-size: 0.75rem;
      color: #ef4444;
      margin-top: 4px;
    }
    
    /* Global styles for native inputs inside ov-form-field */
    ::ng-deep .ov-form-field input,
    ::ng-deep .ov-form-field select,
    ::ng-deep .ov-form-field textarea {
      width: 100%;
      padding: 8px 12px;
      border: 1px solid var(--border-color);
      border-radius: 6px;
      background: transparent;
      color: var(--content-fg);
      font-family: inherit;
      font-size: 0.875rem;
      transition: border-color 0.2s, box-shadow 0.2s;
    }
    ::ng-deep .ov-form-field input:focus,
    ::ng-deep .ov-form-field select:focus,
    ::ng-deep .ov-form-field textarea:focus {
      outline: none;
      border-color: var(--primary);
      box-shadow: 0 0 0 1px var(--primary);
    }
    ::ng-deep .ov-form-field input:disabled,
    ::ng-deep .ov-form-field select:disabled {
      opacity: 0.6;
      cursor: not-allowed;
      background: rgba(0,0,0,0.02);
    }
  `]
})
export class FormFieldComponent {
  @Input() label?: string;
  @Input() required: boolean = false;
  @Input() hint?: string;
  @Input() error?: string;
}
