import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'ov-page-header',
  standalone: true,
  imports: [CommonModule],
  template: `
    <header class="ov-page-header">
      <div class="ov-page-header-title-area">
        <h1 class="ov-page-title">{{ title }}</h1>
        <p *ngIf="subtitle" class="ov-page-subtitle">{{ subtitle }}</p>
      </div>
      <div class="ov-page-header-actions">
        <ng-content select="[actions]"></ng-content>
      </div>
    </header>
  `,
  styles: [`
    .ov-page-header {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      margin-bottom: 24px;
    }
    .ov-page-title {
      font-size: 1.5rem;
      font-weight: 600;
      color: var(--content-fg);
      margin: 0 0 4px 0;
    }
    .ov-page-subtitle {
      font-size: 0.875rem;
      color: var(--shell-muted-fg);
      margin: 0;
    }
    .ov-page-header-actions {
      display: flex;
      gap: 12px;
    }
  `]
})
export class PageHeaderComponent {
  @Input() title!: string;
  @Input() subtitle?: string;
}
