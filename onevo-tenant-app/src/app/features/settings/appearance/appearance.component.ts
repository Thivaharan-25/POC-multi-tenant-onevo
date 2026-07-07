import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ThemeService, AVAILABLE_THEMES } from '../../../core/theme/theme.service';
import { PageHeaderComponent } from '../../../shared/ui/page-header/page-header.component';
import { CardComponent } from '../../../shared/ui/card/card.component';
import { PageShellComponent } from '../../../shared/ui/page-shell/page-shell.component';

@Component({
  selector: 'app-appearance',
  standalone: true,
  imports: [CommonModule, PageHeaderComponent, CardComponent, PageShellComponent],
  template: `
    <ov-page-shell>
      <ov-page-header title="Appearance" subtitle="Customize the look and feel of OneVo"></ov-page-header>
      
      <ov-card>
        <h3 class="settings-title">Color Mode</h3>
        <p class="settings-desc">Choose whether OneVo appears light, dark, or follows your system setting.</p>
        
        <div class="options-group">
          <button class="option-btn" 
                  [class.active]="theme.colorMode() === 'light'"
                  (click)="theme.setPreferences('light', theme.activeThemeId(), theme.customColor())">
            <svg viewBox="0 0 24 24"><path d="M12 7c-2.76 0-5 2.24-5 5s2.24 5 5 5 5-2.24 5-5-2.24-5-5-5zM2 13h2c.55 0 1-.45 1-1s-.45-1-1-1H2c-.55 0-1 .45-1 1s.45 1 1 1zm18 0h2c.55 0 1-.45 1-1s-.45-1-1-1h-2c-.55 0-1 .45-1 1s.45 1 1 1zM11 2v2c0 .55.45 1 1 1s1-.45 1-1V2c0-.55-.45-1-1-1s-1 .45-1 1zm0 18v2c0 .55.45 1 1 1s1-.45 1-1v-2c0-.55-.45-1-1-1s-1 .45-1 1zM5.99 4.58c-.39-.39-1.03-.39-1.41 0-.39.39-.39 1.03 0 1.41l1.06 1.06c.39.39 1.03.39 1.41 0 .39-.39.39-1.03 0-1.41L5.99 4.58zm12.37 12.37c-.39-.39-1.03-.39-1.41 0-.39.39-.39 1.03 0 1.41l1.06 1.06c.39.39 1.03.39 1.41 0 .39-.39.39-1.03 0-1.41l-1.06-1.06zm1.06-10.96c.39-.39.39-1.03 0-1.41-.39-.39-1.03-.39-1.41 0l-1.06 1.06c-.39.39-.39 1.03 0 1.41.39.39 1.03.39 1.41 0l1.06-1.06zM7.05 18.36c.39-.39.39-1.03 0-1.41-.39-.39-1.03-.39-1.41 0l-1.06 1.06c-.39.39-.39 1.03 0 1.41.39.39 1.03.39 1.41 0l1.06-1.06z"/></svg>
            Light
          </button>
          <button class="option-btn"
                  [class.active]="theme.colorMode() === 'dark'"
                  (click)="theme.setPreferences('dark', theme.activeThemeId(), theme.customColor())">
            <svg viewBox="0 0 24 24"><path d="M9.37,5.51C9.19,6.15,9.1,6.82,9.1,7.5c0,4.08,3.32,7.4,7.4,7.4c0.68,0,1.35-0.09,1.99-0.27C17.45,17.19,14.93,19,12,19 c-3.86,0-7-3.14-7-7C5,9.07,6.81,6.55,9.37,5.51z M12,3c-4.97,0-9,4.03-9,9s4.03,9,9,9s9-4.03,9-9c0-0.46-0.04-0.92-0.1-1.36 c-0.98,1.37-2.58,2.26-4.4,2.26c-2.98,0-5.4-2.42-5.4-5.4c0-1.81,0.89-3.42,2.26-4.4C12.92,3.04,12.46,3,12,3L12,3z"/></svg>
            Dark
          </button>
          <button class="option-btn"
                  [class.active]="theme.colorMode() === 'system'"
                  (click)="theme.setPreferences('system', theme.activeThemeId(), theme.customColor())">
            <svg viewBox="0 0 24 24"><path d="M20,18c1.1,0,1.99-0.9,1.99-2L22,6c0-1.1-0.9-2-2-2H4C2.9,4,2,4.9,2,6v10c0,1.1,0.9,2,2,2H0v2h24v-2H20z M4,6h16v10H4V6z"/></svg>
            System
          </button>
        </div>
      </ov-card>

      <ov-card>
        <h3 class="settings-title">Themes</h3>
        <p class="settings-desc">Pick a color for the sidebar, topbar, and app frame.</p>
        
        <div class="themes-grid">
          <button *ngFor="let t of availableThemes" 
                  class="theme-btn" 
                  [class.active]="theme.activeThemeId() === t.id"
                  (click)="theme.setPreferences(theme.colorMode(), t.id, theme.customColor())">
            <div class="theme-color-circle" 
                 [style.background]="t.id === 'custom' ? theme.customColor() : t.primaryBg"
                 [class.default-split]="t.id === 'default'"></div>
            <span>{{ t.name }}</span>
          </button>
        </div>
        
        <div class="custom-color-picker" *ngIf="theme.activeThemeId() === 'custom'">
          <label>Custom Hex Color:</label>
          <input type="color" 
                 [value]="theme.customColor()" 
                 (change)="onCustomColorChange($event)" />
        </div>
      </ov-card>
    </ov-page-shell>
  `,
  styles: [`
    .settings-title {
      margin: 0 0 8px 0;
      font-size: 1.1rem;
      font-weight: 600;
    }
    .settings-desc {
      margin: 0 0 24px 0;
      font-size: 0.875rem;
      color: var(--shell-muted-fg);
    }
    
    .options-group {
      display: flex;
      gap: 16px;
    }
    .option-btn {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 10px 20px;
      border: 1px solid var(--border-color);
      border-radius: 8px;
      background: transparent;
      color: var(--content-fg);
      font-weight: 500;
      font-size: 0.9rem;
      cursor: pointer;
      transition: all 0.2s;
    }
    .option-btn svg {
      width: 20px;
      height: 20px;
      fill: currentColor;
    }
    .option-btn:hover {
      background: rgba(128, 128, 128, 0.05);
    }
    .option-btn.active {
      border-color: var(--primary);
      background: rgba(var(--primary-rgb, 128, 128, 128), 0.1); /* fallback */
      color: var(--primary);
      box-shadow: 0 0 0 1px var(--primary);
    }
    
    .themes-grid {
      display: flex;
      flex-wrap: wrap;
      gap: 16px;
    }
    .theme-btn {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 12px;
      padding: 16px;
      border: 1px solid var(--border-color);
      border-radius: 12px;
      background: transparent;
      color: var(--content-fg);
      font-weight: 500;
      font-size: 0.85rem;
      cursor: pointer;
      transition: all 0.2s;
      min-width: 100px;
    }
    .theme-btn:hover {
      border-color: var(--shell-muted-fg);
    }
    .theme-btn.active {
      border-color: var(--primary);
      box-shadow: 0 0 0 1px var(--primary);
    }
    
    .theme-color-circle {
      width: 48px;
      height: 48px;
      border-radius: 50%;
      border: 1px solid rgba(0,0,0,0.1);
    }
    .theme-color-circle.default-split {
      background: linear-gradient(135deg, #0f172a 50%, #ffffff 50%);
    }
    
    .custom-color-picker {
      margin-top: 24px;
      display: flex;
      align-items: center;
      gap: 12px;
    }
    .custom-color-picker label {
      font-size: 0.9rem;
      font-weight: 500;
    }
    .custom-color-picker input[type="color"] {
      width: 40px;
      height: 40px;
      border: none;
      border-radius: 4px;
      cursor: pointer;
    }
  `]
})
export class AppearanceComponent {
  theme = inject(ThemeService);
  availableThemes = AVAILABLE_THEMES;

  onCustomColorChange(event: Event) {
    const input = event.target as HTMLInputElement;
    this.theme.setPreferences(this.theme.colorMode(), 'custom', input.value);
  }
}
