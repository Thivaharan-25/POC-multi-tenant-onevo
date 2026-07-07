import { Injectable, signal, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { getTextColorForBackground } from './contrast.util';

export type ColorMode = 'light' | 'dark' | 'system';

export interface ThemeConfig {
  id: string;
  name: string;
  primaryBg: string;
}

export const AVAILABLE_THEMES: ThemeConfig[] = [
  { id: 'default', name: 'Default', primaryBg: '#0f172a' }, // Slate 900
  { id: 'aubergine', name: 'Aubergine', primaryBg: '#4c1d95' }, // Violet 900
  { id: 'jade', name: 'Jade', primaryBg: '#064e3b' }, // Emerald 900
  { id: 'midnight', name: 'Midnight', primaryBg: '#1e3a8a' }, // Blue 900
  { id: 'lagoon', name: 'Lagoon', primaryBg: '#164e63' }, // Cyan 900
  { id: 'clementine', name: 'Clementine', primaryBg: '#7c2d12' }, // Orange 900
  { id: 'custom', name: 'Custom', primaryBg: '#0f172a' } // Overridden by custom picker
];

interface ThemePreferences {
  colorMode: ColorMode;
  themeId: string;
  customColor?: string;
}

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly PREF_KEY = 'onevo.theme.preferences';

  colorMode = signal<ColorMode>('system');
  activeThemeId = signal<string>('default');
  customColor = signal<string>('#0f172a');
  
  // Computed values we'll apply to CSS variables
  isDarkEffective = signal<boolean>(true);

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {
    this.loadPreferences();
    this.applyTheme();
    
    if (isPlatformBrowser(this.platformId)) {
      window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', () => {
        if (this.colorMode() === 'system') {
          this.applyTheme();
        }
      });
    }
  }

  setPreferences(mode: ColorMode, themeId: string, customColor?: string) {
    this.colorMode.set(mode);
    this.activeThemeId.set(themeId);
    if (customColor) {
      this.customColor.set(customColor);
    }
    this.savePreferences();
    this.applyTheme();
  }

  private loadPreferences() {
    if (!isPlatformBrowser(this.platformId)) return;
    
    try {
      const stored = localStorage.getItem(this.PREF_KEY);
      if (stored) {
        const prefs = JSON.parse(stored) as ThemePreferences;
        this.colorMode.set(prefs.colorMode || 'system');
        this.activeThemeId.set(prefs.themeId || 'default');
        if (prefs.customColor) {
          this.customColor.set(prefs.customColor);
        }
      }
    } catch (e) {
      console.error('Failed to load theme preferences', e);
    }
  }

  private savePreferences() {
    if (!isPlatformBrowser(this.platformId)) return;
    
    const prefs: ThemePreferences = {
      colorMode: this.colorMode(),
      themeId: this.activeThemeId(),
      customColor: this.customColor()
    };
    localStorage.setItem(this.PREF_KEY, JSON.stringify(prefs));
  }

  private applyTheme() {
    if (!isPlatformBrowser(this.platformId)) return;
    
    // Determine effective dark mode
    let isDark = true;
    if (this.colorMode() === 'light') isDark = false;
    else if (this.colorMode() === 'system') {
      isDark = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
    }
    this.isDarkEffective.set(isDark);

    const root = document.documentElement;

    // Apply color scheme
    if (isDark) {
      root.classList.add('dark');
      root.style.setProperty('--content-bg', '#1e293b'); // Slate 800
      root.style.setProperty('--content-fg', '#f8fafc'); // Slate 50
      root.style.setProperty('--border-color', 'rgba(255,255,255,0.08)');
      root.style.setProperty('--app-bg', '#0f172a'); // Shell background fallback
    } else {
      root.classList.remove('dark');
      root.style.setProperty('--content-bg', '#ffffff');
      root.style.setProperty('--content-fg', '#0f172a'); // Slate 900
      root.style.setProperty('--border-color', 'rgba(0,0,0,0.1)');
      root.style.setProperty('--app-bg', '#f1f5f9'); // Shell background fallback
    }

    // Apply theme color
    const themeConfig = AVAILABLE_THEMES.find(t => t.id === this.activeThemeId());
    let primaryBg = themeConfig?.primaryBg || '#0f172a';
    if (this.activeThemeId() === 'custom') {
      primaryBg = this.customColor();
    }

    const fgColor = getTextColorForBackground(primaryBg);

    // Set shell variables
    root.style.setProperty('--shell-bg', primaryBg);
    root.style.setProperty('--shell-fg', fgColor);
    
    // Slight opacity for muted text on shell
    root.style.setProperty('--shell-muted-fg', fgColor === '#ffffff' ? 'rgba(255,255,255,0.7)' : 'rgba(0,0,0,0.6)');
    
    root.style.setProperty('--shell-active-bg', fgColor === '#ffffff' ? 'rgba(255,255,255,0.15)' : 'rgba(0,0,0,0.1)');
    root.style.setProperty('--shell-active-fg', fgColor);
    
    root.style.setProperty('--topbar-bg', primaryBg);
    root.style.setProperty('--topbar-fg', fgColor);
    
    root.style.setProperty('--nav-active-bg', fgColor === '#ffffff' ? 'rgba(255,255,255,0.1)' : 'rgba(0,0,0,0.05)');
    root.style.setProperty('--nav-active-fg', fgColor);
    
    root.style.setProperty('--primary', primaryBg);
    root.style.setProperty('--primary-fg', fgColor);
  }
}
