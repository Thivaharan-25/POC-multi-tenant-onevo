import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class SignalrService {
  connected = signal(false);

  connect(): void {
    this.connected.set(true);
  }

  disconnect(): void {
    this.connected.set(false);
  }
}
