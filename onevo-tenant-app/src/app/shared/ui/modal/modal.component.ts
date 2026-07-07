import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'ov-modal',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="ov-modal-backdrop" *ngIf="open" (click)="onBackdropClick()">
      <div
        class="ov-modal-panel"
        [class.ov-modal-drawer]="variant === 'drawer'"
        (click)="$event.stopPropagation()"
      >
        <div class="ov-modal-header" *ngIf="title">
          <h2 class="ov-modal-title">{{ title }}</h2>
          <button type="button" class="ov-modal-close" (click)="close()" aria-label="Close">&times;</button>
        </div>
        <div class="ov-modal-body">
          <ng-content></ng-content>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .ov-modal-backdrop {
      position: fixed;
      inset: 0;
      background: rgba(0, 0, 0, 0.5);
      display: flex;
      justify-content: center;
      align-items: center;
      z-index: 1000;
    }
    .ov-modal-panel {
      background: var(--content-bg);
      color: var(--content-fg);
      border: 1px solid var(--border-color);
      border-radius: 8px;
      width: 640px;
      max-width: calc(100vw - 32px);
      max-height: calc(100vh - 64px);
      display: flex;
      flex-direction: column;
      overflow: hidden;
      box-shadow: 0 10px 40px rgba(0, 0, 0, 0.25);
    }
    .ov-modal-panel.ov-modal-drawer {
      position: fixed;
      top: 0;
      right: 0;
      height: 100vh;
      max-height: 100vh;
      width: 420px;
      border-radius: 0;
      border-right: none;
      border-top: none;
      border-bottom: none;
    }
    .ov-modal-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 20px 24px;
      border-bottom: 1px solid var(--border-color);
    }
    .ov-modal-title {
      font-size: 1.125rem;
      font-weight: 600;
      margin: 0;
    }
    .ov-modal-close {
      background: transparent;
      border: none;
      color: var(--content-fg);
      font-size: 1.5rem;
      line-height: 1;
      cursor: pointer;
      padding: 4px;
    }
    .ov-modal-body {
      padding: 24px;
      overflow-y: auto;
      flex: 1;
    }
  `]
})
export class ModalComponent {
  @Input() open = false;
  @Input() title?: string;
  @Input() variant: 'dialog' | 'drawer' = 'dialog';
  @Input() dismissible = true;
  @Output() closed = new EventEmitter<void>();

  onBackdropClick() {
    if (this.dismissible) {
      this.close();
    }
  }

  close() {
    this.closed.emit();
  }
}
