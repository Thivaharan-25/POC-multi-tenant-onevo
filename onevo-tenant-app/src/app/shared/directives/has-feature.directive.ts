import { Directive, TemplateRef, ViewContainerRef, effect, inject, input } from '@angular/core';
import { PermissionService } from '../../core/auth/permission.service';

@Directive({
  selector: '[hasFeature]',
  standalone: true
})
export class HasFeatureDirective {
  private permissions = inject(PermissionService);
  private template = inject(TemplateRef<unknown>);
  private view = inject(ViewContainerRef);

  hasFeature = input.required<string>();

  constructor() {
    effect(() => {
      this.view.clear();
      if (this.permissions.hasFeature(this.hasFeature())) {
        this.view.createEmbeddedView(this.template);
      }
    });
  }
}
