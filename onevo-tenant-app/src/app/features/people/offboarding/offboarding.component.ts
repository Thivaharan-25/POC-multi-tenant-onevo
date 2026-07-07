import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-offboarding',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div style="padding: 24px;">
      <h2>Offboarding</h2>
      <p style="color: var(--shell-muted-fg); margin-top: 8px;">Feature placeholder: Offboarding module not yet implemented.</p>
    </div>
  `
})
export class OffboardingComponent {}
