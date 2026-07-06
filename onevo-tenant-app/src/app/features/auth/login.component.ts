import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthApiService } from '../../core/api/endpoints/auth-api.service';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="login-container">
      <div class="login-card">
        <div class="login-header">
          <div class="logo">OneVo HR</div>
          <h2>Sign in to your account</h2>
          <p class="subtitle">Enter your credentials below</p>
        </div>
        
        <form (ngSubmit)="onSubmit()" #loginForm="ngForm" class="login-form">
          <div class="form-group">
            <label for="email">Email address</label>
            <input 
              id="email" 
              name="email" 
              type="email" 
              required 
              [(ngModel)]="email" 
              #emailInput="ngModel"
              placeholder="name@company.com"
              class="form-control"
              [class.error]="emailInput.invalid && emailInput.touched"
            />
          </div>
          
          <div class="form-group">
            <label for="password">Password</label>
            <input 
              id="password" 
              name="password" 
              type="password" 
              required 
              [(ngModel)]="password" 
              #passwordInput="ngModel"
              placeholder="••••••••"
              class="form-control"
              [class.error]="passwordInput.invalid && passwordInput.touched"
            />
          </div>

          <div *ngIf="errorMessage()" class="error-alert">
            <svg class="error-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"/>
            </svg>
            <span>{{ errorMessage() }}</span>
          </div>
          
          <button type="submit" [disabled]="loginForm.invalid || isLoading()" class="submit-btn">
            <span *ngIf="!isLoading()">Sign In</span>
            <span *ngIf="isLoading()" class="spinner-container">
              <span class="spinner"></span>
              Signing in...
            </span>
          </button>
        </form>

        <!-- Developer Credentials Helper Hint -->
        <div class="dev-hint">
          <div class="hint-title">Demo Credentials</div>
          <div class="hint-item"><span>HR Admin:</span> <code>hr.admin@acme.test</code></div>
          <div class="hint-item"><span>Owner:</span> <code>owner@acme.test</code></div>
          <div class="hint-item"><span>Password:</span> <code>Password123!</code></div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .login-container {
      display: flex;
      align-items: center;
      justify-content: center;
      min-height: 100vh;
      background: radial-gradient(circle at top left, #1e293b, #0f172a);
      font-family: 'Inter', system-ui, -apple-system, sans-serif;
      padding: 1.5rem;
    }
    .login-card {
      width: 100%;
      max-width: 440px;
      padding: 2.5rem;
      border-radius: 16px;
      background: rgba(30, 41, 59, 0.7);
      backdrop-filter: blur(12px);
      border: 1px solid rgba(255, 255, 255, 0.08);
      box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.3), 0 10px 10px -5px rgba(0, 0, 0, 0.2);
    }
    .login-header {
      text-align: center;
      margin-bottom: 2rem;
    }
    .logo {
      font-size: 1.8rem;
      font-weight: 800;
      letter-spacing: -0.05em;
      color: #38bdf8;
      margin-bottom: 0.75rem;
    }
    h2 {
      font-size: 1.5rem;
      font-weight: 700;
      color: #f8fafc;
      margin: 0 0 0.5rem 0;
    }
    .subtitle {
      font-size: 0.875rem;
      color: #94a3b8;
      margin: 0;
    }
    .login-form {
      display: flex;
      flex-direction: column;
      gap: 1.25rem;
    }
    .form-group {
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
    }
    label {
      font-size: 0.875rem;
      font-weight: 500;
      color: #cbd5e1;
    }
    .form-control {
      padding: 0.75rem 1rem;
      border-radius: 8px;
      border: 1px solid rgba(255, 255, 255, 0.1);
      background: rgba(15, 23, 42, 0.6);
      color: #f8fafc;
      font-size: 0.95rem;
      transition: all 0.2s ease;
    }
    .form-control:focus {
      outline: none;
      border-color: #38bdf8;
      box-shadow: 0 0 0 3px rgba(56, 189, 248, 0.15);
    }
    .form-control.error {
      border-color: #ef4444;
    }
    .error-alert {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      padding: 0.75rem 1rem;
      background: rgba(239, 68, 68, 0.1);
      border: 1px solid rgba(239, 68, 68, 0.2);
      border-radius: 8px;
      color: #fca5a5;
      font-size: 0.875rem;
    }
    .error-icon {
      width: 20px;
      height: 20px;
      flex-shrink: 0;
    }
    .submit-btn {
      margin-top: 0.5rem;
      padding: 0.75rem 1rem;
      border-radius: 8px;
      border: none;
      background: linear-gradient(135deg, #0ea5e9, #0284c7);
      color: #ffffff;
      font-weight: 600;
      font-size: 0.95rem;
      cursor: pointer;
      transition: all 0.2s ease;
      display: flex;
      justify-content: center;
      align-items: center;
    }
    .submit-btn:hover:not(:disabled) {
      background: linear-gradient(135deg, #38bdf8, #0ea5e9);
      transform: translateY(-1px);
    }
    .submit-btn:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }
    .spinner-container {
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }
    .spinner {
      width: 16px;
      height: 16px;
      border: 2px solid rgba(255, 255, 255, 0.3);
      border-top-color: #ffffff;
      border-radius: 50%;
      animation: spin 0.8s linear infinite;
    }
    @keyframes spin {
      to { transform: rotate(360deg); }
    }
    .dev-hint {
      margin-top: 1.5rem;
      padding: 1rem;
      background: rgba(56, 189, 248, 0.05);
      border: 1px solid rgba(56, 189, 248, 0.15);
      border-radius: 8px;
      font-size: 0.8rem;
    }
    .hint-title {
      font-weight: 700;
      color: #38bdf8;
      margin-bottom: 0.5rem;
      text-transform: uppercase;
      letter-spacing: 0.05em;
    }
    .hint-item {
      display: flex;
      justify-content: space-between;
      color: #94a3b8;
      margin-bottom: 0.25rem;
    }
    .hint-item span {
      font-weight: 500;
    }
    .hint-item code {
      color: #e2e8f0;
      background: rgba(15, 23, 42, 0.4);
      padding: 0.1rem 0.3rem;
      border-radius: 4px;
    }
  `]
})
export class LoginComponent {
  private authApi = inject(AuthApiService);
  private authService = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  email = '';
  password = '';
  errorMessage = signal<string | null>(null);
  isLoading = signal(false);

  onSubmit(): void {
    if (this.isLoading()) return;
    if (!this.email || !this.password) return;

    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.authApi.login(this.email, this.password).subscribe({
      next: (session) => {
        this.authService.setSession(session);
        const returnUrl = this.route.snapshot.queryParams['redirect'] || '/dashboard';
        this.router.navigateByUrl(returnUrl);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.errorMessage.set('Invalid email or password.');
      }
    });
  }
}
