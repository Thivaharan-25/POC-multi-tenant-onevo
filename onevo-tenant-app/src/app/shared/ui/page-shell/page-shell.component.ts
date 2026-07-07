import { Component } from '@angular/core';

@Component({
  selector: 'ov-page-shell',
  standalone: true,
  template: `
    <div class="ov-page-shell">
      <ng-content></ng-content>
    </div>
  `,
  styles: [`
    .ov-page-shell {
      display: flex;
      flex-direction: column;
      height: 100%;
      padding: 24px 32px;
      overflow-y: auto;
    }
  `]
})
export class PageShellComponent {}
