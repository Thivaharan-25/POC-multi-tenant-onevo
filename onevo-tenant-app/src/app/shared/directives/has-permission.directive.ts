import { Directive, TemplateRef, ViewContainerRef, effect, inject, input } from '@angular/core';
import { PermissionService } from '../../core/auth/permission.service';

@Directive({
  selector: '[hasPermission]',
  standalone: true
})
export class HasPermissionDirective {
  private permissions = inject(PermissionService);
  private template = inject(TemplateRef<unknown>);
  private view = inject(ViewContainerRef);

  hasPermission = input.required<string>();

  constructor() {
    effect(() => {
      this.view.clear();
      if (this.permissions.hasPermission(this.hasPermission())) {
        this.view.createEmbeddedView(this.template);
      }
    });
  }
}
