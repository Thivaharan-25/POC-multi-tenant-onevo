import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CompanyContextService {
  private readonly STORAGE_KEY = 'ov_active_company_id';
  
  public readonly activeCompanyId = signal<string | null>(this.getStoredCompanyId());

  constructor() {
  }

  setCompany(id: string | null) {
    this.activeCompanyId.set(id);
    if (id) {
      localStorage.setItem(this.STORAGE_KEY, id);
    } else {
      localStorage.removeItem(this.STORAGE_KEY);
    }
  }

  private getStoredCompanyId(): string | null {
    return localStorage.getItem(this.STORAGE_KEY);
  }
}
