import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/account.service';

export const signedInGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const router = inject(Router);

  if (!accountService.currentUser()) {
    router.navigate(['/signin'], {
      queryParams: { redirectUrl: state.url },
    });
    return false;
  }
  return true;
};
