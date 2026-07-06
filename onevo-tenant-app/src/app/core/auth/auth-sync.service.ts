import { Injectable, OnDestroy, inject } from '@angular/core';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class AuthSyncService implements OnDestroy {
  private auth = inject(AuthService);
  private channel = new BroadcastChannel('onevo-auth');

  constructor() {
    this.channel.onmessage = event => {
      if (event.data?.type === 'LOGOUT') {
        this.auth.clear();
      }
    };
  }

  broadcastLogout(): void {
    this.channel.postMessage({ type: 'LOGOUT' });
  }

  ngOnDestroy(): void {
    this.channel.close();
  }
}
