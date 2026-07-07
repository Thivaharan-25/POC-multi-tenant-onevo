import { Component } from '@angular/core';
import { PageShellComponent } from '../../../shared/ui/page-shell/page-shell.component';
import { OnboardingWizardComponent } from './onboarding-wizard.component';

@Component({
  selector: 'app-onboarding',
  standalone: true,
  imports: [PageShellComponent, OnboardingWizardComponent],
  template: `
    <ov-page-shell>
      <app-onboarding-wizard mode="inline"></app-onboarding-wizard>
    </ov-page-shell>
  `
})
export class OnboardingComponent {}
