import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'ov-status-pill',
  standalone: true,
  imports: [CommonModule],
  template: `
    <span class="ov-status-pill" [ngClass]="'ov-status-' + status">
      <ng-content></ng-content>
    </span>
  `,
  styles: [`
    .ov-status-pill {
      display: inline-flex;
      align-items: center;
      padding: 2px 8px;
      border-radius: 9999px;
      font-size: 0.75rem;
      font-weight: 600;
      line-height: 1.25rem;
    }
    .ov-status-success {
      background: rgba(16, 185, 129, 0.1);
      color: #059669;
    }
    .ov-status-warning {
      background: rgba(245, 158, 11, 0.1);
      color: #d97706;
    }
    .ov-status-danger {
      background: rgba(239, 68, 68, 0.1);
      color: #dc2626;
    }
    .ov-status-neutral {
      background: rgba(107, 114, 128, 0.1);
      color: #4b5563;
    }
    .ov-status-info {
      background: rgba(59, 130, 246, 0.1);
      color: #2563eb;
    }
    
    :host-context(.dark) .ov-status-success { color: #34d399; }
    :host-context(.dark) .ov-status-warning { color: #fbbf24; }
    :host-context(.dark) .ov-status-danger { color: #fca5a5; }
    :host-context(.dark) .ov-status-neutral { color: #9ca3af; }
    :host-context(.dark) .ov-status-info { color: #60a5fa; }
  `]
})
export class StatusPillComponent {
  @Input() status: 'success' | 'warning' | 'danger' | 'neutral' | 'info' = 'neutral';
}
