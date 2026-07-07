import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-checklist',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div style="padding: 24px;">
      <h2>Checklist</h2>
      <p style="color: var(--shell-muted-fg); margin-top: 8px;">Feature placeholder: Checklist module not yet implemented.</p>
    </div>
  `
})
export class ChecklistComponent {}
