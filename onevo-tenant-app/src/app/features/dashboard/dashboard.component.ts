import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../core/auth/auth.service';
import { AppContextStore } from '../../core/context/app-context.store';
import { environment } from '../../../environments/environment';
import { PageShellComponent } from '../../shared/ui/page-shell/page-shell.component';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header.component';
import { CardComponent } from '../../shared/ui/card/card.component';
import { StatusPillComponent } from '../../shared/ui/status-pill/status-pill.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, PageShellComponent, CardComponent, StatusPillComponent],
  template: `
    <ov-page-shell>
      <div class="dashboard-grid">
        <!-- Live API Status Diagnostics -->
        <ov-card class="full-width">
          <div class="card-header">
            <svg class="card-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z"/>
            </svg>
            <h3>Live Backend Connection Verification</h3>
          </div>
          <div class="card-body diagnostic-grid">
            <div class="diagnostic-item">
              <span class="endpoint-label">GET /api/v1/auth/session</span>
              <ov-status-pill [status]="authService.sessionApiStatus().status === 'success' ? 'success' : (authService.sessionApiStatus().status === 'error' ? 'danger' : 'neutral')">
                {{ authService.sessionApiStatus().status | uppercase }}
                <span *ngIf="authService.sessionApiStatus().statusCode" class="code-suffix">
                  ({{ authService.sessionApiStatus().statusCode }})
                </span>
              </ov-status-pill>
            </div>

            <div class="diagnostic-item">
              <span class="endpoint-label">GET /api/v1/tenant/resource-limits</span>
              <ov-status-pill [status]="limitsApiStatus().status === 'success' ? 'success' : (limitsApiStatus().status === 'error' ? 'danger' : 'neutral')">
                {{ limitsApiStatus().status | uppercase }}
                <span *ngIf="limitsApiStatus().statusCode" class="code-suffix">
                  ({{ limitsApiStatus().statusCode }})
                </span>
              </ov-status-pill>
            </div>

            <div class="diagnostic-item">
              <span class="endpoint-label">GET /api/v1/outbox/recent</span>
              <ov-status-pill [status]="outboxApiStatus().status === 'success' ? 'success' : (outboxApiStatus().status === 'error' ? 'danger' : 'neutral')">
                {{ outboxApiStatus().status | uppercase }}
                <span *ngIf="outboxApiStatus().statusCode" class="code-suffix">
                  ({{ outboxApiStatus().statusCode }})
                </span>
              </ov-status-pill>
            </div>
          </div>
        </ov-card>

        <!-- Section 1: Active Session & User Info -->
        <ov-card>
          <div class="card-header">
            <svg class="card-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"/>
            </svg>
            <h3>Session & User Info</h3>
          </div>
          <div class="card-body" *ngIf="authService.user() as user">
            <div class="info-row">
              <span class="label">Display Name:</span>
              <span class="value">{{ user.displayName }}</span>
            </div>
            <div class="info-row">
              <span class="label">Email:</span>
              <span class="value">{{ user.email }}</span>
            </div>
            <div class="info-row">
              <span class="label">User ID:</span>
              <span class="value code">{{ user.id }}</span>
            </div>
            <div class="info-row">
              <span class="label">Tenant ID:</span>
              <span class="value code">{{ user.tenantId }}</span>
            </div>
            <div class="info-row">
              <span class="label">Dev Tenant Domain:</span>
              <span class="value tag">{{ devTenantDomain }}</span>
            </div>
            <div class="info-row">
              <span class="label">Setup Complete:</span>
              <ov-status-pill [status]="setupComplete() ? 'success' : 'danger'">
                {{ setupComplete() ? 'Yes' : 'No' }}
              </ov-status-pill>
            </div>
          </div>
        </ov-card>

        <!-- Section 2: Resource Limits & Quotas -->
        <ov-card>
          <div class="card-header">
            <svg class="card-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 002 2h2a2 2 0 002-2z"/>
            </svg>
            <h3>Tenant Quotas & Limits</h3>
          </div>
          <div class="card-body">
            <div *ngIf="limitsError()" class="error-box">
              {{ limitsError() }}
            </div>
            <div *ngIf="!limitsError() && resourceLimits().length === 0" class="no-data">Loading resource limits...</div>
            <div *ngIf="!limitsError() && resourceLimits().length > 0">
              <div *ngFor="let limit of resourceLimits()" class="limit-item">
                <div class="info-row">
                  <span class="label">Storage Limit:</span>
                  <span class="value">{{ limit.storageLimitGb }} GB</span>
                </div>
                <div class="info-row">
                  <span class="label">AI Token Limit:</span>
                  <span class="value">{{ limit.aiTokenLimit | number }} tokens</span>
                </div>
                <div class="info-row">
                  <span class="label">Employee Limit:</span>
                  <span class="value">{{ limit.employeeLimit ?? 'Unlimited' }}</span>
                </div>
                <div class="info-row">
                  <span class="label">Limit Source:</span>
                  <span class="value tag">{{ limit.source }}</span>
                </div>
              </div>
            </div>
          </div>
        </ov-card>

        <!-- Section 3: Active Modules & Feature Entitlements -->
        <ov-card>
          <div class="card-header">
            <svg class="card-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10"/>
            </svg>
            <h3>Entitlements & Features</h3>
          </div>
          <div class="card-body">
            <div class="section-title">Active Modules</div>
            <div class="tag-cloud">
              <span class="tag module-tag" *ngFor="let mod of activeModules()">{{ mod }}</span>
            </div>

            <div class="section-title margin-top">Active Features</div>
            <div class="tag-cloud">
              <span class="tag feature-tag" *ngFor="let feat of activeFeatures()">{{ feat }}</span>
            </div>
          </div>
        </ov-card>

        <!-- Section 4: Permissions -->
        <ov-card>
          <div class="card-header">
            <svg class="card-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z"/>
            </svg>
            <h3>Effective Permissions</h3>
          </div>
          <div class="card-body">
            <div class="tag-cloud">
              <span class="tag permission-tag" *ngFor="let perm of permissions()">{{ perm }}</span>
            </div>
          </div>
        </ov-card>

        <!-- Section 5: Latest Outbox Messages -->
        <ov-card class="full-width">
          <div class="card-header">
            <svg class="card-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 4H6a2 2 0 00-2 2v12a2 2 0 002 2h12a2 2 0 002-2V6a2 2 0 00-2-2h-2m-4-1v8m0 0l3-3m-3 3L9 8m-5 5h2.586a1 1 0 01.707.293l2.414 2.414a1 1 0 00.707.293h3.172a1 1 0 00.707-.293l2.414-2.414a1 1 0 01.707-.293H20"/>
            </svg>
            <h3>System Outbox Log (Recent Messages)</h3>
            <button class="refresh-btn" (click)="loadOutboxMessages()">Refresh</button>
          </div>
          <div class="card-body">
            <div *ngIf="outboxError()" class="error-box">
              {{ outboxError() }}
            </div>
            <div *ngIf="!outboxError() && outboxMessages().length === 0" class="no-data">No outbox messages found or missing permission.</div>
            <div class="table-container" *ngIf="!outboxError() && outboxMessages().length > 0">
              <table class="outbox-table">
                <thead>
                  <tr>
                    <th>Type</th>
                    <th>Message ID</th>
                    <th>Created At</th>
                    <th>Payload JSON</th>
                  </tr>
                </thead>
                <tbody>
                  <tr *ngFor="let msg of outboxMessages()">
                    <td class="msg-type"><span class="type-badge">{{ msg.type }}</span></td>
                    <td class="code msg-id">{{ msg.id }}</td>
                    <td class="msg-time">{{ msg.createdAtUtc | date:'yyyy-MM-dd HH:mm:ss' }}</td>
                    <td class="code msg-payload">{{ msg.payloadJson }}</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </ov-card>
      </div>
    </ov-page-shell>
  `,
  styles: [`
    .dashboard-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(400px, 1fr));
      gap: 1.5rem;
    }
    ::ng-deep .full-width {
      grid-column: 1 / -1;
    }
    .diagnostic-grid {
      display: flex;
      flex-direction: column;
      gap: 0.75rem;
    }
    .diagnostic-item {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 0.5rem 1rem;
      border: 1px solid var(--border-color);
    }
    .endpoint-label {
      font-family: 'JetBrains Mono', monospace;
      font-size: 0.85rem;
      color: var(--content-fg);
      opacity: 0.7;
    }
    .code-suffix {
      font-weight: normal;
      opacity: 0.8;
      margin-left: 0.2rem;
    }
    @keyframes pulse {
      0%, 100% { opacity: 1; }
      50% { opacity: 0.5; }
    }
    .error-box {
      background: rgba(239, 68, 68, 0.08);
      border: 1px solid rgba(239, 68, 68, 0.2);
      color: #fca5a5;
      padding: 0.75rem 1rem;
      border-radius: 8px;
      font-size: 0.85rem;
      margin-bottom: 1rem;
    }
    .card-header {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      margin-bottom: 1.25rem;
      border-bottom: 1px solid var(--border-color);
      padding-bottom: 0.75rem;
    }
    .card-icon {
      width: 20px;
      height: 20px;
      color: var(--primary);
    }
    h3 {
      font-size: 1.15rem;
      font-weight: 600;
      color: var(--content-fg);
      margin: 0;
      flex-grow: 1;
    }
    .info-row {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 0.5rem 0;
      border-bottom: 1px solid var(--border-color);
    }
    .label {
      color: var(--content-fg);
      opacity: 0.7;
      font-size: 0.875rem;
    }
    .value {
      color: var(--content-fg);
      font-weight: 500;
      font-size: 0.9rem;
    }
    .code {
      font-family: 'JetBrains Mono', 'Fira Code', monospace;
      font-size: 0.8rem;
      background: rgba(128, 128, 128, 0.1);
      padding: 0.2rem 0.4rem;
      border-radius: 4px;
      border: 1px solid var(--border-color);
    }
    .tag {
      font-size: 0.75rem;
      padding: 0.2rem 0.5rem;
      border-radius: 9999px;
      font-weight: 600;
    }
    .module-tag {
      background: rgba(16, 185, 129, 0.1);
      color: #059669;
    }
    .feature-tag {
      background: rgba(99, 102, 241, 0.1);
      color: #4f46e5;
    }
    .permission-tag {
      background: rgba(245, 158, 11, 0.1);
      color: #d97706;
    }
    :host-context(.dark) .module-tag { color: #34d399; }
    :host-context(.dark) .feature-tag { color: #818cf8; }
    :host-context(.dark) .permission-tag { color: #fbbf24; }
    .tag-cloud {
      display: flex;
      flex-wrap: wrap;
      gap: 0.5rem;
    }
    .section-title {
      font-size: 0.875rem;
      font-weight: 600;
      color: var(--content-fg);
      opacity: 0.7;
      margin-bottom: 0.5rem;
      text-transform: uppercase;
      letter-spacing: 0.05em;
    }
    .margin-top {
      margin-top: 1.25rem;
    }
    .limit-item {
      display: flex;
      flex-direction: column;
      gap: 0.25rem;
      margin-bottom: 1rem;
      background: rgba(15, 23, 42, 0.05);
      padding: 0.75rem;
      border-radius: 8px;
    }
    :host-context(.dark) .limit-item {
      background: rgba(15, 23, 42, 0.2);
    }
    .no-data {
      color: #64748b;
      font-size: 0.875rem;
      text-align: center;
      padding: 1.5rem;
    }
    .refresh-btn {
      background: rgba(128, 128, 128, 0.05);
      border: 1px solid var(--border-color);
      color: var(--content-fg);
      padding: 0.35rem 0.75rem;
      border-radius: 6px;
      font-size: 0.8rem;
      cursor: pointer;
      transition: all 0.2s;
    }
    .refresh-btn:hover {
      background: rgba(128, 128, 128, 0.1);
    }
    .table-container {
      overflow-x: auto;
      background: transparent;
      border-radius: 8px;
      border: 1px solid var(--border-color);
    }
    .outbox-table {
      width: 100%;
      border-collapse: collapse;
      text-align: left;
      font-size: 0.85rem;
    }
    .outbox-table th, .outbox-table td {
      padding: 0.75rem 1rem;
      border-bottom: 1px solid var(--border-color);
    }
    .outbox-table th {
      background: rgba(128, 128, 128, 0.05);
      color: var(--content-fg);
      opacity: 0.7;
      font-weight: 600;
    }
    .type-badge {
      background: rgba(56, 189, 248, 0.15);
      color: #38bdf8;
      padding: 0.15rem 0.4rem;
      border-radius: 4px;
      font-weight: 600;
    }
    .msg-payload {
      max-width: 400px;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    }
  `]
})
export class DashboardComponent implements OnInit {
  private http = inject(HttpClient);
  authService = inject(AuthService);
  private appContextStore = inject(AppContextStore);

  devTenantDomain = environment.tenantDomain;

  setupComplete = signal(false);
  permissions = signal<string[]>([]);
  activeModules = signal<string[]>([]);
  activeFeatures = signal<string[]>([]);
  resourceLimits = signal<any[]>([]);
  outboxMessages = signal<any[]>([]);

  limitsApiStatus = signal<{ status: 'idle' | 'loading' | 'success' | 'error', statusCode?: number, error?: string }>({ status: 'idle' });
  outboxApiStatus = signal<{ status: 'idle' | 'loading' | 'success' | 'error', statusCode?: number, error?: string }>({ status: 'idle' });

  limitsError = signal<string | null>(null);
  outboxError = signal<string | null>(null);

  ngOnInit(): void {
    this.setupComplete.set(this.appContextStore.setupComplete());
    this.permissions.set(this.appContextStore.permissions() || []);
    this.activeModules.set(this.appContextStore.activeModules() || []);
    this.activeFeatures.set(this.appContextStore.activeFeatures() || []);

    this.loadResourceLimits();
    this.loadOutboxMessages();
  }

  loadResourceLimits(): void {
    this.limitsApiStatus.set({ status: 'loading' });
    this.limitsError.set(null);
    this.http.get<any[]>('/api/v1/tenant/resource-limits').subscribe({
      next: (limits) => {
        this.resourceLimits.set(limits);
        this.limitsApiStatus.set({ status: 'success' });
      },
      error: (err) => {
        this.limitsApiStatus.set({ 
          status: 'error', 
          statusCode: err.status, 
          error: err.error?.error || err.message || 'Failed to load resource limits' 
        });
        this.limitsError.set(err.error?.error || `HTTP ${err.status}: ${err.statusText || 'Error'}`);
      }
    });
  }

  loadOutboxMessages(): void {
    this.outboxApiStatus.set({ status: 'loading' });
    this.outboxError.set(null);
    this.http.get<any[]>('/api/v1/outbox/recent').subscribe({
      next: (msgs) => {
        this.outboxMessages.set(msgs);
        this.outboxApiStatus.set({ status: 'success' });
      },
      error: (err) => {
        this.outboxApiStatus.set({ 
          status: 'error', 
          statusCode: err.status, 
          error: err.error?.error || err.message || 'Failed to load outbox messages' 
        });
        this.outboxError.set(err.error?.error || `HTTP ${err.status}: ${err.statusText || 'Error'}`);
      }
    });
  }
}
