import { Component, EventEmitter, Output, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LegalEntitiesApiService } from '../../core/api/endpoints/legal-entities-api.service';
import { CompanyContextService } from '../../core/context/company-context.service';
import { HasPermissionDirective } from '../../shared/directives/has-permission.directive';

@Component({
  selector: 'app-company-switcher',
  standalone: true,
  imports: [CommonModule, FormsModule, HasPermissionDirective],
  template: `
    <div class="modal-backdrop" (click)="close.emit()">
      <div class="company-switcher-modal" (click)="$event.stopPropagation()">
        <div class="modal-header">
        <h2>Select Legal Entity</h2>
        <button type="button" class="close-btn" (click)="close.emit()">&times;</button>
      </div>

      <div class="modal-body">
        <ng-container *ngIf="!isCreating">
          <div class="company-list">
            <div 
              *ngFor="let company of companies" 
              class="company-item"
              [class.active]="company.id === activeCompanyId()"
              (click)="selectCompany(company.id)">
              <div class="company-info">
                <strong>{{ company.name }}</strong>
                <span class="muted">{{ company.country }} - {{ company.currency }}</span>
              </div>
              <span class="status-badge" *ngIf="company.status === 'active'">Active</span>
            </div>
          </div>
          
          <button type="button" class="btn btn-outline full-width mt-4" *hasPermission="'org:legal-entities:manage'" (click)="isCreating = true">
            + Add New Company
          </button>
        </ng-container>

        <ng-container *ngIf="isCreating">
          <form (ngSubmit)="createCompany()" #f="ngForm" class="create-company-form">
            <div class="form-group">
              <label>Company Legal Name *</label>
              <input type="text" [(ngModel)]="newCompany.name" name="name" required class="form-control" />
            </div>
            <div class="form-group">
              <label>Code</label>
              <input type="text" [(ngModel)]="newCompany.code" name="code" class="form-control" />
            </div>
            <div class="form-group">
              <label>Country *</label>
              <select [(ngModel)]="newCompany.country" name="country" required class="form-control">
                <option value="US">United States</option>
                <option value="UK">United Kingdom</option>
                <option value="CA">Canada</option>
                <option value="IN">India</option>
                <option value="AU">Australia</option>
              </select>
            </div>
            <div class="form-group">
              <label>Currency *</label>
              <select [(ngModel)]="newCompany.currency" name="currency" required class="form-control">
                <option value="USD">USD</option>
                <option value="GBP">GBP</option>
                <option value="CAD">CAD</option>
                <option value="INR">INR</option>
                <option value="AUD">AUD</option>
              </select>
            </div>
            <div class="form-group">
              <label>Timezone *</label>
              <select [(ngModel)]="newCompany.timezone" name="timezone" required class="form-control">
                <option value="UTC">UTC</option>
                <option value="America/New_York">America/New_York</option>
                <option value="America/Los_Angeles">America/Los_Angeles</option>
                <option value="Europe/London">Europe/London</option>
                <option value="Asia/Kolkata">Asia/Kolkata</option>
              </select>
            </div>
            <div class="form-group">
              <label>Address *</label>
              <textarea [(ngModel)]="newCompany.address" name="address" required class="form-control"></textarea>
            </div>

            <div class="form-actions mt-4">
              <button type="button" class="btn btn-outline" (click)="isCreating = false">Cancel</button>
              <button type="submit" class="btn btn-primary" [disabled]="!f.valid || isSaving">
                {{ isSaving ? 'Saving...' : 'Create Company' }}
              </button>
            </div>
          </form>
        </ng-container>
      </div>
    </div>
    </div>
  `,
  styles: [`
    .modal-backdrop {
      position: fixed;
      top: 0; left: 0; right: 0; bottom: 0;
      background: rgba(0,0,0,0.5);
      z-index: 1000;
      display: flex;
      align-items: center;
      justify-content: center;
    }
    .company-switcher-modal {
      width: 400px;
      max-width: 90vw;
      background: var(--content-bg, #ffffff);
      border-radius: 12px;
      box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
      display: flex;
      flex-direction: column;
      color: var(--content-fg);
    }
    .modal-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 16px 20px;
      border-bottom: 1px solid var(--shell-border, rgba(0,0,0,0.1));
    }
    .modal-header h2 {
      margin: 0;
      font-size: 1.125rem;
      font-weight: 600;
    }
    .close-btn {
      background: none;
      border: none;
      font-size: 1.5rem;
      cursor: pointer;
      color: var(--content-fg);
    }
    .modal-body {
      padding: 20px;
      max-height: 70vh;
      overflow-y: auto;
    }
    .company-list {
      display: flex;
      flex-direction: column;
      gap: 8px;
    }
    .company-item {
      padding: 12px 16px;
      border: 1px solid var(--shell-border, rgba(0,0,0,0.1));
      border-radius: 8px;
      cursor: pointer;
      display: flex;
      justify-content: space-between;
      align-items: center;
      transition: all 0.2s ease;
    }
    .company-item:hover {
      background: rgba(0, 0, 0, 0.02);
    }
    .company-item.active {
      border-color: #007bff;
      background: rgba(0, 123, 255, 0.05);
    }
    .company-info {
      display: flex;
      flex-direction: column;
      gap: 4px;
    }
    .muted {
      font-size: 0.75rem;
      color: #6c757d;
    }
    .status-badge {
      font-size: 0.7rem;
      padding: 2px 6px;
      border-radius: 12px;
      background: #e6f4ea;
      color: #1e8e3e;
    }
    .full-width {
      width: 100%;
    }
    .mt-4 { margin-top: 16px; }
    
    .create-company-form {
      display: flex;
      flex-direction: column;
      gap: 12px;
    }
    .form-group {
      display: flex;
      flex-direction: column;
      gap: 4px;
    }
    .form-group label {
      font-size: 0.875rem;
      font-weight: 500;
    }
    .form-control {
      padding: 8px 12px;
      border: 1px solid #ccc;
      border-radius: 6px;
      font-size: 0.875rem;
      background: transparent;
      color: inherit;
    }
    .form-actions {
      display: flex;
      justify-content: flex-end;
      gap: 8px;
    }
  `]
})
export class CompanySwitcherComponent implements OnInit {
  @Output() close = new EventEmitter<void>();

  private api = inject(LegalEntitiesApiService);
  private context = inject(CompanyContextService);

  private cdr = inject(ChangeDetectorRef);

  companies: any[] = [];
  activeCompanyId = this.context.activeCompanyId;
  
  isCreating = false;
  isSaving = false;

  newCompany = {
    name: '',
    code: '',
    country: 'US',
    currency: 'USD',
    timezone: 'UTC',
    address: ''
  };

  ngOnInit() {
    this.loadCompanies();
  }

  loadCompanies() {
    this.api.list().subscribe({
      next: (data) => {
        this.companies = data;
        // If no company is selected but companies exist, select the first one
        if (!this.activeCompanyId() && this.companies.length > 0) {
          this.context.setCompany(this.companies[0].id);
        }
        this.cdr.markForCheck();
      },
      error: (err) => console.error('Failed to load companies', err)
    });
  }

  selectCompany(id: string) {
    this.context.setCompany(id);
    this.close.emit(); // Default to auto-close upon selection
  }

  createCompany() {
    this.isSaving = true;
    this.api.create({
      ...this.newCompany,
      status: 'active'
    }).subscribe({
      next: (created) => {
        this.isSaving = false;
        this.isCreating = false;
        this.companies.push(created);
        this.selectCompany(created.id);
      },
      error: (err) => {
        console.error('Failed to create company', err);
        this.isSaving = false;
        alert('Failed to create company');
      }
    });
  }
}
