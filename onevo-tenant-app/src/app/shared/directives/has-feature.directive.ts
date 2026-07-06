import { Directive, Input, TemplateRef, ViewContainerRef, effect, inject } from '@angular/core';
import { PermissionService } from '../../core/auth/permission.service';

@Directive({
  selector: '[hasFeature]',
  standalone: true
})
export class HasFeatureDirective {
  private permissions = inject(PermissionService);
  private template = inject(TemplateRef<unknown>);
  private view = inject(ViewContainerRef);
  private feature = '';

  constructor() {
    effect(() => {
      this.view.clear();
      if (this.feature && this.permissions.hasFeature(this.feature)()) {
        this.view.createEmbeddedView(this.template);
      }
    });
  }

  @Input() set hasFeature(feature: string) {
    this.feature = feature;
  }
}
