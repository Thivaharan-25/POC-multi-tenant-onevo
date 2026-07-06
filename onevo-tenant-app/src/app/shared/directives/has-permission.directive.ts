import { Directive, Input, TemplateRef, ViewContainerRef, effect, inject } from '@angular/core';
import { PermissionService } from '../../core/auth/permission.service';

@Directive({
  selector: '[hasPermission]',
  standalone: true
})
export class HasPermissionDirective {
  private permissions = inject(PermissionService);
  private template = inject(TemplateRef<unknown>);
  private view = inject(ViewContainerRef);
  private permission = '';

  constructor() {
    effect(() => {
      this.view.clear();
      if (this.permission && this.permissions.hasPermission(this.permission)()) {
        this.view.createEmbeddedView(this.template);
      }
    });
  }

  @Input() set hasPermission(permission: string) {
    this.permission = permission;
  }
}
