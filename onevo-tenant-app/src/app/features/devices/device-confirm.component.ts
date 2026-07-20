import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { DevicesApiService, PairConfirmInfo } from '../../core/api/endpoints/devices-api.service';
import { AuthService } from '../../core/auth/auth.service';

type ConfirmState = 'loading' | 'ready' | 'confirmed' | 'declined' | 'error';

@Component({
  selector: 'app-device-confirm',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="confirm-container">
      <div class="confirm-card">
        <ng-container [ngSwitch]="state()">

          <div *ngSwitchCase="'loading'" class="status-block">
            <div class="spinner"></div>
            <p>Loading device details...</p>
          </div>

          <div *ngSwitchCase="'ready'">
            <h2>Connect this desktop?</h2>

            <dl class="details">
              <dt>Device</dt>
              <dd>{{ info()?.deviceName }}</dd>
              <dt>Signed in as</dt>
              <dd>{{ user()?.displayName }} ({{ user()?.email }})</dd>
            </dl>

            <div *ngIf="errorMessage()" class="error-alert">{{ errorMessage() }}</div>

            <div class="actions">
              <button type="button" class="btn-secondary" [disabled]="isSubmitting()" (click)="onDecline()">Cancel</button>
              <button type="button" class="btn-primary" [disabled]="isSubmitting()" (click)="onConfirm()">Confirm Device</button>
            </div>
          </div>

          <div *ngSwitchCase="'confirmed'" class="status-block">
            <h2>Device connected</h2>
            <p>You can return to the OneVoTray app on your desktop now. You can close this tab.</p>
          </div>

          <div *ngSwitchCase="'declined'" class="status-block">
            <h2>Sign-in cancelled</h2>
            <p>This desktop was not connected. You can close this tab.</p>
          </div>

          <div *ngSwitchCase="'error'" class="status-block">
            <h2>This code is invalid or has expired</h2>
            <p>Go back to the OneVoTray app and try signing in again.</p>
          </div>

        </ng-container>
      </div>
    </div>
  `,
  styles: [`
    .confirm-container {
      display: flex;
      align-items: center;
      justify-content: center;
      min-height: 100vh;
      background: radial-gradient(circle at top left, #1e293b, #0f172a);
      font-family: 'Inter', system-ui, -apple-system, sans-serif;
      padding: 1.5rem;
    }
    .confirm-card {
      width: 100%;
      max-width: 440px;
      padding: 2.5rem;
      border-radius: 16px;
      background: rgba(30, 41, 59, 0.7);
      backdrop-filter: blur(12px);
      border: 1px solid rgba(255, 255, 255, 0.08);
      box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.3), 0 10px 10px -5px rgba(0, 0, 0, 0.2);
      color: #f8fafc;
    }
    h2 {
      font-size: 1.35rem;
      font-weight: 700;
      margin: 0 0 1.25rem 0;
      text-align: center;
    }
    .details {
      display: grid;
      grid-template-columns: auto 1fr;
      gap: 0.5rem 1rem;
      margin: 0 0 1.5rem 0;
      font-size: 0.9rem;
    }
    .details dt {
      color: #94a3b8;
      font-weight: 500;
    }
    .details dd {
      margin: 0;
      color: #f8fafc;
    }
    .actions {
      display: flex;
      gap: 0.75rem;
      justify-content: center;
    }
    .btn-primary, .btn-secondary {
      padding: 0.7rem 1.25rem;
      border-radius: 8px;
      font-weight: 600;
      font-size: 0.9rem;
      cursor: pointer;
      border: none;
      transition: all 0.2s ease;
    }
    .btn-primary {
      background: linear-gradient(135deg, #0ea5e9, #0284c7);
      color: #ffffff;
    }
    .btn-primary:hover:not(:disabled) {
      background: linear-gradient(135deg, #38bdf8, #0ea5e9);
    }
    .btn-secondary {
      background: rgba(148, 163, 184, 0.1);
      color: #cbd5e1;
      border: 1px solid rgba(255, 255, 255, 0.1);
    }
    .btn-primary:disabled, .btn-secondary:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }
    .status-block {
      text-align: center;
      color: #cbd5e1;
    }
    .status-block p {
      font-size: 0.9rem;
      color: #94a3b8;
    }
    .error-alert {
      margin: 0 0 1.25rem 0;
      padding: 0.75rem 1rem;
      background: rgba(239, 68, 68, 0.1);
      border: 1px solid rgba(239, 68, 68, 0.2);
      border-radius: 8px;
      color: #fca5a5;
      font-size: 0.85rem;
      text-align: center;
    }
    .spinner {
      width: 28px;
      height: 28px;
      margin: 0 auto 1rem auto;
      border: 3px solid rgba(255, 255, 255, 0.15);
      border-top-color: #38bdf8;
      border-radius: 50%;
      animation: spin 0.8s linear infinite;
    }
    @keyframes spin {
      to { transform: rotate(360deg); }
    }
  `]
})
export class DeviceConfirmComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private devicesApi = inject(DevicesApiService);
  private authService = inject(AuthService);

  state = signal<ConfirmState>('loading');
  info = signal<PairConfirmInfo | null>(null);
  errorMessage = signal<string | null>(null);
  isSubmitting = signal(false);
  user = this.authService.user;

  private userCode = '';

  ngOnInit(): void {
    this.userCode = this.route.snapshot.queryParams['code'] || '';

    if (!this.userCode) {
      this.state.set('error');
      return;
    }

    this.devicesApi.getConfirmationInfo(this.userCode).subscribe({
      next: (info) => {
        this.info.set(info);
        this.state.set('ready');
      },
      error: () => this.state.set('error')
    });
  }

  onConfirm(): void {
    if (this.isSubmitting()) return;
    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    this.devicesApi.confirm(this.userCode).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.state.set('confirmed');
      },
      error: () => {
        this.isSubmitting.set(false);
        this.errorMessage.set('Could not connect this desktop. Please try again.');
      }
    });
  }

  onDecline(): void {
    if (this.isSubmitting()) return;
    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    this.devicesApi.decline(this.userCode).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.state.set('declined');
      },
      error: () => {
        this.isSubmitting.set(false);
        this.errorMessage.set('Could not cancel. Please try again.');
      }
    });
  }
}
