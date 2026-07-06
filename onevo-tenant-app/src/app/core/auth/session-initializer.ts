import { APP_INITIALIZER, Provider, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { AuthService } from './auth.service';

export const initializeSessionProvider: Provider = {
  provide: APP_INITIALIZER,
  multi: true,
  useFactory: () => {
    const auth = inject(AuthService);
    return () => firstValueFrom(auth.initializeSession());
  }
};
