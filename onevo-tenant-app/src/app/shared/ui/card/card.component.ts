import { Component } from '@angular/core';

@Component({
  selector: 'ov-card',
  standalone: true,
  template: `
    <div class="ov-card">
      <ng-content></ng-content>
    </div>
  `,
  styles: [`
    .ov-card {
      background: var(--content-bg);
      color: var(--content-fg);
      border: 1px solid var(--border-color);
      border-radius: 8px;
      padding: 24px;
      box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
      margin-bottom: 16px;
    }
  `]
})
export class CardComponent {}
