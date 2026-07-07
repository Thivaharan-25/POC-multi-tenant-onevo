import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface SubSidebarItem {
  label: string;
  route: string;
  icon?: string;
  permission?: string;
  permissions?: string[];
  feature?: string;
}

@Component({
  selector: 'ov-sub-sidebar',
  standalone: true,
  imports: [CommonModule],
  template: `
    <aside class="ov-sub-sidebar">
      <div class="ov-sub-sidebar-header">
        <h2 class="ov-sub-sidebar-title">{{ title }}</h2>
      </div>
      <nav class="ov-sub-sidebar-nav">
        <ng-content></ng-content>
      </nav>
    </aside>
  `,
  styles: [`
    .ov-sub-sidebar {
      width: 260px;
      min-width: 260px;
      height: 100%;
      background: transparent;
      border-right: 1px solid var(--border-color);
      display: flex;
      flex-direction: column;
    }
    .ov-sub-sidebar-header {
      padding: 24px 20px 12px;
    }
    .ov-sub-sidebar-title {
      font-size: 0.85rem;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.05em;
      color: var(--content-fg);
      opacity: 0.5;
      margin: 0;
    }
    .ov-sub-sidebar-nav {
      padding: 0 12px;
      display: flex;
      flex-direction: column;
      gap: 4px;
      flex: 1;
      overflow-y: auto;
    }
    
    ::ng-deep .ov-sub-sidebar-nav a {
      display: flex;
      align-items: center;
      padding: 8px 12px;
      border-radius: 6px;
      color: var(--content-fg);
      text-decoration: none;
      font-size: 0.9rem;
      font-weight: 500;
      transition: background-color 0.2s, color 0.2s;
    }
    
    ::ng-deep .ov-sub-sidebar-nav a:hover {
      background: rgba(128, 128, 128, 0.1);
    }
    
    ::ng-deep .ov-sub-sidebar-nav a.active {
      background: rgba(128, 128, 128, 0.15);
      color: var(--content-fg);
      font-weight: 600;
    }
    
    ::ng-deep .ov-sub-sidebar-nav svg {
      width: 18px;
      height: 18px;
      margin-right: 10px;
      opacity: 0.7;
    }
    ::ng-deep .ov-sub-sidebar-nav a.active svg {
      opacity: 1;
    }
  `]
})
export class SubSidebarComponent {
  @Input() title: string = 'Section';
}
