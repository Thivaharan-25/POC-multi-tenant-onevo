import { Injectable, signal } from '@angular/core';
import { SessionDto } from '../../shared/models/session.model';

@Injectable({ providedIn: 'root' })
export class AppContextStore {
  private permissionsState = signal<string[]>([]);
  private activeModulesState = signal<string[]>([]);
  private activeFeaturesState = signal<string[]>([]);
  private setupCompleteState = signal(true);

  permissions = this.permissionsState.asReadonly();
  activeModules = this.activeModulesState.asReadonly();
  activeFeatures = this.activeFeaturesState.asReadonly();
  setupComplete = this.setupCompleteState.asReadonly();

  setSession(session: SessionDto): void {
    this.permissionsState.set(session.permissions);
    this.activeModulesState.set(session.activeModules);
    this.activeFeaturesState.set(session.activeFeatures);
    this.setupCompleteState.set(session.setupComplete ?? true);
  }

  clear(): void {
    this.permissionsState.set([]);
    this.activeModulesState.set([]);
    this.activeFeaturesState.set([]);
    this.setupCompleteState.set(false);
  }
}
