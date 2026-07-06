import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../core/auth/auth.service';
import { AppContextStore } from '../../core/context/app-context.store';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="dashboard-container">
      <header class="dashboard-header">
        <h1>Dashboard</h1>
        <p class="subtitle">Developer & System Context Panel (Phase 1 Vertical Slice)</p>
      </header>

      <div class="dashboard-grid">
        <!-- Live API Status Diagnostics -->
        <div class="card diagnostics-card full-width">
          <div class="card-header">
            <svg class="card-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z"/>
            </svg>
            <h3>Live Backend Connection Verification</h3>
          </div>
          <div class="card-body diagnostic-grid">
            <div class="diagnostic-item">
              <span class="endpoint-label">GET /api/v1/auth/session</span>
              <span class="status-badge" [ngClass]="authService.sessionApiStatus().status">
                {{ authService.sessionApiStatus().status | uppercase }}
                <span *ngIf="authService.sessionApiStatus().statusCode" class="code-suffix">
                  ({{ authService.sessionApiStatus().statusCode }})
                </span>
              </span>
            </div>

            <div class="diagnostic-item">
              <span class="endpoint-label">GET /api/v1/tenant/resource-limits</span>
              <span class="status-badge" [ngClass]="limitsApiStatus().status">
                {{ limitsApiStatus().status | uppercase }}
                <span *ngIf="limitsApiStatus().statusCode" class="code-suffix">
                  ({{ limitsApiStatus().statusCode }})
                </span>
              </span>
            </div>

            <div class="diagnostic-item">
              <span class="endpoint-label">GET /api/v1/outbox/recent</span>
              <span class="status-badge" [ngClass]="outboxApiStatus().status">
                {{ outboxApiStatus().status | uppercase }}
                <span *ngIf="outboxApiStatus().statusCode" class="code-suffix">
                  ({{ outboxApiStatus().statusCode }})
                </span>
              </span>
            </div>
          </div>
        </div>

        <!-- Section 1: Active Session & User Info -->
        <div class="card session-card">
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
              <span class="value status-badge-old" [class.success]="setupComplete()">
                {{ setupComplete() ? 'Yes' : 'No' }}
              </span>
            </div>
          </div>
        </div>

        <!-- Section 2: Resource Limits & Quotas -->
        <div class="card limits-card">
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
        </div>

        <!-- Section 3: Active Modules & Feature Entitlements -->
        <div class="card modules-card">
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
        </div>

        <!-- Section 4: Permissions -->
        <div class="card permissions-card">
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
        </div>

        <!-- Section 5: Latest Outbox Messages -->
        <div class="card outbox-card full-width">
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
        </div>
      </div>
    </div>
  `,
  styles: [`
    .dashboard-container {
      padding: 2rem;
      background: #0b0f19;
      min-height: 100vh;
      color: #f8fafc;
      font-family: 'Inter', system-ui, -apple-system, sans-serif;
    }
    .dashboard-header {
      margin-bottom: 2rem;
    }
    .dashboard-header h1 {
      font-size: 2.25rem;
      font-weight: 800;
      letter-spacing: -0.025em;
      margin: 0 0 0.5rem 0;
      background: linear-gradient(to right, #38bdf8, #818cf8);
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
    }
    .subtitle {
      color: #94a3b8;
      font-size: 1rem;
      margin: 0;
    }
    .dashboard-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(400px, 1fr));
      gap: 1.5rem;
    }
    .card {
      background: rgba(30, 41, 59, 0.4);
      border-radius: 12px;
      border: 1px solid rgba(255, 255, 255, 0.05);
      padding: 1.5rem;
      backdrop-filter: blur(8px);
      box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
    }
    .card.full-width {
      grid-column: 1 / -1;
    }
    .diagnostics-card {
      background: rgba(30, 41, 59, 0.6);
      border: 1px solid rgba(56, 189, 248, 0.2);
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
      background: rgba(15, 23, 42, 0.4);
      border-radius: 8px;
      border: 1px solid rgba(255, 255, 255, 0.03);
    }
    .endpoint-label {
      font-family: 'JetBrains Mono', monospace;
      font-size: 0.85rem;
      color: #cbd5e1;
    }
    .status-badge {
      font-size: 0.75rem;
      padding: 0.25rem 0.6rem;
      border-radius: 6px;
      font-weight: 700;
      letter-spacing: 0.05em;
    }
    .status-badge.idle {
      background: rgba(148, 163, 184, 0.1);
      color: #94a3b8;
      border: 1px solid rgba(148, 163, 184, 0.2);
    }
    .status-badge.loading {
      background: rgba(56, 189, 248, 0.1);
      color: #38bdf8;
      border: 1px solid rgba(56, 189, 248, 0.2);
      animation: pulse 1.5s infinite;
    }
    .status-badge.success {
      background: rgba(16, 185, 129, 0.1);
      color: #34d399;
      border: 1px solid rgba(16, 185, 129, 0.2);
    }
    .status-badge.error {
      background: rgba(239, 68, 68, 0.1);
      color: #fca5a5;
      border: 1px solid rgba(239, 68, 68, 0.2);
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
      border-bottom: 1px solid rgba(255, 255, 255, 0.05);
      padding-bottom: 0.75rem;
    }
    .card-icon {
      width: 20px;
      height: 20px;
      color: #38bdf8;
    }
    h3 {
      font-size: 1.15rem;
      font-weight: 600;
      color: #f1f5f9;
      margin: 0;
      flex-grow: 1;
    }
    .info-row {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 0.5rem 0;
      border-bottom: 1px solid rgba(255, 255, 255, 0.02);
    }
    .label {
      color: #94a3b8;
      font-size: 0.875rem;
    }
    .value {
      color: #f1f5f9;
      font-weight: 500;
      font-size: 0.9rem;
    }
    .code {
      font-family: 'JetBrains Mono', 'Fira Code', monospace;
      font-size: 0.8rem;
      background: rgba(15, 23, 42, 0.6);
      padding: 0.2rem 0.4rem;
      border-radius: 4px;
      border: 1px solid rgba(255, 255, 255, 0.03);
    }
    .tag {
      font-size: 0.75rem;
      padding: 0.2rem 0.5rem;
      border-radius: 9999px;
      font-weight: 600;
    }
    .module-tag {
      background: rgba(16, 185, 129, 0.1);
      color: #34d399;
      border: 1px solid rgba(16, 185, 129, 0.2);
    }
    .feature-tag {
      background: rgba(99, 102, 241, 0.1);
      color: #a5b4fc;
      border: 1px solid rgba(99, 102, 241, 0.2);
    }
    .permission-tag {
      background: rgba(245, 158, 11, 0.1);
      color: #fbbf24;
      border: 1px solid rgba(245, 158, 11, 0.2);
    }
    .status-badge-old {
      font-size: 0.75rem;
      padding: 0.2rem 0.5rem;
      border-radius: 4px;
      background: rgba(239, 68, 68, 0.1);
      color: #fca5a5;
    }
    .status-badge-old.success {
      background: rgba(16, 185, 129, 0.1);
      color: #34d399;
    }
    .tag-cloud {
      display: flex;
      flex-wrap: wrap;
      gap: 0.5rem;
    }
    .section-title {
      font-size: 0.875rem;
      font-weight: 600;
      color: #94a3b8;
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
      background: rgba(15, 23, 42, 0.2);
      padding: 0.75rem;
      border-radius: 8px;
    }
    .no-data {
      color: #64748b;
      font-size: 0.875rem;
      text-align: center;
      padding: 1.5rem;
    }
    .refresh-btn {
      background: rgba(255, 255, 255, 0.05);
      border: 1px solid rgba(255, 255, 255, 0.1);
      color: #cbd5e1;
      padding: 0.35rem 0.75rem;
      border-radius: 6px;
      font-size: 0.8rem;
      cursor: pointer;
      transition: all 0.2s;
    }
    .refresh-btn:hover {
      background: rgba(255, 255, 255, 0.1);
      color: #ffffff;
    }
    .table-container {
      overflow-x: auto;
      background: rgba(15, 23, 42, 0.3);
      border-radius: 8px;
      border: 1px solid rgba(255, 255, 255, 0.05);
    }
    .outbox-table {
      width: 100%;
      border-collapse: collapse;
      text-align: left;
      font-size: 0.85rem;
    }
    .outbox-table th, .outbox-table td {
      padding: 0.75rem 1rem;
      border-bottom: 1px solid rgba(255, 255, 255, 0.04);
    }
    .outbox-table th {
      background: rgba(15, 23, 42, 0.6);
      color: #94a3b8;
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
